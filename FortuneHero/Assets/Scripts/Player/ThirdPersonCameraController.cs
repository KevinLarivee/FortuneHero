using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] float zoomSpeed = 2f;
    [SerializeField] float zoomLerpSpeed = 10f;
    [SerializeField] float minDistance = 3f;
    [SerializeField] float maxDistance = 15f;

    InputSystem_Actions controls;

    CinemachineCamera cam;
    CinemachineOrbitalFollow orbital;
    Vector2 scrollDelta;

    float targetZoom;
    float currentZoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new InputSystem_Actions();
        controls.Enable();
        controls.UI.ScrollWheel.performed += HandleMouseScroll;

        cam = GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();

        targetZoom = currentZoom = orbital.Radius;
    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if(scrollDelta.y != 0)
        {
            if(orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance);
                scrollDelta = Vector2.zero;
            }
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }
}
