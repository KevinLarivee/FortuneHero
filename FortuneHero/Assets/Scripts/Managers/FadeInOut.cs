using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasRenderer))]
public class FadeInOut : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 3f;
    CanvasRenderer[] renderers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderers = GetComponentsInChildren<CanvasRenderer>() ?? GetComponents<CanvasRenderer>();
        //1f = opaque et 0f= invisible
        //renderer.SetAlpha(0f);
        //gameObject.SetActive(false);
        StartCoroutine(FadeOut());

    }
    public IEnumerator FadeIn()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha = alpha + fadeSpeed * Time.deltaTime;
            SetAlpha(alpha);
            yield return null;
        }
        gameObject.SetActive(true);

    }

    public IEnumerator FadeOut()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            Debug.Log(alpha);
            alpha = alpha - fadeSpeed * Time.deltaTime;
            SetAlpha(alpha);
            yield return null;
        }
        Debug.Log("FIN FADE OUT");

        gameObject.SetActive(false);
    }
    void SetAlpha(float alpha)
    {
        foreach (var renderer in renderers)
        {
            renderer.SetAlpha(alpha);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
