using UnityEngine;


public class rotation : MonoBehaviour
{
    public float speed = 50.0f; // Pr�dko�� obrotu

    private void Update()
    {
        // Obraca obiekt wok� osi Z
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
