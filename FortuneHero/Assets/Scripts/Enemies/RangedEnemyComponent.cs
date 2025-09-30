using System.Net;
using UnityEngine;

public class RangedEnemyComponent : MonoBehaviour
{
    Animator animator;

    [SerializeField] GameObject player;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float stopDistance = 10f;
    [SerializeField] GameObject Projectile;
    [SerializeField] GameObject exitPoint;

    float timeUntilPatrol = 10f;
    float timeUntilPatrolTimer = 0f;
    float rotationSpeed = 5f;

    bool canAttack = false;
    [SerializeField] bool isDetecting = false; //Placeholder

    Vector3 target;


    void Start()
    {
        animator = GetComponent<Animator>();
       
    }

    void Update()
    {
        if (isDetecting)
        {
            PlayerDetected();
            if (timeUntilPatrolTimer > 0) //si il detecte de nouveau apres avoir parti le timer = reset
                timeUntilPatrolTimer = 0;
        }
        else //Detecte pas le joueur
        {
            timeUntilPatrolTimer += Time.deltaTime; //start le timer 
            if (timeUntilPatrolTimer >= timeUntilPatrol) //si le timer atteint le max:
            {
                //Enable le patrol
                //animator.SetBool("isPatrolling", true);
            }
        }
    }

    public void PlayerDetected()
    {
        //animator.SetBool("isPatrolling", false);
        target = player.transform.position; //La target est la Pos du joueur recu quand l'enemy detecte le joueur
        Vector3 posToTarget = target - transform.position;
        canAttack = posToTarget.sqrMagnitude <= stopDistance * stopDistance;

        if (canAttack)
        {
            Attack();
            //animator.SetBool("isChasing", false);
        }
        else //Disable just le mouvement pour attaquer
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            //animator.SetBool("isChasing", true);
        }
        Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Attack()
    {
        //animator.SetTrigger("Attack");
        Instantiate(Projectile, exitPoint.transform.position, gameObject.transform.rotation);

    }
}
