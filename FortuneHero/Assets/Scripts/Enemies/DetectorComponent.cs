using System;
using Unity.VisualScripting;
using UnityEngine;

//https://youtu.be/CSeUMTaNFYk?si=iJySCe-ZcVHSxKwn

public class DetectorComponent : MonoBehaviour
{
    [SerializeField] string target = "Player";
    [SerializeField] LayerMask mask;
    [SerializeField] float activeDistance = 50f;

    [SerializeField] float fovHorizontal = 90f;
    [SerializeField] float fovVertical = 90f;
    [SerializeField] float viewDistance = 20f;
    [SerializeField] int rayCountHorizontal = 10;
    [SerializeField] int rayCountVertical = 10;

    float angleIncreaseH;
    float angleIncreaseV;

    PlayerMovement player;

    public Action<Vector3> targetDetected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        angleIncreaseH = fovHorizontal / rayCountHorizontal;
        angleIncreaseV = fovVertical / rayCountVertical;

        player = PlayerMovement.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new(0f, 0.1f, 0f));
        Vector3 origin = transform.position;
        if(Vector3.Distance(origin, player.transform.position) <= activeDistance)
        {
            for (float angleV = fovVertical / -2f; angleV <= fovVertical / 2; angleV += angleIncreaseV)
            {
                Color c = angleV <= 0f ? Color.red : Color.blue;
                for (float angleH = fovHorizontal / -2f; angleH <= fovHorizontal / 2; angleH += angleIncreaseH)
                {
                    //Solution de chatgpt pour empêcher de flip les rays verticaux selon la rotation du transform
                    Quaternion yaw = Quaternion.AngleAxis(angleH, transform.up);    // gauche-droite
                    Quaternion pitch = Quaternion.AngleAxis(angleV, transform.right); // haut-bas

                    //var dir = Quaternion.Euler(angleV, angleH, 0f) * transform.forward;
                    var dir = yaw * pitch * transform.forward;
                    if (Physics.Raycast(origin, dir, out RaycastHit hit, viewDistance, mask))
                    {
                        if (hit.collider.CompareTag(target))
                        {
                            Debug.DrawLine(origin, hit.point, Color.green); // touche la cible
                            //targetDetected(hit.transform.position);
                            return;
                        }
                        else
                        {
                            Debug.DrawLine(origin, hit.point, c); // rien touché
                        }
                    }
                    else
                    {
                        Debug.DrawRay(origin, dir * viewDistance, c); // rien touché
                    }

                }
            }
        }
    }
}
