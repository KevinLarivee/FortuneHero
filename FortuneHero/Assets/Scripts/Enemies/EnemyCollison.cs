using UnityEngine;

public class EnemyCollison : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("RangedAtk"))
        {
            gameObject.GetComponent<EnemyDrops>().SpawnDrops();
            gameObject.SetActive(false);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MeleeAtk"))
        {
            gameObject.GetComponent<EnemyDrops>().SpawnDrops();
            gameObject.SetActive(false);
        }
    }
}
