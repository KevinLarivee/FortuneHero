using UnityEngine;

public class ShieldCollision : MonoBehaviour
{
    int defenceDmgReduce = 2;
    HealthComponent healthComponent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthComponent = GetComponentInParent<HealthComponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("hitfr");
            int dmg = other.gameObject.GetComponent<EnemyComponent>().dmg;
            healthComponent.Hit(dmg / defenceDmgReduce);
        }
    }
}
