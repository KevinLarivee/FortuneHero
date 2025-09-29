using UnityEngine;

public class MeleeAttackCollision : MonoBehaviour
{
    [SerializeField] GameObject player;
    float meleeDmg;
    
    private void Start()
    {

    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("You have attacked the enemy !!!");
    //        other.gameObject.GetComponentInParent<HealthComponent>().Hit(player.GetComponent<PlayerActions>().meleeAtkDmg);
    //        //Pas getComponent dans start pour que si ya un upgrade au dmg, le nb de dmg s'update
    //    }
    //}
}
