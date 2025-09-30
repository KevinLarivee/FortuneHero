using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    static PlayerMovement instance;
    public static PlayerMovement Instance { get { return instance; } }

    [SerializeField] GameObject jumpVFX;
    CharacterController player;
    Animator animator;
    Vector3 jump;
    Vector3 knockBackDirection;
    Vector2 move;
    Vector2 look;
    Vector3 direction;

    #region Aiming
    [Header("Aiming")]
    [SerializeField] CinemachineCamera freelookCam;
    [SerializeField] CinemachineCamera aimCam;
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
    bool isAiming = false;
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
        //playerCamera = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        inputAxisController = freelookCam.GetComponent<CinemachineInputAxisController>();
        aimingCamera = aimCam.GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimingCamera.CameraSide;

        Vector3 angles = dirTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        //Vector3 moveDirection = Vector3.zero;

        //if (isAiming)
        //{
        //    Vector3 forward = transform.forward;
        //    Vector3 right = transform.right;

        //    forward.y = 0;
        //    right.y = 0;

        //    forward.Normalize();
        //    right.Normalize();
        //    moveDirection = forward * move.y + right * move.x;
        //}
        //else
        //{
        //    Vector3 forward = playerCamera.forward;
        //    Vector3 right = playerCamera.right;

        //    forward.y = 0;
        //    right.y = 0;

        //    forward.Normalize();
        //    right.Normalize();
        //    moveDirection = forward * move.y + right * move.x;
        //}
        //player.Move(moveDirection * speedMultiplier * Time.deltaTime);

        //if (isAiming)
        //{
        //    yaw += look.x * sensitivity;
        //    pitch -= look.y * sensitivity;

        //    yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        //    pitchTarget.rotation = Quaternion.Euler(pitch, 0f, 0f);

        //    aimingCamera.CameraSide = Mathf.Lerp(aimingCamera.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);



        //    Vector3 lookDirection = yawTarget.forward;
        //    lookDirection.y = 0;

        //    if(lookDirection.sqrMagnitude > 0.01f)
        //    {
        //        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        //    }
        //}
        //else if (shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        //{
        //    Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        //}

        //velocity.y += gravity * Time.deltaTime;
        //player.Move(velocity * Time.deltaTime);






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
        if(!isDashing)
            move = ctx.ReadValue<Vector2>();
    }
    public void Look(InputAction.CallbackContext ctx)
    {
        look = ctx.ReadValue<Vector2>() * mouseSensitivity;
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

    public void SwitchShoulder(InputAction.CallbackContext context)
    {
        targetCameraSide = aimingCamera.CameraSide < 0.5f ? 1f : 0f;
    }

    public void Aim(InputAction.CallbackContext context)
    {
        isAiming = !context.canceled;

        if (isAiming)
        {
            SetYawPitchFromCameraForward(freelookCam.transform);
            aimCam.Priority = 20;
            freelookCam.Priority = 10;
            inputAxisController.enabled = false;
        }
        else
        {
            SnapFreeLookBehindPlayer();
            aimCam.Priority = 10;
            freelookCam.Priority = 20;
            inputAxisController.enabled = true;
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
