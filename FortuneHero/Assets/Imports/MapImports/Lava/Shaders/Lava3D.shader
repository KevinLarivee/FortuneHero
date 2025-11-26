/*
URP version of the original "Ultimate 10+ Shaders/Lava3D" shader.
- Migrated away from Surface Shader (not supported in URP)
- Uses an Unlit forward pass with emissive color (good for lava)
- Keeps original property names (_MainTex, _HeightMap, _Color, _FlowDirection, _Speed, _Amplitude, _Cull)
- Adds SRP Batcher–friendly CBUFFER and URP includes
*/

Shader "Ultimate 10+ Shaders/Lava3D_URP"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _HeightMap ("Height Map (Black and White)", 2D) = "black" {}
        _FlowDirection ("Flow Direction", Vector) = (1, 0, 0, 0)
        _Speed ("Speed", float) = 0.25
        _Amplitude ("Amplitude", float) = 1.0

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
            "RenderType"     = "Opaque"
            "Queue"          = "Geometry"
            "UniversalMaterialType" = "Unlit"
        }

        LOD 150
        Cull [_Cull]

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            // ===== URP Includes =====
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ===== Keywords (none needed for basic unlit) =====

            // ===== Textures & Samplers =====
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_HeightMap);
            SAMPLER(sampler_HeightMap);

            // Unity supplies _Time as float4
            // x = t/20, y = t, z = t*2, w = t*3 (seconds)
            // We will use _Time.y like in the original
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;

                float4 _MainTex_ST;    // for tiling/offset on _MainTex
                float4 _HeightMap_ST;  // for tiling/offset on _HeightMap

                float4 _FlowDirection; // (x,y,z,w) we will use xy for UVs
                float   _Speed;
                float   _Amplitude;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv0        : TEXCOORD0;
                float2 uv1        : TEXCOORD1; // used by original height sampling
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uvMain      : TEXCOORD0;
                float2 uvHeight    : TEXCOORD1;
                // If needed later, we could pass world pos/normal
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Helper to get scrolling offset
            float2 GetScrollOffset()
            {
                // Match original: IN.uv + _FlowDirection * fmod(_Time.y, 1200) * _Speed
                // fmod can be dropped (wrap happens in UV frac), but we’ll keep behavior similar
                float t = fmod(_Time.y, 1200.0);
                return _FlowDirection.xy * (t * _Speed);
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                // Compute scrolling UVs
                float2 scroll = GetScrollOffset();

                // Base UVs with tiling/offset
                float2 uvMain   = TRANSFORM_TEX(IN.uv0, _MainTex)   + scroll;
                float2 uvHeight = TRANSFORM_TEX(IN.uv1, _HeightMap) + scroll;

                // Sample height in the vertex stage (LOD 0 is fine for standard meshes)
                float height = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uvHeight, 0).r;

                // Displace the vertex in object space along Y like original
                float3 posOS = IN.positionOS.xyz;
                posOS += IN.normalOS * height * _Amplitude;

                // Output
                OUT.positionHCS = TransformObjectToHClip(float4(posOS, 1.0));
                OUT.uvMain      = uvMain;
                OUT.uvHeight    = uvHeight;

                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                // Sample main albedo and tint by _Color
                float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uvMain) * _Color;

                // Unlit: return emissive-looking lava
                // (If you want HDR bloom, ensure the post-processing bloom is enabled.)
                return albedo;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
