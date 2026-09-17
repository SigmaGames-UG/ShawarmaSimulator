using UnityEngine;

public class Yahya : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 5f;
    public float gravedad = -9.81f; 
    [Header("Configuración de Cámara")]
    public float sensibilidadMouse = 2f;
    public Transform camaraJugador;

    private CharacterController controller;
    private float rotacionVertical = 0f;
    private Vector3 velocidadCaida; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MoverJugador();
        RotarCamara();
    }

    void MoverJugador()
    {
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movimiento = transform.right * x + transform.forward * z;
        controller.Move(movimiento * velocidad * Time.deltaTime);

       
        if (controller.isGrounded && velocidadCaida.y < 0)
        {
            velocidadCaida.y = -30f; 
        }

        velocidadCaida.y += gravedad * Time.deltaTime;
        controller.Move(velocidadCaida * Time.deltaTime);
    }

    void RotarCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -90f, 90f);

        camaraJugador.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}