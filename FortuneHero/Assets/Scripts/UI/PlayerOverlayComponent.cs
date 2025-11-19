using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverlayComponent : MonoBehaviour
{
    [SerializeField]  Image shieldBar;
    [SerializeField] float maxShield = 10;
    private float currentShield;
    [SerializeField] Image xpBar;
    [SerializeField] TMPro.TextMeshProUGUI levelText;
    [SerializeField] TMPro.TextMeshProUGUI resultText; // ton texte UI pour victoire/défaite
    [SerializeField] Color victoryColor = Color.green;
    [SerializeField] Color defeatColor = Color.red;

    private int level = 1;
    private int currentXP = 0;
    private int xpToNextLevel = 100;
    [SerializeField]  TMPro.TextMeshProUGUI coinText;
    private int coins = 0;



    void Start()
    {
        currentShield = maxShield;
        shieldBar.fillAmount = 1f;
        xpBar.fillAmount = 0f;

        level = PlayerPrefs.GetInt("Level", 1);
        currentXP = PlayerPrefs.GetInt("XP", 0);
        coins = PlayerPrefs.GetInt("coins", 0);

        levelText.text = "Niveau: " + level;
        coinText.text = "" + coins;
        xpBar.fillAmount = (float)currentXP / RequiredXpForLevel(level);
    }

    void Update()
    {
        int newLevel = PlayerPrefs.GetInt("Level", level);
        int newXP = PlayerPrefs.GetInt("XP", currentXP);
        int newCoins = PlayerPrefs.GetInt("coins", coins);

        if (newLevel != level || newXP != currentXP || newCoins != coins)
        {
            level = newLevel;
            currentXP = newXP;
            coins = newCoins;

            levelText.text = "Niveau: " + level;
            coinText.text = "" + coins;
            xpBar.fillAmount = (float)currentXP / RequiredXpForLevel(level);
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinText.text = "" + coins;
        PlayerPrefs.SetInt("coins", coins);

    }

    public void UseShield(float amount)
    {
        currentShield = amount;
        

        shieldBar.fillAmount = currentShield / maxShield;
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
        xpBar.fillAmount = (float)currentXP / xpToNextLevel;
        PlayerPrefs.SetInt("XP", currentXP);

    }

    void LevelUp()
    {
        level++;
        xpToNextLevel += 50; 
        levelText.text = "Niveau: " + level;
        PlayerPrefs.SetInt("Level", level);

    }

    private int RequiredXpForLevel(int level)
    {
        level = Mathf.Max(1, level);
        float need = 100f * Mathf.Pow(1.1f, level - 1);
        return Mathf.CeilToInt(need);
    }
}
