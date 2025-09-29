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

    [SerializeField] GameObject jumpVFX;
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

    [SerializeField] float dashCooldown = 0.75f;
    [SerializeField] float dashTime = 0.2f;
    [SerializeField] float dashSpeed = 2f;
    bool canDash = true;
    bool isDashing = false;

    #region Jump
    [Header("Jump")]
    public float jumpMultiplier = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -13;
    [SerializeField] float gravityMultiplier = 2;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float jumpBufferTime = 0.2f;
    [SerializeField] float doubleJumpAnimDelay = 0.5f;
    [SerializeField] float doubleJumpAnimTimer;
    float jumpVFXCd = 0.4f; 
    float coyoteTimeCounter;
    float jumpBufferCounter;
    bool doubleJumpStartTimer = false;
    bool doubleJumped = false;
    #endregion

    [Header("Speed")]
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] float maxSpeed = 12f;
    [SerializeField] float slowedAcceleration = 9f;
    [SerializeField] float acceleration = 12f;
    [SerializeField] float deceleration = 10f;
    [SerializeField] float slowedDownSpeed = 6f;
    [SerializeField] float speedWhileDefending = 3f;
    float speedMultiplier = 1f;

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

    }

    void Update()
    {
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
            jump.y = Mathf.Max(-10, jump.y);
            coyoteTimeCounter = coyoteTime;
            doubleJumped = false;
            animator.SetBool("isGrounded", true);
            animator.SetBool("hasJumped", false);
        }
        else
        {
            jump.y += Mathf.Max(gravity * gravityMultiplier * Time.deltaTime, -15);
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
                StartCoroutine(StartJumpVFX());
            }
            else if (coyoteTimeCounter <= 0f && !doubleJumped)
            {
                jump = transform.up * (jumpForce * jumpMultiplier);
                jumpBufferCounter = 0f;
                doubleJumped = true;
                doubleJumpStartTimer = true;
                StartCoroutine(StartJumpVFX());
            }
        }
        if (doubleJumpStartTimer)
        {
            doubleJumpAnimTimer -= Time.deltaTime;
            animator.SetBool("hasDoubleJumped", true);
        }
        if (doubleJumpAnimTimer <= 0)
        {
            doubleJumpStartTimer = false;
            animator.SetBool("hasDoubleJumped", false);
            doubleJumpAnimTimer = doubleJumpAnimDelay;
        }
        if (direction.sqrMagnitude > 0.001f) //si ya input
        {
            float drag = IsGrounded() ? 1f : 0.5f;
            if (moveSpeed <= maxSpeed && move.y > 0)
                moveSpeed = Mathf.Min(moveSpeed + acceleration * drag * Time.deltaTime, maxSpeed * speedMultiplier); //accelere
            else if (move.y <= 0)
                moveSpeed = Mathf.Min(moveSpeed + slowedAcceleration * drag * Time.deltaTime, slowedDownSpeed * speedMultiplier); //pas forward = ralenti
        }
        else
        {
            if (moveSpeed > 0f) //bouge pas mais a tjrs vitesse
                moveSpeed = Mathf.Max(moveSpeed - deceleration * Time.deltaTime, 0f); //decelere
        }

        animator.SetFloat("x", move.x, 0.2f, Time.deltaTime);
        animator.SetFloat("y", move.y, 0.2f, Time.deltaTime);
        player.Move((moveSpeed * direction + jump) * Time.deltaTime);
    }
    public bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(origin, 0.4f, Vector3.down, out RaycastHit hit, 0.2f, playerLayer))
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
        if(!isDashing)
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
        if (ctx.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        canDash = false;
        float originalGravity = gravity;
        gravity = 0f;
        moveSpeed = maxSpeed * dashSpeed;
        isDashing = true;
        //animation.setBool("isDashing", true);
        yield return new WaitForSeconds(dashTime);

        gravity = originalGravity;
        moveSpeed = maxSpeed;
        isDashing = false;
        //animation.setBool("isDashing", false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public void SlowPlayer(float divider)
    {
        speedMultiplier /= divider;
    }
    public void SpeedUpPlayer(float multiplier)
    {
        speedMultiplier *= multiplier;
    }

    public void KnockBack(Vector3 direction, float knockForce)
    {
        knockBackCounter = knockBackTime;
        knockBackDirection = direction * knockForce;

    }
    public IEnumerator StartJumpVFX()
    {
        var gameobject = Instantiate(jumpVFX, transform.position + Vector3.up * 0.5f, Quaternion.Euler(90, 0, 0)); //Object Pool
        yield return new WaitForSeconds(jumpVFXCd);
        Destroy(gameobject);
    }
}

//Source for jumping mechanics: https://www.youtube.com/watch?v=RFix_Kg2Di0
