using System.Collections;
using System.Linq;
using UnityEngine;

public class MovingSpikeWall : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float syncTime = 8f;
    [SerializeField] float extendDistance = 3f;

    //Les wall sont bizarre, c'est right qui est "forward"
    Vector3 initialPos;
    Vector3 targetPos;

    float elapsedTime = 0f;
    bool extending = true;
    public bool isActive = true;

    Collider spikes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPos = transform.position;
        targetPos = initialPos + transform.right * extendDistance;

        spikes = GetComponentInChildren<Collider>();

        StartCoroutine(AlternateSync());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator AlternateSync()
    {
        while (isActive)
        {
            elapsedTime = 0f;

            while (extending && Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            while (elapsedTime < syncTime || Vector3.Distance(transform.position, initialPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPos, speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            extending = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            extending = false;
    }

}
