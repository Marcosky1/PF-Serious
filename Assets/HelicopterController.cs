using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [Header("Helicopter Settings")]
    [Tooltip("Speed at which the helicopter moves up and down.")]
    public float liftSpeed = 5f;

    [Tooltip("Speed at which the helicopter moves horizontally.")]
    public float moveSpeed = 10f;

    [Tooltip("Maximum height the helicopter can reach.")]
    public float maxHeight = 50f;

    [Tooltip("Minimum height the helicopter can reach.")]
    public float minHeight = 0f;

    [Tooltip("Smooth rotation speed for tilting effects.")]
    public float tiltSpeed = 2f;

    [Header("Input Keys")]
    [Tooltip("Input key to ascend.")]
    public KeyCode ascendKey = KeyCode.W;

    [Tooltip("Input key to descend.")]
    public KeyCode descendKey = KeyCode.S;

    private Rigidbody rb;

    private Quaternion fixedRotation;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found! Please attach a Rigidbody to the helicopter.");
        }

        // Store the fixed rotation with X set to 270
        fixedRotation = Quaternion.Euler(270f, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void FixedUpdate()
    {
        HandleVerticalMovement();
        HandleHorizontalMovement();
        HandleTilting();
    }

    void LateUpdate()
    {
        // Ensure the rotation on the X axis is always 270 degrees
        transform.rotation = Quaternion.Euler(270f, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void HandleVerticalMovement()
    {
        // Get current position
        Vector3 currentPosition = transform.position;

        // Ascend
        if (Input.GetKey(ascendKey) && currentPosition.y < maxHeight)
        {
            rb.velocity = new Vector3(rb.velocity.x, liftSpeed, rb.velocity.z);
        }
        // Descend
        else if (Input.GetKey(descendKey) && currentPosition.y > minHeight)
        {
            rb.velocity = new Vector3(rb.velocity.x, -liftSpeed, rb.velocity.z);
        }
        // Stabilize vertical velocity if no input
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void HandleHorizontalMovement()
    {
        // Get horizontal inputs (arrow keys or WASD)
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float verticalInput = Input.GetAxis("Vertical");   // W/S or Up/Down arrows

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed * Time.deltaTime;

        // Apply the movement
        rb.MovePosition(transform.position + transform.TransformDirection(movement));
    }

    void HandleTilting()
    {
        // Calculate tilt based on input
        float tiltZ = Input.GetAxis("Horizontal") * 10f; // Left/Right tilt
        float tiltY = Input.GetAxis("Vertical") * -10f;  // Forward/Backward tilt

        // Smoothly interpolate to the target tilt while keeping the fixed rotation
        Quaternion targetRotation = Quaternion.Euler(270f, transform.eulerAngles.y + tiltY, tiltZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}

