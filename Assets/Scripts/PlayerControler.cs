using UnityEngine;

public class Yahya : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 5f;

    [Header("Configuración de Cámara")]
    public float sensibilidadMouse = 2f;
    public Transform camaraJugador; 

    private CharacterController controller;
    private float rotacionVertical = 0f;

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