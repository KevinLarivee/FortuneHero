using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamageCollision : MonoBehaviour
{
    [Header("Paramètres d'attaque")]
    [SerializeField] int damage = 1;
    [SerializeField] string target = "Player";
    //[SerializeField] StatusEffect statusEffect = StatusEffect.Knockback;

    //[Header("Knockback")]
    //[SerializeField] float knockbackForce = 10f;
    //[SerializeField] float knockbackDuration = 0.25f;
    //[Range(0f, 3f)][SerializeField] float verticalFactor = 1f;


    ParticleSystem particle;
    List<ParticleCollisionEvent> collisionEvents;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(target))
        {
            //faire degats
            other.GetComponent<HealthComponent>().Hit(damage);//statusEffect
            Vector3 sourcePos = other.GetComponent<Collider>().bounds.center;
            //appel knockback
            //PlayerMovement.Instance.KnockBack(sourcePos, knockbackForce, knockbackDuration, verticalFactor);
            Debug.Log($"{other.name} touché. Dégats: {damage}");

            //int numCollisionEvents = particle.GetCollisionEvents(other, collisionEvents);
            //ParticleCollisionEvent collision = collisionEvents[0];

            //if (Physics.Raycast(collision.intersection, -collision.normal, 1f, 69))
            //    RespawnManager.Instance.Respawn();

        }
    }
}
