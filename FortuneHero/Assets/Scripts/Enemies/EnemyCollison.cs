using UnityEngine;

public class EnemyCollison : MonoBehaviour
{
    EnemyDrops drops;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drops = GetComponent<EnemyDrops>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("RangedAtk"))
        {
            drops.SpawnDrops();
            gameObject.SetActive(false);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MeleeAtk"))
        {
            drops.SpawnDrops();
            gameObject.SetActive(false);
        }
    }
}
