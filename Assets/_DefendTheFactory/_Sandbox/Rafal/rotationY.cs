using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed = 50.0f; // Prêdkoœæ obrotu

    void Update()
    {
        // Obraca obiekt wokó³ osi Y
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
