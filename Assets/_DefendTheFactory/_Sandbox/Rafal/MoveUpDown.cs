using UnityEngine;
using System.Collections;

public class MoveUpDown : MonoBehaviour
{
    public float height = 2.0f; // Maksymalna wysokoœæ przemieszczenia
    public float speed = 1.0f; // Prêdkoœæ przemieszczenia
    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingUp = true;

    void Start()
    {
        startPos = transform.position;
        endPos = new Vector3(startPos.x, startPos.y + height, startPos.z);
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        while (true)
        {
            // Przemieszczaj obiekt
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, movingUp ? endPos : startPos, step);

            // SprawdŸ, czy osi¹gniêto skrajne po³o¿enie
            if (Vector3.Distance(transform.position, movingUp ? endPos : startPos) < 0.001f)
            {
                // Zmieñ kierunek ruchu
                movingUp = !movingUp;
                // Czekaj pó³ sekundy
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }
    }
}

