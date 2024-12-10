using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class UDPSendTest : MonoBehaviour
{

    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();


    private string IP;  // define in init
    public int port;  // define in init
    public Text engineA;
    public Text engineAHex;
    public Slider sliderA;
    public Text engineB;
    public Text engineBHex;
    public Slider sliderB;
    public Text engineC;
    public Text engineCHex;
    public Slider sliderC;

    public Text Data;

    UdpClient client;

    public bool active = false;

    public float SmoothEngine = 0.5f;

    public float A = 0, B = 0, C = 0, longg;

    public Transform vehicle;

    // start from unity3d
    public void Start()
    {
        init();
    }
    public void init()
    {

        // define
        IP = "192.168.15.201";
        port = 7408;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(53342);


        // AppControlField
        mUDPDATA.mAppControlField.ConfirmCode = "55aa";
        mUDPDATA.mAppControlField.PassCode = "0000";
        mUDPDATA.mAppControlField.FunctionCode = "1301";
        // AppWhoField
        mUDPDATA.mAppWhoField.AcceptCode = "ffffffff";
        mUDPDATA.mAppWhoField.ReplyCode = "";//"00000001";
                                             // AppDataField
        mUDPDATA.mAppDataField.RelaTime = "00000064";
        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        mUDPDATA.mAppDataField.PortOut = "12345678";

        A = 100;
        B = 100;
        C = 100;

        sliderA.value = A;
        sliderB.value = B;
        sliderC.value = C;

        string HexA = DecToHexMove(A);
        string HexB = DecToHexMove(B);
        string HexC = DecToHexMove(C);

        engineAHex.text = "Engine A: " + HexA;
        engineBHex.text = "Engine B: " + HexB;
        engineCHex.text = "Engine C: " + HexC;

        mUDPDATA.mAppDataField.PlayMotorC = HexC;
        mUDPDATA.mAppDataField.PlayMotorA = HexA;
        mUDPDATA.mAppDataField.PlayMotorB = HexB;


        engineA.text = ((int)sliderA.value).ToString();
        engineB.text = ((int)sliderB.value).ToString();
        engineC.text = ((int)sliderC.value).ToString();

        Data.text = "Data: " + mUDPDATA.GetToString();

        sendString(mUDPDATA.GetToString());

        StartCoroutine(UpMovePlatform(3));
    }
    public void ActiveSend()
    {
        active = true;

    }
    public void ResertPositionEngine()
    {

        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString(mUDPDATA.GetToString());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }

    IEnumerator UpMovePlatform(float wait)
    {
        active = false;

        yield return new WaitForSeconds(3f);

        active = true;
    }
    void CalcularAltitud()
    {

        //valueMotor = 125;

        //A = (float)(Mathf.Clamp(valueMotor, 0, 250));
        //B = (float)(Mathf.Clamp(valueMotor, 0, 250));
        //C = (float)(Mathf.Clamp(valueMotor, 0, 250));

    }
    void CalcularRotacion()
    {
        // Obtener la rotación actual del objeto como ángulos de Euler
        Quaternion currentRotation = vehicle.rotation;
        Vector3 eulerAngles = currentRotation.eulerAngles;

        // Mapear los ángulos de Euler al rango de 0 a 200, con 100 como punto medio
        float servoPitch = MapToRange(eulerAngles.x, -180, 180, 0, 200);
        float servoYaw = MapToRange(eulerAngles.y, -180, 180, 0, 200);
        float servoRoll = MapToRange(eulerAngles.z, -180, 180, 0, 200);

        // Convertir los valores mapeados a hexadecimal
        string hexPitch = DecToHexMove(servoPitch);
        string hexYaw = DecToHexMove(servoYaw);
        string hexRoll = DecToHexMove(servoRoll);

        // Mostrar los valores mapeados y convertidos en consola
        Debug.Log($"Servo Pitch (Hex): {hexPitch}, Servo Yaw (Hex): {hexYaw}, Servo Roll (Hex): {hexRoll}");

        // Asignar los valores a los campos correspondientes para envío
        mUDPDATA.mAppDataField.PlayMotorA = hexPitch;
        mUDPDATA.mAppDataField.PlayMotorB = hexYaw;
        mUDPDATA.mAppDataField.PlayMotorC = hexRoll;

        // Enviar los datos
        sendString(mUDPDATA.GetToString());
    }

    // Función auxiliar para mapear valores de un rango a otro
    float MapToRange(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Clamp((value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin, toMin, toMax);
    }

    void FixedUpdate()
    {
        if (active)
        {

            CalcularRotacion();

            sliderA.value = A;
            sliderB.value = B;
            sliderC.value = C;

            string HexA = DecToHexMove(A);
            string HexB = DecToHexMove(B);
            string HexC = DecToHexMove(C);

            engineAHex.text = "Engine A: " + HexA;
            engineBHex.text = "Engine B: " + HexB;
            engineCHex.text = "Engine C: " + HexC;

            mUDPDATA.mAppDataField.PlayMotorC = HexC;
            mUDPDATA.mAppDataField.PlayMotorA = HexA;
            mUDPDATA.mAppDataField.PlayMotorB = HexB;


            engineA.text = ((int)sliderA.value).ToString();
            engineB.text = ((int)sliderB.value).ToString();
            engineC.text = ((int)sliderC.value).ToString();

            Data.text = "Data: " + mUDPDATA.GetToString();

            sendString(mUDPDATA.GetToString());
        }
    }

    void OnApplicationQuit()
    {

        ResertPositionEngine();



        if (client != null)
            client.Close();
        Application.Quit();
    }

    byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    string DecToHexMove(float num)
    {
        int d = (int)((num / 5f) * 10000f);
        return "000" + d.ToString("X");
    }

    private void sendString(string message)
    {

        try
        {
            // Bytes empfangen.
            if (message != "")
            {

                //byte[] data = StringToByteArray(message);
                print(message);
                // Den message zum Remote-Client senden.
                //client.Send(data, data.Length, remoteEndPoint);

            }


        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    void OnDisable()
    {

        if (client != null)
            client.Close();
    }

    private void OnDrawGizmos()
    {

        Quaternion rotY = vehicle.rotation;
        rotY.x = 0;
        rotY.z = 0;

        // rotate left or Right
        Vector3 Vb1 = vehicle.position + vehicle.rotation * Vector3.right * longg;
        Vector3 Vb2 = vehicle.position + rotY * Vector3.right * longg;

        // rotate forward or back
        Vector3 VbA1 = vehicle.position + vehicle.rotation * Vector3.forward * longg;
        Vector3 VbA2 = vehicle.position + rotY * Vector3.forward * longg;


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vb1, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, Vb1);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Vb2, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, Vb2);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(VbA1, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, VbA1);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(VbA2, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, VbA2);

    }

}