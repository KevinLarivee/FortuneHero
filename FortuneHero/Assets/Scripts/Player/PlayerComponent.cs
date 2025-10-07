using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    static PlayerComponent instance;
    public static PlayerComponent Instance { get { return instance; } }

    [Header("Status Effect")]
    string statusEffect = "";
    int statusDuration = 0;
    int statusTickDmg = 0;

    [Header("Status")]
    [SerializeField] int currentCoins = 0;
    int currentXp = 0;
    int currentLevel = 0;
    int xpRequirement = 100;

    PlayerMovement playerM;
    PlayerActions playerA;
    PlayerInteractions playerI;

    InputSystem_Actions.PlayerActions actions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        playerM = GetComponent<PlayerMovement>();
        playerA = GetComponent<PlayerActions>();
        playerI = GetComponent<PlayerInteractions>();

        actions = new InputSystem_Actions.PlayerActions();
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
    public void PausePlayer(bool paused)
    {
        playerM.isPaused = paused;
        playerA.isPaused = paused;
        playerI.isPaused = paused;

        playerM.enabled = !paused;
        playerA.enabled = !paused;
        playerI.enabled = !paused;
    }
}
