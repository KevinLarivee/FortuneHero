using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineComponent : MonoBehaviour, IInteractable
{
    public float exitTime { get => throw new System.NotImplementedException(); set => new System.NotImplementedException(); }

    [Header("UI Panels")]
    [SerializeField] GameObject panelSlot;
    [SerializeField] GameObject panelPlayer;

    [Header("UI Elements")]
    [SerializeField] Button btnIncreaseBet;
    [SerializeField] Button btnDecreaseBet;
    [SerializeField] Button btnSpin;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI betText;
    [SerializeField] TextMeshProUGUI balanceText;

    [Header("Cinemachine")]
    [SerializeField] CinemachineCamera mainVcam;   // caméra de jeu par défaut
    [SerializeField] CinemachineCamera slotVcam;   // caméra de la slot machine
    [SerializeField] int mainPriority = 10;        // priorité par défaut de la caméra principale
    [SerializeField] int slotInactivePriority = 0; // priorité quand la caméra slot est idle
    [SerializeField] int slotActivePriority = 20;  // priorité quand on veut focus la slot

    private CinemachineBrain brain;
    private Coroutine blendWaitCo;
    private bool pendingOpen; // vrai quand on attend d’ouvrir le menu après le blend

    [Header("Slot Settings")]
    [SerializeField] int currentBet = 10;
    [SerializeField] int minBet = 5;
    [SerializeField] int maxBet = 50;

    [Header("Reels (RawImage) – utilisés pour l’animation ET l’icône finale")]
    [SerializeField] RawImage icon1;
    [SerializeField] RawImage icon2;
    [SerializeField] RawImage icon3;

    [Header("Textures des symboles (pour le RÉSULTAT FINAL)")]
    [SerializeField] Texture2D textureCerise;
    [SerializeField] Texture2D textureCloche;
    [SerializeField] Texture2D textureBAR;
    [SerializeField] Texture2D textureSept;

    [Header("Bande verticale pour l’ANIMATION (ex: image 7/BAR/Cloche/Cerise)")]
    [SerializeField] Texture2D stripTexture;   // image verticale des 4 symboles empilés
    [SerializeField] int symbolsPerStrip = 4;  // 7, BAR, Cloche, Cerise (de haut en bas)
    [SerializeField] bool invertUV = true;     // bascule si l’ordre paraît inversé

    [Header("Animation du scroll (vitesse constante, sans ralentir)")]
    [SerializeField] float spinDurationMin = 1.2f;
    [SerializeField] float spinDurationMax = 1.8f;
    [SerializeField] float cyclesPerSecond = 3.0f; // nombre de tours par seconde (UV)

    private enum Symbol { Cerise, Cloche, BAR, Sept }

    private int weightCerise = 60;
    private int weightCloche = 35;
    private int weightBAR = 25;
    private int weightSept = 15;
    private int totalWeight;

    private bool isSpinning = false;

    void Awake()
    {
        // Récupère le CinemachineBrain sur la caméra principale (Camera.main)
        if (Camera.main != null)
            brain = Camera.main.GetComponent<CinemachineBrain>();

        // Assure des priorités connues au démarrage
        if (mainVcam != null) mainVcam.Priority = mainPriority;
        if (slotVcam != null) slotVcam.Priority = slotInactivePriority;
    }

    void Start()
    {
        totalWeight = weightCerise + weightCloche + weightBAR + weightSept;

        if (!PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", 100);

        // Reste invisibles tant qu’on n’a pas de texture posée
        icon1.gameObject.SetActive(false);
        icon2.gameObject.SetActive(false);
        icon3.gameObject.SetActive(false);

        UpdateBalanceText();

        // Panels init
        panelSlot.SetActive(false);
        panelPlayer.SetActive(true);
    }

    public void Enter() { }
    public void Exit() { }

    public void Interact()
    {
        bool isOpen = panelSlot.activeSelf;

        if (isOpen)
        {
            // FERMETURE : remet l’état de jeu normal et revient à la caméra principale
            CloseSlotMenuAndReturnCamera();
        }
        else
        {
            // OUVERTURE : focus caméra slot, attend la fin du blend, puis ouvre le menu
            OpenWithCameraBlend();
        }
    }

    private void OpenWithCameraBlend()
    {
        // Priorités de caméras
        if (mainVcam != null) mainVcam.Priority = mainPriority;
        if (slotVcam != null) slotVcam.Priority = slotActivePriority;

        // IMPORTANT : NE PAS pauser le player ici, on veut que la même touche d'interaction fonctionne pour refermer
        pendingOpen = true;

        if (blendWaitCo != null) StopCoroutine(blendWaitCo);
        blendWaitCo = StartCoroutine(WaitForCameraBlendThenOpenMenu());
    }

    private IEnumerator WaitForCameraBlendThenOpenMenu()
    {
        // Sécurité : si pas de brain (ou Cinemachine pas présent), ouvre direct.
        if (brain == null)
        {
            ForceOpenSlotMenuNow();
            yield break;
        }

        yield return null; // laisser démarrer le blend

        while (brain.IsBlending) yield return null;
        yield return null; // petite frame de sécurité

        if (pendingOpen)
        {
            ForceOpenSlotMenuNow();
        }
    }

    private void ForceOpenSlotMenuNow()
    {
        pendingOpen = false;

        panelSlot.SetActive(true);
        panelPlayer.SetActive(false);

        // Brancher les boutons à chaque ouverture
        btnIncreaseBet.onClick.RemoveAllListeners();
        btnDecreaseBet.onClick.RemoveAllListeners();
        btnSpin.onClick.RemoveAllListeners();

        btnIncreaseBet.onClick.AddListener(IncreaseBet);
        btnDecreaseBet.onClick.AddListener(DecreaseBet);
        btnSpin.onClick.AddListener(() => { if (!isSpinning) StartCoroutine(SpinRoutine()); });

        UpdateBetText();
        UpdateBalanceText();
        resultText.text = "";

        // Gel du jeu visuel, mais on laisse le script Player lire l’input (pour Interact)
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseSlotMenuAndReturnCamera()
    {
        // Dé-gel et fermeture
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        panelSlot.SetActive(false);
        panelPlayer.SetActive(true);
        resultText.text = "";

        // Stop attente d’ouverture si en cours
        pendingOpen = false;
        if (blendWaitCo != null)
        {
            StopCoroutine(blendWaitCo);
            blendWaitCo = null;
        }

        // Redonne la priorité à la caméra principale et rebaisse celle de la slot
        if (mainVcam != null) mainVcam.Priority = mainPriority;
        if (slotVcam != null) slotVcam.Priority = slotInactivePriority;
    }

    void IncreaseBet()
    {
        if (currentBet < maxBet)
        {
            currentBet += 5;
            UpdateBetText();
        }
    }

    void DecreaseBet()
    {
        if (currentBet > minBet)
        {
            currentBet -= 5;
            UpdateBetText();
        }
    }

    void UpdateBetText()
    {
        betText.text = $"{currentBet}";
    }

    void UpdateBalanceText()
    {
        int coins = PlayerPrefs.GetInt("coins");
        balanceText.text = $"Balance : {coins} coins";
    }

    // --- tirage pondéré ---
    Symbol RandomSymbol()
    {
        int value = Random.Range(1, totalWeight + 1);
        if (value <= weightCerise) return Symbol.Cerise;
        else if (value <= weightCerise + weightCloche) return Symbol.Cloche;
        else if (value <= weightCerise + weightCloche + weightBAR) return Symbol.BAR;
        else return Symbol.Sept;
    }

    Texture2D GetTextureForSymbol(Symbol symbol)
    {
        switch (symbol)
        {
            case Symbol.Cerise: return textureCerise;
            case Symbol.Cloche: return textureCloche;
            case Symbol.BAR: return textureBAR;
            case Symbol.Sept: return textureSept;
            default: return null;
        }
    }

    // --- Animation à vitesse constante SANS ralentir et POP instantané de l’icône finale ---
    IEnumerator AnimateReelConstant(RawImage reel, float duration, Texture2D finalIconTex, Symbol finalSymbol)
    {
        float cell = 1f / Mathf.Max(1, symbolsPerStrip);
        reel.texture = stripTexture;
        reel.uvRect = new Rect(0f, Random.value * (1f - cell), 1f, cell);
        reel.gameObject.SetActive(true);

        float end = Time.unscaledTime + duration;
        while (Time.unscaledTime < end)
        {
            Rect r = reel.uvRect;
            float advance = cyclesPerSecond * Time.unscaledDeltaTime;
            r.y = Mathf.Repeat(r.y + advance, 1f - cell);
            reel.uvRect = r;
            yield return null;
        }

        reel.texture = finalIconTex;
        reel.uvRect = new Rect(0, 0, 1, 1);
        reel.gameObject.SetActive(true);
    }

    IEnumerator SpinRoutine()
    {
        if (isSpinning) yield break;

        int coins = PlayerPrefs.GetInt("coins");
        if (coins < currentBet)
        {
            resultText.text = "Pas assez de coins !";
            yield break;
        }

        isSpinning = true;
        resultText.text = "Spin en cours...";

        coins -= currentBet;
        PlayerPrefs.SetInt("coins", coins);
        UpdateBalanceText();

        Symbol s1 = RandomSymbol();
        Symbol s2 = RandomSymbol();
        Symbol s3 = RandomSymbol();

        Texture2D t1 = GetTextureForSymbol(s1);
        Texture2D t2 = GetTextureForSymbol(s2);
        Texture2D t3 = GetTextureForSymbol(s3);

        float d1 = 1f;
        float d2 = 2f;
        float d3 = 3f;

        bool done1 = false, done2 = false, done3 = false;

        StartCoroutine(CoAnim(icon1, d1, t1, s1, () => done1 = true));
        StartCoroutine(CoAnim(icon2, d2, t2, s2, () => done2 = true));
        StartCoroutine(CoAnim(icon3, d3, t3, s3, () => done3 = true));

        yield return new WaitUntil(() => done1 && done2 && done3);

        if (s1 == s2 && s2 == s3)
        {
            int mult = 0;
            switch (s1)
            {
                case Symbol.Cerise: mult = 3; break;
                case Symbol.Cloche: mult = 7; break;
                case Symbol.BAR: mult = 10; break;
                case Symbol.Sept: mult = 20; break;
            }
            int gain = currentBet * mult;
            coins += gain;
            PlayerPrefs.SetInt("coins", coins);
            resultText.text = $"GAGNÉ {gain} COINS!";
        }
        else
        {
            resultText.text = "Perdu.";
        }

        UpdateBalanceText();
        isSpinning = false;
    }

    IEnumerator CoAnim(RawImage reel, float duration, Texture2D finalTex, Symbol finalSymbol, System.Action onDone)
    {
        if (stripTexture == null)
        {
            yield return new WaitForSecondsRealtime(duration);
            reel.texture = finalTex;
            reel.uvRect = new Rect(0, 0, 1, 1);
            reel.gameObject.SetActive(true);
            onDone?.Invoke();
            yield break;
        }

        yield return AnimateReelConstant(reel, duration, finalTex, finalSymbol);
        onDone?.Invoke();
    }
}
