using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimCameraController : MonoBehaviour
{
    [SerializeField] Transform yawTarget;
    [SerializeField] Transform pitchTarget;

    [SerializeField] InputActionReference lookInput;
    [SerializeField] InputActionReference switchShouldInput;

    [SerializeField] float sensitivity = 1.5f;
    [SerializeField] float mouseSensitivity = 0.05f;

    [SerializeField] float pitchMin = -40f;
    [SerializeField] float pitchMax = 80f;

    [SerializeField] float shoulderSwitchSpeed = 5f;

    float yaw;
    float pitch;
    float targetCameraSide;

    Vector2 look;

    CinemachineThirdPersonFollow aimCam;
    void Awake()
    {
        //aimCam = GetComponent<CinemachineThirdPersonFollow>();
        //targetCameraSide = aimCam.CameraSide;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Vector3 angles = yawTarget.rotation.eulerAngles;
        //yaw = angles.y;
        //pitch = angles.x;

        //lookInput.asset.Enable();
    }
    //Nécessaire?
    //void OnEnable()
    //{
    //    switchShouldInput.action.Enable();
    //    switchShouldInput.action.performed += OnSwitchShoulder;
    //}
    //void OnDisable()
    //{
    //    switchShouldInput.action.Disable();
    //    switchShouldInput.action.performed -= OnSwitchShoulder;
    //}
    //void OnSwitchShoulder(InputAction.CallbackContext context)
    //{
    //    targetCameraSide = aimCam.CameraSide < 0.5f ? 1f : 0f;
    //}


    // Update is called once per frame
    void Update()
    {
        //Vector2 look = lookInput.action.ReadValue<Vector2>();

        //if(Mouse.current != null && Mouse.current.delta.IsActuated())
        //{
        //    look *= mouseSensitivity;
        //}

        //yaw += look.x * sensitivity;
        //pitch -= look.y * sensitivity;

        //yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        //pitchTarget.rotation = Quaternion.Euler(pitch, 0f, 0f);

        //aimCam.CameraSide = Mathf.Lerp(aimCam.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);
    }

    //public void SetYawPitchFromCameraForward(Transform cameraTransform)
    //{
    //    Vector3 flatForward = cameraTransform.forward;
    //    flatForward.y = 0f;

    //    if(flatForward.sqrMagnitude < 0.001f)
    //        return;

    //    yaw = Quaternion.LookRotation(flatForward).eulerAngles.y;

    //    yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
    //    pitchTarget.localRotation = Quaternion.Euler(0f, 0f, 0f);
    //}
}
