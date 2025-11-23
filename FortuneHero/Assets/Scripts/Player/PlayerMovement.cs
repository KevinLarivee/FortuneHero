using System.Collections;
using System.Drawing;
using Unity.Cinemachine;
using Unity.InferenceEngine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    static PlayerMovement instance;
    public static PlayerMovement Instance { get { return instance; } }

    public bool isPaused = false;

    [SerializeField] GameObject jumpVFX;
    [SerializeField] GameObject lightningVFX;
    [SerializeField] GameObject fireVFX;

    [Header("Audio")]
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip landingClip;
    [SerializeField] AudioClip dashClip;
    AudioSource audioSource;


    CharacterController player;
    Animator animator;
    HealthComponent health;
    PlayerComponent playerInstance;
  

    [SerializeField] LayerMask groundLayers;
    LayerMask currentGroundLayer;
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

    [SerializeField] float dashTime = 0.2f;
    bool canDash = true;
    bool isDashing = false;

    #region Jump
    [Header("Jump")]
    //public float jumpMultiplier = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -13;
    //[SerializeField] float gravityMultiplier = 2;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float jumpBufferTime = 0.2f;
    [SerializeField] float doubleJumpAnimDelay = 0.5f;
    [SerializeField] float doubleJumpAnimTimer;
    float jumpVFXCd = 0.4f;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    bool doubleJumpStartTimer = false;
    bool doubleJumped = false;
    bool usedJumpPad = false;
    bool landed = false;
    #endregion

    [Header("Speed")]
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] float maxSpeed = 12f;
    [SerializeField] float acceleration = 12f;
    [SerializeField] float deceleration = 10f;
    float groundDrag = 1f;
    float airDrag = 0.5f;

    #region Status
    [SerializeField] float burnTimeUntilDmgTick = 1f;
    [SerializeField] float knockBackTime = 0.5f;
    [SerializeField] float invDuration = 1f;
    float knockBackTimer;
    float burnDmgPerTick = 2f; //Temp ? (envoye par fireComponent potentiellement)
    float burnTimer;
    int burnCount = 0;
    float paralyseTimer;
    bool canJump = true;
    bool isKnockedBack = false;
    #endregion

    private float knockBackVerticalVelocity;
    void Awake()
    {
        instance = this;

        player = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<HealthComponent>();
        audioSource = GetComponent<AudioSource>();

        inputAxisController = freelookCam.GetComponent<CinemachineInputAxisController>();
        aimingCamera = aimCam.GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimingCamera.CameraSide;

        Vector3 angles = dirTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }
    void Start()
    {
        playerInstance = PlayerComponent.Instance;
    }

    void Update()
    {
        if (knockBackTimer <= 0)
        {
            if (!playerInstance.isParalysed)
                Movement();
            else
            {
                paralyseTimer -= Time.deltaTime;
                lightningVFX.SetActive(true);
                animator.SetBool("isParalysed", true);
                if (paralyseTimer <= 0)
                {
                    lightningVFX.SetActive(false);
                    playerInstance.isParalysed = false;
                    animator.SetBool("isParalysed", false);
                    StartCoroutine(MakeInvincible(invDuration));
                }
            }

            RotateCamera();
        }
        else
        {
            knockBackTimer -= Time.deltaTime;
            player.Move(knockBackDirection * Time.deltaTime);
        }

        if (playerInstance.isBurning)
        {
            fireVFX.SetActive(true);
            burnTimer += Time.deltaTime;
            if (burnTimer >= burnTimeUntilDmgTick)
            {
                health.Hit(burnDmgPerTick);
                burnTimer = 0;
                Debug.Log("You have been burned! - " + burnDmgPerTick + "hp");
                animator.SetTrigger("isHit");
            }
        }
        else
            fireVFX.SetActive(false);

    }
    public void Movement()
    {
        Vector3 forward = isAiming ? transform.forward : freelookCam.transform.forward;
        Vector3 right = isAiming ? transform.right : freelookCam.transform.right;

        if(!isDashing)
        {
            direction = forward * move.y + right * move.x;
            if (direction.magnitude > 0)
                direction.Normalize();
            direction.y = 0;
        }

        if (IsGrounded())
        {
            if (!landed)
            {
                //audioSource.clip = landingClip;
                //audioSource.outputAudioMixerGroup = PlayerComponent.Instance.SFXGroup;
                //audioSource.Play();
                SFXManager.Instance.PlaySFX(landingClip, transform, PlayerComponent.Instance.SFXGroup);

                landed = true;
            }
            if (jump.y <= -10)
            {
                if (usedJumpPad)
                {
                    usedJumpPad = false;
                    jump.x = 0;
                    jump.z = 0;
                }
            }
            jump.y = Mathf.Max(-10, jump.y);
            coyoteTimeCounter = coyoteTime;
            doubleJumped = false;
            animator.SetBool("isGrounded", true);
            animator.SetBool("hasJumped", false);
        }
        else
        {
            jump.y += Mathf.Max(gravity * playerInstance.gravityMultiplier * Time.deltaTime, -15);
            coyoteTimeCounter -= Time.deltaTime;
            animator.SetBool("isGrounded", false);
            landed = false;
        }

        //Jump + buffer mechanics
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (IsGrounded() || coyoteTimeCounter > 0f)
            {
                coyoteTimeCounter = 0f;
                jump = transform.up * (jumpForce * playerInstance.jumpMultiplier);
                jumpBufferCounter = 0f;
                isKnockedBack = false;
                animator.SetBool("hasJumped", true);
                StartCoroutine(StartJumpEffects());
            }
            else if (coyoteTimeCounter <= 0f && !doubleJumped)
            {
                jump = transform.up * (jumpForce * playerInstance.jumpMultiplier);
                jumpBufferCounter = 0f;
                doubleJumped = true;
                doubleJumpStartTimer = true;
                StartCoroutine(StartJumpEffects());
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

        //Movement
        if (direction.sqrMagnitude > 0.001f) //si ya input
        {
            float drag = IsGrounded() ? groundDrag : airDrag;
            if (moveSpeed <= maxSpeed * playerInstance.speedMultiplier)
                moveSpeed = Mathf.Min(moveSpeed + acceleration * drag * Time.deltaTime, maxSpeed * playerInstance.speedMultiplier); //accelere
            else if (moveSpeed > maxSpeed * playerInstance.speedMultiplier && !isDashing) //S'il est deja plus vite (ex.: il se fait slow mais deja full speed)
                moveSpeed = maxSpeed * playerInstance.speedMultiplier;
        }
        else
            if (moveSpeed > 0f) //bouge pas mais a tjrs vitesse
            moveSpeed = Mathf.Max(moveSpeed - deceleration * Time.deltaTime, 0f); //decelere (si arrete pour bref moment, recommence avec vitesse et non 0)


        if (!isAiming && direction.sqrMagnitude > 0.001f)
            animator.SetBool("isRunning", true);
        else
        {
            animator.SetFloat("x", move.x, 0.2f, Time.deltaTime);
            animator.SetFloat("y", move.y, 0.2f, Time.deltaTime);
            animator.SetBool("isRunning", false);
        }
           

        player.Move((moveSpeed * direction + jump) * Time.deltaTime);
    }
    public bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(origin, 0.4f, Vector3.down, out RaycastHit hit, 0.2f, groundLayers))
        {
            currentGroundLayer = hit.transform.gameObject.layer;
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
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        //if (isPaused) return;
            move = ctx.ReadValue<Vector2>();
    }
    public void Look(InputAction.CallbackContext ctx)
    {
        //if (isPaused) return;

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
        if (isPaused || playerInstance.bossDisableDash) return;

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
        //if (isPaused) return;

        isAiming = !context.canceled;

        if (isPaused || context.canceled)
        {
            SnapFreeLookBehindPlayer();
            aimCam.Priority = 10;
            freelookCam.Priority = 20;
            inputAxisController.enabled = true;
            SpeedUpPlayer(2);
            animator.SetBool("isAiming", false);
        }

        else if (isAiming && context.performed)
        {
            SetYawPitchFromCameraForward(freelookCam.transform);
            aimCam.Priority = 20;
            freelookCam.Priority = 10;
            inputAxisController.enabled = false;
            SlowPlayer(2);
            animator.SetBool("isAiming", true);
            animator.SetBool("isRunning", false);  
        }
    }

    public IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        //yield return new WaitForEndOfFrame();
        jump.y = 0f;
        moveSpeed = maxSpeed * playerInstance.speedMultiplier * playerInstance.dashSpeed;
        //audioSource.clip = dashClip;
        //audioSource.outputAudioMixerGroup = PlayerComponent.Instance.SFXGroup_Louder;
        //audioSource.Play();
        SFXManager.Instance.PlaySFX(dashClip, transform, PlayerComponent.Instance.SFXGroup_Louder);

        yield return new WaitForSeconds(dashTime);
        moveSpeed = maxSpeed * playerInstance.speedMultiplier;
        isDashing = false;
        yield return new WaitForSeconds(playerInstance.dashCooldown);

        canDash = true;
    }
    public void SlowPlayer(float divider) // les mettres dans playerComponent ???
    {
        if (isPaused) return;
        playerInstance.speedMultiplier /= divider;
    }
    public void SpeedUpPlayer(float multiplier)
    {
        if (isPaused) return;
        playerInstance.speedMultiplier *= multiplier;
    }
    public void KnockBack(Vector3 sourcePosition, float horizontalForce, float verticalMultiplier)
    {
        isKnockedBack = true;
        Vector3 direction = (transform.position - sourcePosition).normalized;
        direction.Normalize();

        knockBackDirection = direction * horizontalForce;
        knockBackDirection.y = horizontalForce * verticalMultiplier;
        knockBackTimer = knockBackTime;

        animator.SetTrigger("isHit");
        animator.SetBool("isRunning", false);
    }
    public void ToggleBurn(bool burning)
    {
        if (!health.isInvincible)
        {
            burnCount += burning ? 1 : -1;
            playerInstance.isBurning = burnCount > 0;
        }
    }
    public void ToggleParalyse(float paralyseTime)
    {
        if (!playerInstance.isParalysed && !health.isInvincible)
        {
            playerInstance.isParalysed = true;
            paralyseTimer = paralyseTime;
        }
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
    public void SetJumpPadForce(Vector3 force)
    {
        usedJumpPad = true;
        jump = force;
        animator.SetBool("hasJumped", true);
    }
    public IEnumerator StartJumpEffects()
    {
        //audioSource.clip = jumpClip;
        //audioSource.outputAudioMixerGroup = PlayerComponent.Instance.SFXGroup;
        //audioSource.Play();
        SFXManager.Instance.PlaySFX(jumpClip, transform, PlayerComponent.Instance.SFXGroup);

        var gameobject = Instantiate(jumpVFX, transform.position + Vector3.up * 0.5f, Quaternion.Euler(90, 0, 0)); //Object Pool
        yield return new WaitForSeconds(jumpVFXCd);
        Destroy(gameobject);
    }
    public IEnumerator MakeInvincible(float duration)
    {
        health.isInvincible = true;
        yield return new WaitForSeconds(duration);
        health.isInvincible = false;
    }
    public void AfterBurn(float afterBurnTime) =>
        StartCoroutine(AfterBurnCR(afterBurnTime));
    IEnumerator AfterBurnCR(float afterBurnTime)
    {
        yield return new WaitForSeconds(afterBurnTime);
        Debug.Log("Stop Burn");
        //Retirer burning de la target
        ToggleBurn(false);
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
