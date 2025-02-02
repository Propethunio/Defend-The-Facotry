using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll : MonoBehaviour
{
    public float scrollSpeedX;
    public float scrollSpeedY;
    public Material targetMaterial; // Materia³, który bêdzie przesuwany

    // Update jest wywo³ywane raz na klatkê
    void Update()
    {
        if (targetMaterial != null) // SprawdŸ, czy materia³ jest przypisany
        {
            targetMaterial.mainTextureOffset = new Vector2(Time.realtimeSinceStartup * scrollSpeedX, Time.realtimeSinceStartup * scrollSpeedY / 2);
        }
    }
}
