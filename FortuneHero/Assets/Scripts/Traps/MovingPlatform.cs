using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolComponent))]
public class MovingPlatform : MonoBehaviour
{
    private PatrolComponent _patrol;

    [SerializeField]
    private float _speed = 1f;

    [SerializeField]
    private float delayToStart = 0f;

    [SerializeField]
    private Transform _previousTarget;

    private float _timeToTarget;
    private float _elapsedTime;

    void Start()
    {
        _patrol = GetComponent<PatrolComponent>();
        _patrol.move = Move;
        _patrol.isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    private void Move(Transform destination)
    {
        if (delayToStart > 0)
        {
            delayToStart -= Time.deltaTime;
            return;
        }
        _elapsedTime += Time.deltaTime;
        //pas optimisé, à revoir
        float distanceToTarget = Vector3.Distance(_previousTarget.position, destination.position);
        _timeToTarget = distanceToTarget / _speed;

        float elapsedPercentage = _elapsedTime / _timeToTarget;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousTarget.position, destination.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(_previousTarget.rotation, destination.rotation, elapsedPercentage);
        if (elapsedPercentage >= 1)
        {
            _elapsedTime = 0f;
            _previousTarget = destination;
            destination = _patrol.NextTarget();
            //float distanceToTarget = Vector3.Distance(_previousTarget.position, destination.position);
            //_timeToTarget = distanceToTarget / _speed;
        }
    }
}