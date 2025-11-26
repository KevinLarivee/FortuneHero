using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapDemo : MonoBehaviour {

    //This script goes on the SpikeTrap prefab;

    Animator[] spikeTrapAnims; //Animator for the SpikeTrap;
    [SerializeField] AudioClip spikeTrapSFX;
    [SerializeField] float detectionRange = 20f;

    // Use this for initialization
    void Start()
    {
        //get the Animator component from the trap;
        spikeTrapAnims = GetComponentsInChildren<Animator>();
        //start opening and closing the trap for demo purposes;
        StartCoroutine(OpenCloseTrap());
    }
  

    IEnumerator OpenCloseTrap()
    {
        if ((transform.position - PlayerComponent.Instance.transform.position).sqrMagnitude > detectionRange * detectionRange)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(OpenCloseTrap());
        }
        else
        {
            SFXManager.Instance.PlaySFX(spikeTrapSFX, transform, PlayerComponent.Instance.SFXGroup);
            foreach (Animator anim in spikeTrapAnims)
            {
                anim.SetTrigger("open");
            }

            yield return new WaitForSeconds(2);

            foreach (Animator anim in spikeTrapAnims)
            {
                anim.SetTrigger("close");
            }

            yield return new WaitForSeconds(2);
            StartCoroutine(OpenCloseTrap());
        }
            
    }
}