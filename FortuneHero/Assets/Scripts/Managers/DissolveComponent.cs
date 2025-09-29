using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

//https://youtu.be/we406Hc_WrM?si=WtW3l9ibP3skfdUZ

public class DissolveComponent : MonoBehaviour
{
    [SerializeField] float dissolveRate = 0.0125f;
    [SerializeField] float refreshRate = 0.025f;

    Material[] materials;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer[] meshRenderers;
        meshRenderers = GetComponentsInChildren<Renderer>();
        if (meshRenderers != null)
        {
            List<Material> temp = new List<Material>();
            foreach (Renderer renderer in meshRenderers)
            {
                temp.AddRange(renderer.materials);
            }
            materials = temp.ToArray();
        }
            
    }

    //public void StartDissolve()
    //{
    //    StartCoroutine(Dissolve());
    //}

    //public void StartAppear()
    //{
    //    StartCoroutine(Dissolve(true));
    //}

    public IEnumerator Dissolve(bool reverse = false)
    {
        Debug.Log(reverse);
        if(materials.Length > 0)
        {
            float rate = reverse ? -dissolveRate : dissolveRate;
            float counter = reverse ? 1f : 0f;
            Predicate<float> condition = reverse ? new(x => x > 0f ) : new(x => x < 1f);
            while (condition(materials[0].GetFloat("_DissolveAmount")))
            {
                counter += rate;
                for(int i = 0; i < materials.Length; ++i)
                {
                    materials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
