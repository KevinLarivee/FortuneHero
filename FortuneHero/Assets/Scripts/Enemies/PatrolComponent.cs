using UnityEditor;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using System;
using Unity.VisualScripting;
using System.Collections;

public enum PatrolType { Loop, Reverse, Random, /*Chaos*/};
public class PatrolComponent : MonoBehaviour
{
    //Vector3?
    [SerializeField] Transform[] targets;

    [SerializeField, Range(0f, 1f)] float lookAroundProbability = 0f;
    [SerializeField] float lookAroundFrequency = 1f;
    [SerializeField] float lookAroundSeconds = 1f;
    [SerializeField] float waitSecondsBetweenTargets = 0f;

    [SerializeField] PatrolType patrolType = PatrolType.Loop;

    //[SerializeField, ShowIf(nameof(patrolType), PatrolType.Loop)] float isLooping;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Reverse)] float waitSecondsAtEnds = 1f;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Random)] bool allowPreviousTarget = true;

    //[Header("Warning : targets must be declared clockwise and create an area with no obstructions"), 
    //    SerializeField, ShowIf(nameof(patrolType), PatrolType.Chaos)] float minDistance = 1f;
    //[SerializeField, ShowIf(nameof(patrolType), PatrolType.Chaos)] float maxDistance = 20f;

    int currentTarget = 0;

    //Défini par les autres scripts
    public Action<Transform> move;

    //Modifié par d'autres scripts
    public bool isActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LookAround());
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            move(targets[currentTarget]);
        }
    }

    //void MoveAgent()
    //{
    //    if (Vector3.Distance(transform.position, destination) <= stoppingDistance)
    //    {

    //    }
    //}

    //void MovePlatform()
    //{
    //    float elapsedPercentage = elapsedTime / timeToTarget;
    //    var _previousTarget = targets[(currentTarget - 1 + targets.Length) % targets.Length];
    //    elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
    //    transform.position = Vector3.Lerp(_previousTarget.position, targets[currentTarget].position, elapsedPercentage);
    //    transform.rotation = Quaternion.Lerp(_previousTarget.rotation, targets[currentTarget].rotation, elapsedPercentage);
    //    if (elapsedPercentage >= 1)
    //    {
    //        elapsedTime = 0f;
    //        float distanceToWaypoint = Vector3.Distance(_previousTarget.position, targets[currentTarget].position);
    //        timeToTarget = distanceToWaypoint / speed;
    //        NextTarget();
    //    }
    //}

    public Transform NextTarget()
    {
        switch (patrolType)
        {
            case PatrolType.Loop:
                currentTarget = (currentTarget + 1) % targets.Length;
                break;
            case PatrolType.Reverse:
                currentTarget = (currentTarget + 1 + targets.Length) % targets.Length;
                break;
            case PatrolType.Random:
                currentTarget = UnityEngine.Random.Range(0, targets.Length);
                break;
                //case PatrolType.Chaos:
                //    break;
        }
        StartCoroutine(Waiting(waitSecondsBetweenTargets));
        return targets[currentTarget];
    }
    IEnumerator Waiting(float seconds)
    {
        isActive = false;
        yield return new WaitForSeconds(seconds);
        isActive = true;
    }
    IEnumerator LookAround()
    {
        do
        {
            if (UnityEngine.Random.value <= lookAroundProbability)
            {
                StartCoroutine(Waiting(lookAroundSeconds));
                yield return new WaitForSeconds(lookAroundSeconds);
            }
            yield return new WaitForSeconds(lookAroundFrequency);
        } while (lookAroundProbability > 0f);
    }
}
