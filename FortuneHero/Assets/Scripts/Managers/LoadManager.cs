using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    [SerializeField] float loadingTime = 1f;

    [SerializeField] FadeInOut fadeManager;

    bool isLoading = false;

    static LoadManager instance;
    public static LoadManager Instance { get { return instance; } }
    void Awake()
    {
        //Nécessaire?
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }
    public void Load(params string[] scenesToLoad)
    {
        StartCoroutine(LoadScene(scenesToLoad));
    }

    IEnumerator LoadScene(params string[] scenesToLoad)
    {
        yield return StartCoroutine(fadeManager.FadeIn());
        StartCoroutine(LoadingAnimation());
        AsyncOperation[] asyncOps = new AsyncOperation[scenesToLoad.Length];
        asyncOps[0] = SceneManager.LoadSceneAsync(scenesToLoad[0]);
        asyncOps[0].allowSceneActivation = false;
        for(int i = 1; i < scenesToLoad.Length; ++i)
            SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
        //Sugestion pour gérer le loading screen
        do
        {
            //float progress = asyncOps[0].progress;
            ////update le fill amount de notre loading bar
            //yield return new WaitWhile(() => asyncOps[0].progress - progress < 0.1);
            if(!isLoading)
                asyncOps[0].allowSceneActivation = true;
            yield return null;
        } while (asyncOps.Any(o => !o.isDone));
        yield return StartCoroutine(fadeManager.FadeOut());
    }

    //Idée, peut-être déplacer dans FadeInOut
    IEnumerator LoadingAnimation()
    {
        isLoading = true;
        yield return new WaitForSeconds(loadingTime);
        isLoading = false;
    }
}
