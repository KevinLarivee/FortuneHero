using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    [SerializeField] float loadingTime = 1f;

    FadeInOut fadeManager;

    bool isLoading = false;

    static LoadManager instance;
    public static LoadManager Instance { get { return instance; } }

    [Header("Réglages du slot")]
    [Header("Reels (RawImage) – utilisés pour l’animation ET l’icône finale")]
    [SerializeField] RawImage icon1;
    [SerializeField] RawImage icon2;
    [SerializeField] RawImage icon3;

    [Header("Textures des symboles (pour le RÉSULTAT FINAL)")]
    [SerializeField] Texture2D textureCerise;
    [SerializeField] Texture2D textureCloche;
    [SerializeField] Texture2D textureBAR;
    [SerializeField] Texture2D textureSept;

    [Header("Autres paramètres")]
    [SerializeField] Texture2D stripTexture;
    public float cyclesPerSecond = 1f;
    public int symbolsPerStrip = 5;

    private RawImage[] reels;
    void Awake()
    {
        //Nécessaire?
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
        fadeManager = GetComponentInChildren<FadeInOut>();
        DontDestroyOnLoad(gameObject);
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
        if (isLoading) yield break;
        isLoading = true;

        RawImage[] reels = { icon1, icon2, icon3 };

        // Durées de spin différentes pour chaque rouleau
        float spinDuration1 = Random.Range(2f, 3f);
        float spinDuration2 = spinDuration1 + Random.Range(0.5f, 1f);
        float spinDuration3 = spinDuration2 + Random.Range(0.5f, 1f);
        float[] durations = { spinDuration1, spinDuration2, spinDuration3 };

        // Symboles finaux possibles
        Texture2D[] finalTextures = { textureCerise, textureCloche, textureBAR, textureSept };

        bool[] done = { false, false, false };

        // Lancer les rouleaux
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].gameObject.SetActive(true);
            StartCoroutine(CoSpin(reels[i], durations[i], finalTextures, i, done));
        }

        // Attendre que tous les rouleaux aient fini
        yield return new WaitUntil(() => done[0] && done[1] && done[2]);

        // Petit délai avant de passer à la scène
        yield return new WaitForSeconds(0.5f);

        isLoading = false;
    }

    // Coroutine pour chaque rouleau
    IEnumerator CoSpin(RawImage reel, float duration, Texture2D[] finalTextures, int index, bool[] done)
    {
        float startTime = Time.time;
        float cellHeight = 1f / symbolsPerStrip; // hauteur d'un symbole dans le strip

        while (Time.time - startTime < duration)
        {
            float offsetY = Mathf.Repeat(Time.time * cyclesPerSecond, 1f - cellHeight);
            reel.uvRect = new Rect(0f, offsetY, 1f, cellHeight);
            reel.texture = stripTexture;
            yield return null;
        }

        // Choisir un symbole final aléatoire
        reel.texture = finalTextures[Random.Range(0, finalTextures.Length)];
        reel.uvRect = new Rect(0f, 0f, 1f, 1f);

        done[index] = true;
    }
}
