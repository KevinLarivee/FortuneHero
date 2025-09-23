using System.Collections;
using TreeEditor;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    static PlayerMovement instance;
    public static PlayerMovement Instance { get { return instance; } }

    Camera playerCamera;
    CharacterController player;
    Animator animator;

    [SerializeField] float cameraSpeed = 15f;



    #region Dash

    [SerializeField] float dashCooldown = 0.75f;
    [SerializeField] float dashTime = 0.2f;
    [SerializeField] float dashSpeed = 2f;
    //bool isDashing = false;
    bool canDash = true;

    #endregion

    #region Jump
    [Header("Jump")]
    public float jumpMultiplier = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -13;
    [SerializeField] float gravityMultiplier = 2;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float jumpBufferTime = 0.2f;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    bool doubleJumped = false;
    bool isAirborne = false;
    #endregion

    #region Player Status
    int currentXp = 0;
    int currentLevel = 0;
    int xpRequirement = 100;
    
    [Header("Status")]
    [SerializeField] int currentCoins = 0;
    [SerializeField] int hp = 100;
    int maxHp;

    [Header("Speed")]
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float slowedAcceleration = 9f;
    [SerializeField] float acceleration = 12f;
    [SerializeField] float deceleration = 10f;
    [SerializeField] float slowedDownSpeed = 3f;

    [Header("Status Effect")]
    string statusEffect = "";
    int statusDuration = 0;
    int statusTickDmg = 0;
    #endregion

    Vector3 cameraRotation;
    [SerializeField] Vector3 jump;
    Vector2 look;
    Vector2 move;
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
    }

    void Update()
    {
        Movement();
        RotateCamera();
    }

    public void Movement()
    {
        Vector3 direction = playerCamera.transform.forward * move.y + playerCamera.transform.right * move.x;
        if (direction.magnitude > 0)
            direction.Normalize();
        direction.y = 0;

        if (player.isGrounded)
        {
            jump.y = Mathf.Max(-1, jump.y);
            coyoteTimeCounter = coyoteTime;
            doubleJumped = false;
            isAirborne = false;
            animator.SetBool("isFalling", false);
        }
        else
        {
            jump += gravity * gravityMultiplier * Time.deltaTime * transform.up;
            if(jump.y < 0)
            {
                animator.SetBool("isFalling", true);
            }
            coyoteTimeCounter -= Time.deltaTime;
            isAirborne = true;
        }

        //Jump + buffer mechanics
        if (jumpBufferCounter > 0f)
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
        if (player.isGrounded)
        {
            if (direction.sqrMagnitude > 0.001f) //si ya input
            {
               
                if (moveSpeed < maxSpeed && move.y > 0) //et que ya pas atteint sa vitesse max
                    moveSpeed = Mathf.Min(moveSpeed + acceleration * Time.deltaTime, maxSpeed); //accelere
                if (move.y <= 0)
                    moveSpeed = Mathf.Min(moveSpeed + slowedAcceleration * Time.deltaTime, slowedDownSpeed); //pas forward = ralenti
            }
            else
            {
                if (moveSpeed > 0f) //bouge pas mais a tjrs vitesse
                    moveSpeed = Mathf.Max(moveSpeed - deceleration * Time.deltaTime, 0f); //decelere
            }
        }
        /*else
        {
        }*/

        animator.SetFloat("x", move.x, 0.2f, Time.deltaTime);
        animator.SetFloat("y", move.y, 0.2f, Time.deltaTime);
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
        if(ctx.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        canDash = false;
        float originalGravity = gravity;
        gravity = 0f;
        moveSpeed = moveSpeed * dashSpeed;
        //animation.setBool("isDashing", true);
        yield return new WaitForSeconds(dashTime);
        
        gravity = originalGravity;
        moveSpeed = maxSpeed;
        //animation.setBool("isDashing", false);
        yield return new WaitForSeconds(dashCooldown);
        
        canDash = true;
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
