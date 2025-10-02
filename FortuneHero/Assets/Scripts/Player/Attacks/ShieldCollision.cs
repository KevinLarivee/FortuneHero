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
        var temp = other.GetComponent<IBlockable>();
        if (temp != null)
        {
            
            //int dmg = other.gameObject.GetComponent<EnemyComponent>().dmg;
            //healthComponent.Hit(dmg / defenceDmgReduce);
        }
    }
}
