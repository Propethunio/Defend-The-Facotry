using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed = 50.0f; // Pr�dko�� obrotu

    private void Update()
    {
        // Obraca obiekt wok� osi Y
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
