using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    // Input fields
    private ThirdPersonActionsAsset playerActionsAsset;
    private InputAction move;
    private InputAction run;

    // Movement fields
    private Rigidbody rb;
    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 5f;

    // Public variables for walk and run speeds
    [SerializeField]
    public float walkSpeed = 5f;
    [SerializeField]
    public float runSpeed = 7.5f; // Speed for running
    private float currentMaxSpeed;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private Camera playerCamera;
    private Animator animator;

    // New fields for jumping and grounded state
    private bool isJumping;
    private bool isGrounded;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionsAsset = new ThirdPersonActionsAsset();
        animator = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerActionsAsset.Player.Jump.started += DoJump;
        move = playerActionsAsset.Player.Move;
        run = playerActionsAsset.Player.Run;
        playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Player.Jump.started -= DoJump;
        playerActionsAsset.Player.Disable();
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if (isGrounded) // Only jump if grounded
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsGrounded", false);
            isJumping = true; // Set jumping state
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            return true;
        else
            return false;
    }

    private void FixedUpdate()
    {
        // Update currentMaxSpeed based on run input
        bool isRunning = run.ReadValue<float>() > 0.1f; // Check if Shift is held down
        currentMaxSpeed = isRunning ? runSpeed : walkSpeed;

        // Update the movement force direction
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        // Adjust gravity and horizontal velocity
        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > currentMaxSpeed * currentMaxSpeed)
            rb.velocity = horizontalVelocity.normalized * currentMaxSpeed + Vector3.up * rb.velocity.y;

        LookAt();

        // Update animation parameters
        UpdateAnimationParameters(horizontalVelocity.magnitude, isRunning); // Pass horizontal velocity magnitude and run status
    }

    private void UpdateAnimationParameters(float horizontalVelocityMagnitude, bool isRunning)
    {
        // Calculate speed as a normalized value between 0 and 1
        float normalizedSpeed = horizontalVelocityMagnitude / currentMaxSpeed;

        // If no movement (speed is very small), set the speed to 0
        if (horizontalVelocityMagnitude < 0.1f)
        {
            normalizedSpeed = 0f;
        }
        else if (!isRunning) // Walking, use a speed range between 0.1 and 0.5
        {
            normalizedSpeed = Mathf.Clamp(normalizedSpeed, 0.1f, 0.5f);
        }
        else // Running, set to 1
        {
            normalizedSpeed = Mathf.Clamp(normalizedSpeed, 0.5f, 1f); // Adjust range for running
        }

        animator.SetFloat("speed", normalizedSpeed);
    }

    private void UpdateVerticalAnimations()
    {
        Debug.Log($"Grounded: {isGrounded}, VelocityY: {rb.velocity.y}");

        isGrounded = IsGrounded(); // Check grounded state

        if (isGrounded)
        {
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            isJumping = false;
        }
        else
        {
            animator.SetBool("IsGrounded", false);

            if (rb.velocity.y < -0.1f) // Falling
            {
                animator.SetBool("IsFalling", true);
                animator.SetBool("IsJumping", false);
            }
        }
    }

    private void Update()
    {
        UpdateVerticalAnimations();

        // Handle walking and running animations
        float horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        bool isRunning = run.ReadValue<float>() > 0.1f;
        UpdateAnimationParameters(horizontalVelocity, isRunning);
    }
}
