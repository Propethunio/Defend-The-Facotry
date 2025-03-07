using UnityEngine;
using System.Collections;

public class MoveUpDown : MonoBehaviour
{
    public float height = 2.0f; // Maksymalna wysoko�� przemieszczenia
    public float speed = 1.0f; // Pr�dko�� przemieszczenia
    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingUp = true;

    private void Start()
    {
        startPos = transform.position;
        endPos = new Vector3(startPos.x, startPos.y + height, startPos.z);
        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        while (true)
        {
            // Przemieszczaj obiekt
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, movingUp ? endPos : startPos, step);

            // Sprawd�, czy osi�gni�to skrajne po�o�enie
            if (Vector3.Distance(transform.position, movingUp ? endPos : startPos) < 0.001f)
            {
                // Zmie� kierunek ruchu
                movingUp = !movingUp;
                // Czekaj p� sekundy
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }
    }
}

