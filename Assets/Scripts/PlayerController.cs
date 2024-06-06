using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private Vector3 movement;
    private bool isGrounded;
    private Transform cameraTransform;

    private PlayerControls controls;
    private Vector2 moveInput;

    void Awake()
    {
        controls = new PlayerControls();

        // Setup Move action
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Setup Jump action
        controls.Player.Jump.performed += ctx => Jump();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform; // Get the main camera transform
    }

    void Update()
    {
        // Calculate movement direction relative to the camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; // Ignore vertical movement
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        movement = forward * moveInput.y + right * moveInput.x;
        movement.Normalize();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        Vector3 moveVelocity = movement * moveSpeed * Time.fixedDeltaTime;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void Rotate()
    {
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Assume not grounded after jump
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Use OnCollisionStay to continuously check if grounded
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.point.y < transform.position.y - 0.1f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Ensure isGrounded is set to false when leaving any collision
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
