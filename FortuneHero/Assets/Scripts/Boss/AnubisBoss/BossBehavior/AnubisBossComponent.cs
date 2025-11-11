using UnityEngine;

public class AnubisBossComponent : MonoBehaviour
{
    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform exitPoint;
    Transform target;
    HealthComponent healthComponent;
    public int dmg = 10;
    public int collisionDmg = 5;
    float bufferCd = 0.2f;
    float bufferTimer = 0;

    void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.onDeath += Death;
        bufferTimer = bufferCd;
        target = PlayerComponent.Instance.transform;
    }
    void Update()
    {
        if(bufferTimer < bufferCd)
        {
            bufferTimer += Time.deltaTime;
        }
    }

    public void RangedAttack()
    {
        var rotation = Quaternion.LookRotation(target.position - exitPoint.position);
        Instantiate(projectilePrefab, exitPoint.position, rotation);
    }

    private void Death()
    {
        Destroy(gameObject);
    }
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthComponent>().Hit(collisionDmg);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (bufferTimer >= bufferCd && other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HealthComponent>().Hit(dmg);
            bufferTimer = 0;
        }
    }
}
