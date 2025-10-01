using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    static PlayerMovement instance;
    public static PlayerMovement Instance { get { return instance; } }

    public bool isPaused = false;

    [SerializeField] GameObject jumpVFX;
    CharacterController player;
    Animator animator;
    HealthComponent health;

    Vector3 jump;
    Vector3 knockBackDirection;
    Vector2 move;
    Vector2 look;
    Vector3 direction;

    #region Aiming
    [Header("Aiming")]
    [SerializeField] CinemachineCamera freelookCam;
    [SerializeField] CinemachineCamera aimCam;
    [SerializeField] GameObject crossHair;
    //[SerializeField] Transform playerCamera;
    [SerializeField] Transform dirTarget;
    [SerializeField] float sensitivity = 1.5f;
    [SerializeField] float mouseSensitivity = 0.05f;
    [SerializeField] float pitchMin = -40f;
    [SerializeField] float pitchMax = 80f;
    [SerializeField] float shoulderSwitchSpeed = 5f;
    CinemachineInputAxisController inputAxisController;
    CinemachineThirdPersonFollow aimingCamera; //jsp comment combiner
    float yaw;
    float pitch;
    float targetCameraSide;
    public bool isAiming = false;
    //Vector3 cameraRotation;
    #endregion

    [SerializeField] LayerMask playerLayer;

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

    #region Status
    float knockBackTime;
    float knockBackCounter;

    [SerializeField] bool isParalysed = false;
    [SerializeField] bool isBurning = false;
    [SerializeField] float paralyseTime = 3f;
    [SerializeField] float burnTimeUntilDmgTick = 1f;
    float burnDmgPerTick = 2f; //Temp ? (envoye par fireComponent potentiellement)
    float burnTimer;

    bool isInCoroutine = false;
    bool canJump = true;
    #endregion

    void Awake()
    {
       instance = this;

        player = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<HealthComponent>();

        inputAxisController = freelookCam.GetComponent<CinemachineInputAxisController>();
        aimingCamera = aimCam.GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimingCamera.CameraSide;

        Vector3 angles = dirTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (knockBackCounter <= 0)
        {
            if (!isParalysed)
                Movement();
            else 
                if(!isInCoroutine)
                    StartCoroutine(ApplyParalyse());

            RotateCamera();
        }
        else
        {
            knockBackCounter -= Time.deltaTime;
            player.Move(knockBackDirection * Time.deltaTime);
        }
        if (isBurning)
        {
            burnTimer += Time.deltaTime;
            if (burnTimer >= burnTimeUntilDmgTick)
            {
                health.Hit(burnDmgPerTick);
                burnTimer = 0;
                Debug.Log("You have been burned! - " + burnDmgPerTick + "hp");
                animator.SetTrigger("isHit");
            }
        }
    }

    public void Movement()
    {
        Vector3 forward = isAiming ? transform.forward : freelookCam.transform.forward;
        Vector3 right = isAiming ? transform.right : freelookCam.transform.right;

        direction = forward * move.y + right * move.x;
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
            if (moveSpeed <= maxSpeed)
                moveSpeed = Mathf.Min(moveSpeed + acceleration * drag * Time.deltaTime, maxSpeed * speedMultiplier); //accelere
            //else if (move.y <= 0)
            //    moveSpeed = Mathf.Min(moveSpeed + slowedAcceleration * drag * Time.deltaTime, slowedDownSpeed * speedMultiplier); //pas forward = ralenti
        }
        else
        {
            if (moveSpeed > 0f) //bouge pas mais a tjrs vitesse
                moveSpeed = Mathf.Max(moveSpeed - deceleration * Time.deltaTime, 0f); //decelere
        }


        if (!isAiming && direction.sqrMagnitude > 0.001f)
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);

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
        crossHair.SetActive(isAiming);
        if (isAiming)
        {
            yaw += look.x * sensitivity;
            pitch -= look.y * sensitivity;

            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

            Quaternion yawTemp = Quaternion.AngleAxis(yaw, Vector3.up);    // gauche-droite
            Quaternion pitchTemp = Quaternion.AngleAxis(pitch, Vector3.right); // haut-bas

            dirTarget.rotation = yawTemp * pitchTemp;

            aimingCamera.CameraSide = Mathf.Lerp(aimingCamera.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);



            Vector3 lookDirection = dirTarget.forward;
            lookDirection.y = 0;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
        else if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        //cameraRotation += cameraSpeed * Time.deltaTime * new Vector3(-look.y, look.x, 0);
        //cameraRotation.x = Mathf.Clamp(cameraRotation.x, -50, 50);
        //transform.rotation = Quaternion.Euler(new Vector3(0, cameraRotation.y, 0));
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (isPaused) return;

        if (!isDashing)
            move = ctx.ReadValue<Vector2>();
    }
    public void Look(InputAction.CallbackContext ctx)
    {
        if (isPaused) return;

        look = ctx.ReadValue<Vector2>() * mouseSensitivity;
    }
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (isPaused) return;

        if (ctx.performed)
            jumpBufferCounter = jumpBufferTime;
    }
    public void Dash(InputAction.CallbackContext ctx)
    {
        if (isPaused) return;

        if (ctx.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void SwitchShoulder(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        targetCameraSide = aimingCamera.CameraSide < 0.5f ? 1f : 0f;
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        isAiming = !context.canceled;

        if (isAiming && context.performed)
        {
            SetYawPitchFromCameraForward(freelookCam.transform);
            aimCam.Priority = 20;
            freelookCam.Priority = 10;
            inputAxisController.enabled = false;
            SlowPlayer(2);
            animator.SetBool("isAiming", true);
            animator.SetBool("isRunning", false);
            animator.SetFloat("x", move.x, 0.2f, Time.deltaTime);
            animator.SetFloat("y", move.y, 0.2f, Time.deltaTime);
        }
        else if (context.canceled)
        {
            SnapFreeLookBehindPlayer();
            aimCam.Priority = 10;
            freelookCam.Priority = 20;
            inputAxisController.enabled = true;
            SpeedUpPlayer(2);
            animator.SetBool("isAiming", false);
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
    public void ToggleBurn(bool burning)
    {
        isBurning = burning;
    }
    public void ToggleParalyse(bool paralyse)
    {
        isParalysed = paralyse;
        //if (isParalysed)
        //    StartCoroutine(ApplyParalyse());
    }
    public IEnumerator ApplyParalyse() //A appeler ailleur plus tard ?
    {
        isInCoroutine = true;
        Debug.Log("Started Paralyse Coroutine");
        animator.SetBool("isParalysed", true);
        //Activer le particleSystem
        yield return new WaitForSeconds(paralyseTime);
        animator.SetBool("isParalysed", false);
        //Desactiver le particleSystem
        ToggleParalyse(false);
        isInCoroutine = false;
        
    }
    public void ToggleDash(bool dash)
    {
        canDash = dash;
    }
    public void ToggleJump(bool doubleJump, bool jump = true)
    {
        canJump = jump;
        doubleJumped = doubleJump;
    }
    public IEnumerator StartJumpVFX()
    {
        var gameobject = Instantiate(jumpVFX, transform.position + Vector3.up * 0.5f, Quaternion.Euler(90, 0, 0)); //Object Pool
        yield return new WaitForSeconds(jumpVFXCd);
        Destroy(gameobject);
    }

    void SnapFreeLookBehindPlayer()
    {
        CinemachineOrbitalFollow orbitalFollow = freelookCam.GetComponent<CinemachineOrbitalFollow>();
        Vector3 forward = aimCam.transform.forward;
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        orbitalFollow.HorizontalAxis.Value = angle;
    }

    void SetYawPitchFromCameraForward(Transform cameraTransform)
    {
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0f;

        if (flatForward.sqrMagnitude < 0.001f)
            return;

        yaw = Quaternion.LookRotation(flatForward).eulerAngles.y;

        Quaternion yawTemp = Quaternion.AngleAxis(yaw, Vector3.up);    // gauche-droite
        Quaternion pitchTemp = Quaternion.AngleAxis(pitch, Vector3.right); // haut-bas

        dirTarget.rotation = yawTemp * pitchTemp;
    }
}

//Source for jumping mechanics: https://www.youtube.com/watch?v=RFix_Kg2Di0
