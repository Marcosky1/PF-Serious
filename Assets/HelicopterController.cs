using UnityEngine;

public class SimpleHelicopterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f; // Velocidad de movimiento horizontal
    public float liftSpeed = 5f;  // Velocidad de ascenso/descenso
    public float maxTiltAngle = 30f; // Ángulo máximo de inclinación en X y Z
    public float tiltSpeed = 2f;  // Velocidad de inclinación
    public float maxHeight = 50f; // Altura máxima
    public float minHeight = 0f;  // Altura mínima

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found! Please attach a Rigidbody to the helicopter.");
        }
    }

    void FixedUpdate()
    {
        HandleHorizontalMovement();
        HandleVerticalMovement();
    }

    void Update()
    {
        HandleTilting();
    }

    void HandleHorizontalMovement()
    {
        // Obtener entrada de las teclas WASD
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W)) verticalInput = 1f;    // Adelante
        if (Input.GetKey(KeyCode.S)) verticalInput = -1f;   // Atrás
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f; // Izquierda
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;  // Derecha

        // Calcular el movimiento horizontal
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));
    }

    void HandleVerticalMovement()
    {
        Vector3 velocity = rb.velocity;

        // Subir con tecla O
        if (Input.GetKey(KeyCode.O) && transform.position.y < maxHeight)
        {
            velocity.y = liftSpeed;
        }
        // Bajar con tecla L
        else if (Input.GetKey(KeyCode.L) && transform.position.y > minHeight)
        {
            velocity.y = -liftSpeed;
        }
        else
        {
            velocity.y = 0f; // Estabilizar verticalmente si no hay entrada
        }

        rb.velocity = velocity;
    }

    void HandleTilting()
    {
        float tiltX = 0f;
        float tiltZ = 0f;

        // Inclinar en Z basado en movimiento hacia adelante/atrás
        if (Input.GetKey(KeyCode.W)) tiltX = -maxTiltAngle; // Inclinar hacia adelante
        if (Input.GetKey(KeyCode.S)) tiltX = maxTiltAngle;  // Inclinar hacia atrás

        // Inclinar en X basado en movimiento lateral
        if (Input.GetKey(KeyCode.A)) tiltZ = maxTiltAngle;  // Inclinar a la izquierda
        if (Input.GetKey(KeyCode.D)) tiltZ = -maxTiltAngle; // Inclinar a la derecha

        // Interpolar suavemente hacia la inclinación objetivo
        Quaternion targetRotation = Quaternion.Euler(tiltX, transform.eulerAngles.y, tiltZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}
