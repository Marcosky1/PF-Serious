using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    public float moveSpeed = 10f; // Velocidad de movimiento horizontal
    public float ascendSpeed = 5f; // Velocidad de ascenso y descenso
    public float rotationSpeed = 2f; // Velocidad de rotación para inclinación

    private float horizontalInput;
    private float verticalInput;
    private float ascendInput;

    void Update()
    {
        // Inputs para movimiento horizontal
        horizontalInput = Input.GetAxis("Horizontal"); // A y D
        verticalInput = Input.GetAxis("Vertical"); // W y S

        // Input para ascenso/descenso
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ascendInput = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            ascendInput = -1;
        }
        else
        {
            ascendInput = 0;
        }

        // Movimiento horizontal (adelante, atrás, izquierda, derecha)
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(moveDirection, Space.Self);

        // Movimiento vertical (ascender/descender)
        transform.Translate(Vector3.up * ascendInput * ascendSpeed * Time.deltaTime, Space.World);

        // Inclinación del helicóptero
        float tiltAngleX = verticalInput * rotationSpeed; // Inclinación adelante/atrás
        float tiltAngleZ = -horizontalInput * rotationSpeed; // Inclinación izquierda/derecha
        Quaternion targetRotation = Quaternion.Euler(tiltAngleX, transform.eulerAngles.y, tiltAngleZ);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
