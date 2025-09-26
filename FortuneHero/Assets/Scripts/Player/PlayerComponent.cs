using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    [Header("Status Effect")]
    string statusEffect = "";
    int statusDuration = 0;
    int statusTickDmg = 0;

    [Header("Status")]
    [SerializeField] int currentCoins = 0;
    int currentXp = 0;
    int currentLevel = 0;
    int xpRequirement = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetXpAndCoins(int xpGain, int coinGain) //Mettre valeur negative pour perdre coins ou xp, pour get un des deux, mettre l'autre a 0
    {
        currentXp += xpGain;
        currentCoins += coinGain;
        //Faire autre logique: sound effects, Ui updates (?), etc.
    }

}
