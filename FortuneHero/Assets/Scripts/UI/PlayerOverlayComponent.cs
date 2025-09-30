using UnityEngine;
using UnityEngine.UI;

public class PlayerOverlayComponent : MonoBehaviour
{
    [SerializeField] private Image shieldBar;
    [SerializeField] private float maxShield = 100f;
    private float currentShield;
    [SerializeField] private Image xpBar;
    [SerializeField] private TMPro.TextMeshProUGUI levelText;

    private int level = 1;
    private int currentXP = 0;
    private int xpToNextLevel = 100;
    [SerializeField] private TMPro.TextMeshProUGUI coinText;
    private int coins = 0;
    private HealthComponent health;



    void Start()
    {
        currentShield = maxShield;
        shieldBar.fillAmount = 1f;
        xpBar.fillAmount = 0f;
        levelText.text = "Niveau : " + level;
        health = GetComponent<HealthComponent>();
    }

    void Update()
    {
      /* health.Hit(10); */// -10 HP

        //// Test : utiliser le bouclier
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    UseShield(15); // -15 shield
        //}

        //// Test : recharger le bouclier
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    RechargeShield(10);
        //}
        //GainXP(30);

            AddCoins(5);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinText.text = "" + coins;
    }

    public void UseShield(float amount)
    {
        currentShield -= amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        shieldBar.fillAmount = currentShield / maxShield;
    }

    public void RechargeShield(float amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
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
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel += 50; 
        levelText.text = "Lvl " + level;
    }
}
