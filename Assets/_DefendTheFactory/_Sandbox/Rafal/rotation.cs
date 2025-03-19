using UnityEngine;


public class rotation : MonoBehaviour
{
    public float speed = 50.0f; // Prêdkoœæ obrotu

    void Update()
    {
        // Obraca obiekt wokó³ osi Z
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
