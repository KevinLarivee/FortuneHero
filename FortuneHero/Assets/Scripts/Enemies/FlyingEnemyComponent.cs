using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrolComponent))]
public class FlyingEnemyComponent : EnemyComponent
{
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        patrol.move = Move;
    }

    //� cause du parent... � retravailler anyways
    protected override void Move(Transform newTarget)
    {
        //D�j� dans base
        //target = newTarget.position;
        base.Move(newTarget);
        Vector3 posToTarget = target - transform.position;

        //Utilise pas NavMesh, donc doit gerer la rotation 
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if(posToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        //D�j� dans base
        //if (posToTarget.sqrMagnitude <= stoppingDistance * stoppingDistance)
        //    newTarget = patrol.NextTarget();
        //animator.SetBool("isPatrolling", true);
        //animator.SetBool("isChasing", false);
    }

    protected override void ChasingMove()
    {
        Vector3 posToTarget = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        //Envoie un message dans la console si posToTarget est 0...
        if (posToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        base.ChasingMove();
    }

    public override void ToggleParalyze(float aoeDuration)
    {
        base.ToggleParalyze(aoeDuration);
        target = transform.position;
    }
}
