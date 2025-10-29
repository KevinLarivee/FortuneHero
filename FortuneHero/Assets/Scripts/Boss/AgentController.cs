using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    //[SerializeField] Transform target;
    Animator animator;
    NavMeshAgent agent;
    bool _traversingLink = false;
    bool _isJumping = false;
    OffMeshLinkData _currLink;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //agent.destination = target.position;
        agent.autoTraverseOffMeshLink = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isOnOffMeshLink)
        {
            if (!_traversingLink)
            {
                //This is done only once. The animation's progress will determine link traversal.
                animator.ResetTrigger("jump");
                animator.SetTrigger("jump");
                //cache current link
                _currLink = agent.currentOffMeshLinkData;
                //start traversing
                _traversingLink = true;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index (par défaut)
            if (stateInfo.IsName("JumpZombie"))
            {
                _isJumping = true;
                //lerp from link start to link end in time to animation
                var tlerp = stateInfo.normalizedTime;
                //straight line from startlink to endlink
                var newPos = Vector3.Lerp(_currLink.startPos, _currLink.endPos, tlerp);
                //add the 'hop'
                newPos.y += 10f * Mathf.Sin(Mathf.PI * tlerp);
                //Update transform position
                transform.position = newPos;
            }

            // when the animation is stopped, we've reached the other side. Don't use looping animations with this control setup
            else if(_isJumping)
            {
                //make sure the player is right on the end link
                transform.position = _currLink.endPos;
                //internal logic reset
                _traversingLink = false;
                //Tell unity we have traversed the link
                agent.CompleteOffMeshLink();
                //Resume normal navmesh behaviour
                agent.isStopped = false;
            }
        }
        else
        {
            _traversingLink = false;
        }
    }
}
