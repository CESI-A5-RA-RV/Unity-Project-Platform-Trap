using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControllerTest : MonoBehaviour
{
    // Input fields
    private Vector2 movementInput;
    private bool isRunningInput;
    private bool isJumpingInput;

    // Movement fields
    private Rigidbody rb;
    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 5f;

    [SerializeField]
    public float walkSpeed = 5f;
    [SerializeField]
    public float runSpeed = 7.5f; // Speed for running
    private float currentMaxSpeed;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private Camera playerCamera;
    private Animator animator;

    // Grounded state
    private bool isGrounded;

    private bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunningInput = true;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            isJumpingInput = true;
        }
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight()
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        return Physics.Raycast(ray, out _, 0.3f);
    }

    public void EnableMovement(){
        canMove = true;
        Debug.Log("Player Free");
    }

    public void DisableMovement(){
        canMove = false;
        Debug.Log("Player Trapped");
    }

    private void FixedUpdate()
    {
        if(canMove){
            // Check grounded state
        isGrounded = IsGrounded();

        // Update speed based on running input
        currentMaxSpeed = isRunningInput ? runSpeed : walkSpeed;

        // Movement direction
        forceDirection += movementInput.x * GetCameraRight() * movementForce;
        forceDirection += movementInput.y * GetCameraForward() * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        // Jumping
        if (isJumpingInput)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumpingInput = false;
        }

        // Clamp horizontal velocity
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > currentMaxSpeed * currentMaxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * currentMaxSpeed + Vector3.up * rb.velocity.y;
        }

        // Apply gravity if falling
        if (rb.velocity.y < 0f)
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
        }

        LookAt();

        // Update animations
        UpdateAnimationParameters(horizontalVelocity.magnitude, isRunningInput);
        }
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (movementInput.sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void UpdateAnimationParameters(float horizontalVelocityMagnitude, bool isRunning)
    {
        float normalizedSpeed = horizontalVelocityMagnitude / currentMaxSpeed;

        if (horizontalVelocityMagnitude < 0.1f)
        {
            normalizedSpeed = 0f;
        }
        else if (!isRunning)
        {
            normalizedSpeed = Mathf.Clamp(normalizedSpeed, 0.1f, 0.5f);
        }
        else
        {
            normalizedSpeed = Mathf.Clamp(normalizedSpeed, 0.5f, 1f);
        }

        animator.SetFloat("speed", normalizedSpeed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded && rb.velocity.y > 0);
        animator.SetBool("IsFalling", !isGrounded && rb.velocity.y < 0);
    }
}
