using UnityEditor;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using System;
using Unity.VisualScripting;
using System.Collections;

public enum PatrolType { Loop, Reverse, Random, Chaos};
public class PatrolComponent : MonoBehaviour
{
    //Vector3?
    [SerializeField] Transform[] targets;
    [SerializeField, InfoBox("Will replace the NavMeshAgent stoppingDistance if has one")]
    float stoppingDistance = 0.1f;

    [SerializeField, Range(0f, 1f)] float lookAroundProbability = 0f;
    [SerializeField] float lookAroundSeconds = 1f;
    [SerializeField] float waitSecondsBetweenTargets = 0f;
    [SerializeField] bool stopAtGoal = false;

    [SerializeField, InfoBox("false if has NavMeshAgent")] bool doRotation = false;

    [SerializeField] PatrolType patrolType = PatrolType.Loop;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Loop)] float isLooping;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Reverse)] float waitSecondsAtEnds = 1f;

    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Random)] bool allowPreviousTarget = true;

    [Header("Warning : targets must be declared clockwise and create an area with no obstructions"), 
        SerializeField, ShowIf(nameof(patrolType), PatrolType.Chaos)] float minDistance = 1f;
    [SerializeField, ShowIf(nameof(patrolType), PatrolType.Chaos)] float maxDistance = 20f;

    bool isActive;

    NavMeshAgent agent;
    int currentTarget = 0;
    Vector3 destination;

    Action move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Si null, alors déplace manuellement
        if ((agent = GetComponent<NavMeshAgent>()) != null)
        {
            move = MoveAgent;
            doRotation = false;
            agent.stoppingDistance = stoppingDistance;
        }
        else
        {
            move = MovePlatform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            if (UnityEngine.Random.value <= lookAroundProbability)
            {
                StartCoroutine(Waiting(lookAroundSeconds));
            }
            else
            {
                if (Vector3.Distance(transform.position, destination) <= stoppingDistance)
                {
                    switch (patrolType)
                    {
                        case PatrolType.Loop:
                            currentTarget = (currentTarget + 1) % targets.Length;
                            destination = targets[currentTarget].position;
                            break;
                        case PatrolType.Reverse:
                            currentTarget = (currentTarget + 1 + targets.Length) % targets.Length;
                            break;
                        case PatrolType.Random:
                            destination = targets[UnityEngine.Random.Range(0, targets.Length)].position;
                            break;
                        case PatrolType.Chaos:
                            break;
                    }
                }
                move();
            }
        }
    }

    public void StopPatrol()
    {
        isActive = false;
    }

    void MoveAgent()
    {
        
    }

    void MovePlatform()
    {

    }

    IEnumerator Waiting(float seconds)
    {
        isActive = false;
        yield return new WaitForSeconds(seconds);
        isActive = true;
    }
}
