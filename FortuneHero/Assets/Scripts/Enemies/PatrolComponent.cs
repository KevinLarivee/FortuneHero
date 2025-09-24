using UnityEditor;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using System;

public enum PatrolType { Loop, Reverse, Random, Chaos};
public class PatrolComponent : MonoBehaviour
{
    [SerializeField] Transform[] targets;
    [SerializeField, Range(0f, 1f)] float lookAroundProbability = 0f;

    [SerializeField] PatrolType patrolType = PatrolType.Loop;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Loop)] float isLooping;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Reverse)] float isReverse;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Random)] float isRandom;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Chaos)] float isChaos;

    public bool isActive;

    NavMeshAgent agent;
    int currentTarget = 0;

    Action Move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Si null, alors déplace manuellement
        if((agent = GetComponent<NavMeshAgent>()) == null)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            transform.Translate(targets[currentTarget].position);
            if (true)
                currentTarget = (currentTarget + 1) % targets.Length;
        }
    }
}

//[CustomEditor(typeof(PatrolComponent))]
//public class PatrolComponent_Editor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        var script = (PatrolComponent)target;

//        script.patrolType = (PatrolType)EditorGUILayout.EnumPopup("Patrol Type", script.patrolType);

//        switch (script.patrolType)
//        {
//            case PatrolType.Loop:
//                script.isLooping = EditorGUILayout.FloatField("Is Looping", script.isLooping);
//                break;

//            case PatrolType.Reverse:
//                script.isReverse = EditorGUILayout.FloatField("Is Reverse", script.isReverse);
//                break;

//            case PatrolType.Random:
//                script.isRandom = EditorGUILayout.FloatField("Is Random", script.isRandom);
//                break;

//            case PatrolType.Chaos:
//                script.isChaos = EditorGUILayout.FloatField("Is Chaos", script.isChaos);
//                break;
//        }

//        //if (GUI.changed)
//        //{
//        //    EditorUtility.SetDirty(script);
//        //}
//    }
//}
