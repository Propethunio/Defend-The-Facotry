using UnityEngine;
using UnityEngine.VFX;

public class HitVFXLooper : MonoBehaviour
{
    public VisualEffect vfx;
    public string eventName = "OnPlay";
    public float interval = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            vfx.SendEvent(eventName);
        }
    }
}
