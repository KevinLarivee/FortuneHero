using UnityEngine;
using Unity.Cinemachine;

public class PlayerComponent : MonoBehaviour
{
    static PlayerComponent instance;
    public static PlayerComponent Instance { get { return instance; } }


    [Header("Movement")]
    public float jumpMultiplier = 1f;
    public float gravityMultiplier = 2; //Dans playerComponenet pour le jumpPad ? (modifier la grav. pour faire floter le joueur ?)
    public float speedMultiplier = 1f;
    public float dashCooldown = 0.75f; //Pour upgrades ?
    public float dashSpeed = 2f; //Pour upgrades ?
    public bool bossDisableDash = false; //A utiliser dans PlayerMovement pour disable si le boss veut ?

    [Header("Status Effect")]
    public bool isParalysed = false;
    public bool isBurning = false;

    [Header("Player status")]
    public int currentCoins = 0;
    public int currentXp = 0;
    public int currentLevel = 0;
    public int xpRequirement = 100;

    [Header("Attacks")]
    public int meleeAtkDmg = 10;
    public int rangedAtkDmg = 20;

    [Header("Shield")]
    public bool bossDisableShield = false; //A utiliser dans PlayerMovement pour disable si le boss veut ?

    [SerializeField] private float minSensitivity = 0.5f;
    [SerializeField] private float maxSensitivity = 5f;

    [SerializeField] CinemachineInputAxisController inputController;
    PlayerMovement playerM;
    PlayerActions playerA;
    PlayerInteractions playerI;
    HealthComponent healthComponent;
    Animator animator;
    PlayerOverlayComponent playerOverlay;

    InputSystem_Actions.PlayerActions actions;
    void Awake()
    {
        instance = this;

        playerM = GetComponent<PlayerMovement>();
        playerA = GetComponent<PlayerActions>();
        playerI = GetComponent<PlayerInteractions>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<HealthComponent>();
        playerOverlay = GetComponent<PlayerOverlayComponent>();

        actions = new InputSystem_Actions.PlayerActions();

        healthComponent.onDeath = PlayerDeath;

        float saved = PlayerPrefs.GetFloat(MouseSensativity.PrefKey, 1f);
        ApplySensitivity(saved);
    }

    public void GetXpAndCoins(int xpGain, int coinGain) //Mettre valeur negative pour perdre coins ou xp, pour get un des deux, mettre l'autre a 0
    {
        currentXp += xpGain;
        currentCoins += coinGain;
        playerOverlay.AddCoins(coinGain);
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
    public void ApplySensitivity(float normalizedValue)
    {
        // Convertir [0..1], [min..max]
        float sens = Mathf.Lerp(minSensitivity, maxSensitivity, normalizedValue);

        foreach (var c in inputController.Controllers)
        {
            Debug.Log($"Controller: {c.Name} with value {sens}");

            c.Input.Gain = sens;
            if (c.Name == "Look Orbit Y")
                c.Input.Gain *= -1; //Inverser l'axe Y pour la camera
        }
    }
    public void PlayerDeath()
    {
        //jouer animation de mort
        animator.SetTrigger("isDead");
        PausePlayer(true);
        //Afficher lecran de mort
        GameManager.Instance.OnPlayerDeath();
        //particle effect ou autre ??
        //Comment on veut reset les stats et tt. (boss stats, coins, currenthp, etc.)
    }
    private void OnDestroy()
    {
        instance = null;
    }
}
