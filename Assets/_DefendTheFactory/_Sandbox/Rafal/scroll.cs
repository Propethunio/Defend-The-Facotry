using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll : MonoBehaviour
{
    public float scrollSpeedX;
    public float scrollSpeedY;
    public Material targetMaterial; // Materia�, kt�ry b�dzie przesuwany

    // Update jest wywo�ywane raz na klatk�
    private void Update()
    {
        if (targetMaterial != null) // Sprawd�, czy materia� jest przypisany
        {
            targetMaterial.mainTextureOffset = new Vector2(Time.realtimeSinceStartup * scrollSpeedX, Time.realtimeSinceStartup * scrollSpeedY / 2);
        }
    }
}
