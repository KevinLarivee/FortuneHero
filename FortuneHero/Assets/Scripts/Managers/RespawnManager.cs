using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    PlayerMovement pm;
    CharacterController cc;
    DissolveComponent dissolve;
    Vector3 respawnPoint = Vector3.zero;

    static RespawnManager instance;
    public static RespawnManager Instance { get { return instance; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        pm = PlayerMovement.Instance;
        cc = pm.GetComponent<CharacterController>();
        dissolve = pm.GetComponent<DissolveComponent>();
        InvokeRepeating(nameof(UpdateRespawn), 0.5f, 0.5f);
    }

    void UpdateRespawn()
    {
        //Vérifie les conditions seulement si la précédente est vraie.
        //Si toutes vraies, alors mettre à jour le RespawnPoint.
        if (pm.IsGrounded()
            && Physics.Raycast(pm.transform.position + pm.transform.up * 0.1f, pm.transform.up * -1, out RaycastHit hit, 0.3f, mask) 
            && hit.transform.CompareTag("Respawn"))
                SetRespawn(hit.point);
    }

    public void Respawn()
    {
        StartCoroutine(RespawnAnimation());
        //Faire des dégâts au Joueur? ou ailleurs?
    }
    IEnumerator RespawnAnimation()
    {

        cc.enabled = false;
        //Animation de despawn
        yield return dissolve.Dissolve();
        pm.transform.position = respawnPoint;
        //Animation de respawn
        yield return dissolve.Dissolve(true);
        cc.enabled = true;
    }

    public void SetRespawn(Vector3 point)
    {
        respawnPoint = point;
    }
}
