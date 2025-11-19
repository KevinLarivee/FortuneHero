using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapDemo : MonoBehaviour {

    //This script goes on the SpikeTrap prefab;

    public Animator spikeTrapAnim; //Animator for the SpikeTrap;
    AudioSource audioSource;


    // Use this for initialization
    void Awake()
    {
        //get the Animator component from the trap;
        spikeTrapAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //start opening and closing the trap for demo purposes;
        StartCoroutine(OpenCloseTrap());
    }


    IEnumerator OpenCloseTrap()
    {
        //play open animation;
        audioSource.Play();

        spikeTrapAnim.SetTrigger("open");
        //wait 2 seconds;
        Debug.Log(audioSource);
        yield return new WaitForSeconds(2);
        //play close animation;
        spikeTrapAnim.SetTrigger("close");
        audioSource.Stop();
        //wait 2 seconds;
        yield return new WaitForSeconds(2);
        //Do it again;
        StartCoroutine(OpenCloseTrap());

    }
}