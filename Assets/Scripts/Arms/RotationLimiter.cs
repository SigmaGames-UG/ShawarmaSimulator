using UnityEngine;

public class RotationLimit : MonoBehaviour
{
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public float speed = 100f;

    float angle;

    void Update()
    {
        float input = Input.GetAxis("Vertical");
        angle += input * speed * Time.deltaTime;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
}