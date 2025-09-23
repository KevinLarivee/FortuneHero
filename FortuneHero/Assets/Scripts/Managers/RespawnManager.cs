using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    CharacterController cc;
    Vector3 respawnPoint = Vector3.zero;

    static RespawnManager instance;
    public static RespawnManager Instance { get { return instance; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Nécessaire?
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        cc = PlayerMovement.Instance.GetComponent<CharacterController>();
        InvokeRepeating(nameof(UpdateRespawn), 0.5f, 0.5f);
    }

    void UpdateRespawn()
    {
        //Vérifie les conditions seulement si la précédente est vraie.
        //Si toutes vraies, alors mettre à jour le RespawnPoint.
        if (cc.isGrounded 
            && Physics.Raycast(cc.transform.position, cc.transform.TransformDirection(Vector3.down), out RaycastHit hit, 0.1f) 
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
        yield return new WaitForSeconds(1f);
        cc.transform.position = respawnPoint;
        //Animation de respawn
        yield return new WaitForSeconds(1f);
        cc.enabled = true;
    }

    public void SetRespawn(Vector3 point)
    {
        respawnPoint = point;
    }
}
