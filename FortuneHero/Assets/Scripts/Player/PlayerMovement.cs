using System.Collections;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    static PlayerMovement instance;
    public static PlayerMovement Instance { get { return instance; } }

    Camera playerCamera;
    CharacterController player;
    Animator animator;
    Vector3 cameraRotation;
    Vector3 jump;
    Vector3 knockBackDirection;
    Vector2 look;
    Vector2 move;
    [SerializeField] LayerMask playerLayer;

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
    [SerializeField] float currentMaxSpeed;
    float baseMaxSpeed = 12f;

    [SerializeField] float slowedAcceleration = 9f;
    [SerializeField] float acceleration = 12f;
    [SerializeField] float deceleration = 10f;
    [SerializeField] float slowedDownSpeed = 6f;
    [SerializeField] float speedWhileDefending = 3f;
    bool isDefending = false;

    [Header("Status Effect")]
    string statusEffect = "";
    int statusDuration = 0;
    int statusTickDmg = 0;
    #endregion

    #region KnockBack
    float knockBackTime;
    float knockBackCounter;
    #endregion

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
        currentMaxSpeed = baseMaxSpeed;
    }

    void Update()
    {
        Debug.Log(IsGrounded());
        if (knockBackCounter <= 0)
        {
            Movement();
            RotateCamera();
        }
        else
        {
            knockBackCounter -= Time.deltaTime;
            player.Move(knockBackDirection * Time.deltaTime);
        }
    }

    public void Movement()
    {
        Vector3 direction = playerCamera.transform.forward * move.y + playerCamera.transform.right * move.x;
        if (direction.magnitude > 0)
            direction.Normalize();
        direction.y = 0;

        if (IsGrounded())
        {
            jump.y = Mathf.Max(-1, jump.y);
            coyoteTimeCounter = coyoteTime;
            doubleJumped = false;
            animator.SetBool("isGrounded", true);
            animator.SetBool("hasJumped", false);
        }
        else
        {
            jump += gravity * gravityMultiplier * Time.deltaTime * transform.up;
            coyoteTimeCounter -= Time.deltaTime;
            animator.SetBool("isGrounded", false);
        }

        //Jump + buffer mechanics
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (IsGrounded() || coyoteTimeCounter > 0f)
            {
                coyoteTimeCounter = 0f;
                jump = transform.up * (jumpForce * jumpMultiplier);
                jumpBufferCounter = 0f;
                animator.SetBool("hasJumped", true);
            }
            else if (coyoteTimeCounter <= 0f && !doubleJumped)
            {
                jump = transform.up * (jumpForce * jumpMultiplier);
                doubleJumped = true;
                jumpBufferCounter = 0f;
                animator.SetTrigger("doubleJump");
            }
        }
        if (IsGrounded())
        {
            if (direction.sqrMagnitude > 0.001f) //si ya input
            {
               
                if (moveSpeed < currentMaxSpeed && move.y > 0) //et que ya pas atteint sa vitesse max
                    moveSpeed = Mathf.Min(moveSpeed + acceleration * Time.deltaTime, currentMaxSpeed); //accelere
                if (move.y <= 0)
                    moveSpeed = Mathf.Min(moveSpeed + slowedAcceleration * Time.deltaTime, isDefending ? speedWhileDefending : slowedDownSpeed); //pas forward = ralenti
            }
            else
            {
                if (moveSpeed > 0f) //bouge pas mais a tjrs vitesse
                    moveSpeed = Mathf.Max(moveSpeed - deceleration * Time.deltaTime, 0f); //decelere
            }
        }
        else
        {

        }

        animator.SetFloat("x", move.x, 0.2f, Time.deltaTime);
        animator.SetFloat("y", move.y, 0.2f, Time.deltaTime);
        player.Move((moveSpeed * direction + jump) * Time.deltaTime);
    }
    public bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.4f;

        if (Physics.SphereCast(origin, 0.4f, Vector3.down, out RaycastHit hit, 0.5f, playerLayer))
        {
            return true;
        }
        return false;
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
        moveSpeed = baseMaxSpeed * dashSpeed;
        //animation.setBool("isDashing", true);
        yield return new WaitForSeconds(dashTime);
        
        gravity = originalGravity;
        moveSpeed = currentMaxSpeed;
        //animation.setBool("isDashing", false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public void SlowPlayer() 
    {
        isDefending = !isDefending;
        if (isDefending)
            currentMaxSpeed = speedWhileDefending;
        else
            currentMaxSpeed = baseMaxSpeed;
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

    public void KnockBack(Vector3 direction, float knockForce)
    {
        knockBackCounter = knockBackTime;
        knockBackDirection = direction * knockForce;

    }
}

//Source for jumping mechanics: https://www.youtube.com/watch?v=RFix_Kg2Di0
