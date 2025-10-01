using UnityEngine;

public class MovingSpikeWall : MonoBehaviour
{
    private PatrolComponent _patrol;
    [SerializeField]  float _speed = 1f;

    private float _timeToTarget;
    private float _elapsedTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _patrol = GetComponent<PatrolComponent>();
        _patrol.move = Move;
        _patrol.isActive = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Move(Transform destination)
    {
        // avancer vers la destination
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination.position,
            _speed * Time.deltaTime
        );

        // arrivé à la destination demander la prochaine cible
        if (Vector3.Distance(transform.position, destination.position) < 0.5f)
        {
            destination = _patrol.NextTarget();
        }
    }

    


}
