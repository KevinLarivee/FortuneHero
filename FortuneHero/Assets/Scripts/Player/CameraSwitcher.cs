using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] CinemachineCamera freelookCam;
    [SerializeField] CinemachineCamera aimCam;
    
    CinemachineInputAxisController inputAxisController;
    //AimCameraController aimCamController;
    PlayerMovement player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //aimCamController = aimCam.GetComponent<AimCameraController>();
        //player = PlayerMovement.Instance;
        //inputAxisController = freelookCam.GetComponent<CinemachineInputAxisController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExitAimMode()
    {
        //player.isAiming = false;

        //SnapFreeLookBehindPlayer();

        //aimCam.Priority = 10;
        //freelookCam.Priority = 20;

        //inputAxisController.enabled = true;
    }

    private void SnapFreeLookBehindPlayer()
    {
        //CinemachineOrbitalFollow orbitalFollow = freelookCam.GetComponent<CinemachineOrbitalFollow>();
        //Vector3 forward = aimCam.transform.forward;
        //float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        //orbitalFollow.HorizontalAxis.Value = angle;
    }

    private void SnapAimCameraToPlayerForward()
    {
        //aimCamController.SetYawPitchFromCameraForward(freelookCam.transform);
    }

    public void EnterAimMode()
    {
        //player.isAiming = true;

        //SnapAimCameraToPlayerForward();

        //aimCam.Priority = 20;
        //freelookCam.Priority = 10;

        //inputAxisController.enabled = false;
    }
}
