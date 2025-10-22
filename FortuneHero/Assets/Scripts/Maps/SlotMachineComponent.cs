using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SlotMachineComponent : MonoBehaviour, IInteractable
{
    public float exitTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

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

    [Header("Slot Settings")]
    [SerializeField] int currentBet = 10;
    [SerializeField] int minBet = 5;
    [SerializeField] int maxBet = 50;

    [Header("Slot Icons (RawImage)")]
    [SerializeField] RawImage icon1;
    [SerializeField] RawImage icon2;
    [SerializeField] RawImage icon3;

    [Header("Textures des symboles")]
    [SerializeField] Texture2D textureCerise;
    [SerializeField] Texture2D textureCloche;
    [SerializeField] Texture2D textureBAR;
    [SerializeField] Texture2D textureSept;

    private enum Symbol { Cerise, Cloche, BAR, Sept }

    private int weightCerise = 60;  // symbole commun — gain faible
    private int weightCloche = 35;  // un peu plus fréquent
    private int weightBAR = 25;     // inchangé
    private int weightSept = 15;    // jackpot un peu plus accessible
    private int totalWeight;


    void Start()
    {
        totalWeight = weightCerise + weightCloche + weightBAR + weightSept;

        if (!PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", 100);

        icon1.gameObject.SetActive(false);
        icon2.gameObject.SetActive(false);
        icon3.gameObject.SetActive(false);
        UpdateBalanceText();
    }

    public void Enter() { }

    public void Exit() { }

    public void Interact()
    {
        bool isOpen = panelSlot.activeSelf;
        PlayerComponent.Instance.PausePlayer(isOpen);

        if (isOpen)
        {
            panelSlot.SetActive(false);
            panelPlayer.SetActive(true);
            resultText.text = "";
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            panelSlot.SetActive(true);
            panelPlayer.SetActive(false);

            btnIncreaseBet.onClick.RemoveAllListeners();
            btnDecreaseBet.onClick.RemoveAllListeners();
            btnSpin.onClick.RemoveAllListeners();

            btnIncreaseBet.onClick.AddListener(IncreaseBet);
            btnDecreaseBet.onClick.AddListener(DecreaseBet);
            btnSpin.onClick.AddListener(() => StartCoroutine(SpinRoutine()));

            UpdateBetText();
            UpdateBalanceText();
            resultText.text = "";

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
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
        betText.text = $"Mise : {currentBet} coins";
    }

    void UpdateBalanceText()
    {
        int coins = PlayerPrefs.GetInt("coins");
        balanceText.text = $"Balance : {coins} coins";
    }

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

    IEnumerator SpinRoutine()
    {
        int coins = PlayerPrefs.GetInt("coins");
        if (coins < currentBet)
        {
            resultText.text = "Pas assez de coins !";
            yield break;
        }

        // Retirer la mise
        coins -= currentBet;
        PlayerPrefs.SetInt("coins", coins);
        UpdateBalanceText();

        resultText.text = "Spin en cours...";

        // Désactiver et réinitialiser les icônes
        RawImage[] icons = { icon1, icon2, icon3 };
        foreach (var icon in icons)
        {
            icon.texture = null;
            icon.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        Symbol[] reels = new Symbol[3];

        for (int i = 0; i < 3; i++)
        {
            reels[i] = RandomSymbol();
            Texture2D symbolTexture = GetTextureForSymbol(reels[i]);
            icons[i].texture = symbolTexture;
            icons[i].gameObject.SetActive(true); // Activer seulement quand la texture est prête
            Debug.Log($"Rouleau {i + 1} : {reels[i]}");
            yield return new WaitForSecondsRealtime(0.3f);
        }

        string resultLine = $"{reels[0]} | {reels[1]} | {reels[2]}";

        if (reels[0] == reels[1] && reels[1] == reels[2])
        {
            int multiplier = 0;
            switch (reels[0])
            {
                case Symbol.Cerise: multiplier = 3; break;
                case Symbol.Cloche: multiplier = 7; break;
                case Symbol.BAR: multiplier = 10; break;
                case Symbol.Sept: multiplier = 20; break;
            }

            int gain = currentBet * multiplier;
            coins += gain;
            PlayerPrefs.SetInt("coins", coins);
            resultText.text = $"GAGNÉ {gain} coins!";
        }
        else
        {
            resultText.text = $"Perdu.";
        }

        UpdateBalanceText();
    }

}
