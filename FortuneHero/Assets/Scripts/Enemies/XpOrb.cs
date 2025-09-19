using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class XpOrb : MonoBehaviour
{
    [Min(1)] public int amount = 5;
    public float attractRadius = 4f;
    public float moveSpeed = 6f;

    Transform target;

    void Reset()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.4f;
    }

    void Update()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            if (Vector3.Distance(transform.position, player.transform.position) <= attractRadius)
                target = player.transform;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Ajoute l’XP ici (remplace par ton propre gestionnaire d’XP si tu en as un)
       // var xpHolder = other.GetComponent<PlayerXpHolder>();
        //if (xpHolder != null) xpHolder.AddXp(amount);

        gameObject.SetActive(false);
    }
}
