using UnityEngine;

public class SimulatorPlatform : MonoBehaviour
{
    public Transform MotorA;
    public Transform MotorB;
    public Transform MotorC;
    public Transform vehicle; 
    public float limit = 20f; 

    void Update()
    {
        Vector3 vehicleRotation = vehicle.localEulerAngles;

        ControlMotor(MotorA, vehicleRotation.x);
        ControlMotor(MotorB, vehicleRotation.y); 
        ControlMotor(MotorC, vehicleRotation.z); 
    }

    void ControlMotor(Transform motor, float rotationValue)
    {
        float clampedRotation = Mathf.Clamp(rotationValue, -limit, limit);

        motor.localRotation = Quaternion.Euler(clampedRotation, 0f, 0f);
    }
}

