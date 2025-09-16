using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    static PlayerMovement instance;
    public static PlayerMovement Instance { get { return instance; } } 


    Camera playerCamera;
    CharacterController player;
    Animator animator;

    [SerializeField] float gravity = -9.81f;
    [SerializeField] float cameraSpeed = 15f;

    #region Jump
    public float jumpMultiplier = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float jumpBufferTime = 0.2f;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    bool doubleJumped = false;
    #endregion

    #region Player Status
   
    float defenceChargeTime = 10f;
    float defenceChargeIncrement = 1f;
    float defenceConsumption = 1f; //Vitesse a laquelle le joeur perds de l'energie en bloquant
    
    float distanceAtkCd = 3f;
    float meleeAtkCd = 1f;
    float dashCd = 2.5f; //Cd = cooldown

    [SerializeField] int currentXp = 0;
    [SerializeField] int currentLevel = 0;
    [SerializeField] int xpRequirement = 100;
    [SerializeField] int currentCoins = 0;

    [SerializeField] int hp = 100;
    [SerializeField] int meleeAtkDmg = 10; //Dmg = damage
    [SerializeField] int distanceAtkDmg = 20;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float dashSpeed = 20f;
    int maxHp;

    string statusEffect = "";
    int statusDuration = 0;
    int statusTickDmg = 0;
    #endregion

    Vector3 cameraRotation;
    Vector3 jump;
    Vector2 look;
    Vector2 move;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        player = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        maxHp = hp;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        RotateCamera();
    }

    public void Movement()
    {
        Vector3 direction = playerCamera.transform.forward * move.y + playerCamera.transform.right * move.x;
        if(direction.magnitude > 0)
            direction.Normalize();
        direction.y = 0;

        if (player.isGrounded)
        {
            jump.y = Mathf.Max(-1, jump.y);
            doubleJumped = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            jump += Time.deltaTime * gravity * transform.up;
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Jump + buffer mechanics
        if(jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (player.isGrounded || coyoteTimeCounter > 0f)
            {
                coyoteTimeCounter = 0f;
                jump = transform.up * (jumpForce * jumpMultiplier);
                jumpBufferCounter = 0f;
                animator.SetTrigger("Jump");
            }
            else if (coyoteTimeCounter <= 0f && !doubleJumped)
            {
                jump = transform.up * (jumpForce * jumpMultiplier);
                doubleJumped = true;
                jumpBufferCounter = 0f;
            }
        }
        
        player.Move((moveSpeed * direction + jump) * Time.deltaTime);
    }

    public void RotateCamera()
    {
        cameraRotation += cameraSpeed * Time.deltaTime * new Vector3(-look.y, look.x, 0);
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -50, 50);
        transform.rotation = Quaternion.Euler(new Vector3(0, cameraRotation.y, 0));
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector2>();
        animator.SetFloat("x", move.x);
        animator.SetFloat("y", move.y);
    }
    public void Look(InputAction.CallbackContext ctx)
    {
        look = ctx.ReadValue<Vector2>();
    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            jumpBufferCounter = jumpBufferTime;
    }
    public void Dash(InputAction.CallbackContext ctx)
    {

    }

    public IEnumerator Dash()
    {
        yield return null;
    }

    public void UpdateHp(int dmg) //Mettre valeur negative pour regagner de la vie
    {
        hp -= dmg;
        if(hp > maxHp) //Peut pas depasser le max de hp
            hp = maxHp;

        //Faire autre logique: animations, sound effects, etc.
    }

    public void ApplyStatusEffect(string statusToApply, int duration, int tickDmg) //Mettre le statusEffect, son temps et son dmg par tick, si pas de dmg (ex.: paralyser) --> 0
    {
        statusEffect = statusToApply;
        statusDuration = duration;
        statusTickDmg = tickDmg;
        //pt d'autre logique, sinon juste mettre variable publique et faire qqchose avec
    }

    public void GetXpAndCoins(int xpGain, int coinGain) //Mettre valeur negative pour perdre coins ou xp, pour get un des deux, mettre l'autre a 0
    {
        currentXp += xpGain;
        currentCoins += coinGain;
        //Faire autre logique: sound effects, Ui updates (?), etc.
    }



}

//Source for jumping mechanics: https://www.youtube.com/watch?v=RFix_Kg2Di0
