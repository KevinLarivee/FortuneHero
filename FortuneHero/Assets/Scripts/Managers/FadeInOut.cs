using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasRenderer))]
public class FadeInOut : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 3f;
    CanvasRenderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<CanvasRenderer>();
        //1f = opaque et 0f= invisible
        //renderer.SetAlpha(0f);
        //gameObject.SetActive(false);
        StartCoroutine(FadeOut());

    }
    public IEnumerator FadeIn()
    {
        float alpha = renderer.GetAlpha();
        while (alpha < 1f)
        {
            alpha = alpha + fadeSpeed * Time.deltaTime;
            renderer.SetAlpha(alpha);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float alpha = renderer.GetAlpha();
        while (alpha > 0f)
        {

            alpha = alpha - fadeSpeed * Time.deltaTime;
            renderer.SetAlpha(alpha);
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}

