using UnityEngine;

public class WorldCrosshairController : MonoBehaviour
{
    [SerializeField] RectTransform crosshairUI;
    [SerializeField] Camera aimCamera;
    [SerializeField] float maxDistance = 20f;
    [SerializeField] float crossHairOffsetMultiplier = 0.01f;
    [SerializeField] LayerMask raycastMask = ~0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = aimCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPos;
        if(Physics.Raycast(ray, out RaycastHit hit, maxDistance, raycastMask))
        {
            targetPos = hit.point + hit.normal * crossHairOffsetMultiplier;
            crosshairUI.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            targetPos = ray.GetPoint(maxDistance);
            crosshairUI.forward = aimCamera.transform.forward;
        }

        crosshairUI.position = targetPos;
    }
}
