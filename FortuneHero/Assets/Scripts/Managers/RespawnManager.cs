using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    PlayerComponent pc;
    PlayerMovement pm;
    DissolveComponent dissolve;
    Vector3 respawnPoint = Vector3.zero;

    bool isRespawning = false;

    static RespawnManager instance;
    public static RespawnManager Instance { get { return instance; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        pc = PlayerComponent.Instance;
        pm = PlayerMovement.Instance;
        dissolve = pc.GetComponent<DissolveComponent>();
        InvokeRepeating(nameof(UpdateRespawn), 0.5f, 0.5f);
    }

    void UpdateRespawn()
    {
        //Vérifie les conditions seulement si la précédente est vraie.
        //Si toutes vraies, alors mettre à jour le RespawnPoint.
        if (pm.IsGrounded()
            && Physics.Raycast(pc.transform.position + pc.transform.up * 0.1f, pc.transform.up * -1, out RaycastHit hit, 0.3f, mask) 
            && hit.transform.CompareTag("Respawn"))
                SetRespawn(hit.point);
    }

    public void Respawn()
    {
        if(!isRespawning)
            StartCoroutine(RespawnAnimation());
        //Faire des dégâts au Joueur? ou ailleurs?
    }
    IEnumerator RespawnAnimation()
    {
        isRespawning = true;
        pc.PausePlayer(true);
        //Animation de despawn
        yield return dissolve.Dissolve();
        pc.transform.position = respawnPoint;
        //Animation de respawn
        yield return dissolve.Dissolve(true);
        pc.PausePlayer(false);
        isRespawning = false;
    }

    public void SetRespawn(Vector3 point)
    {
        respawnPoint = point;
    }
}
