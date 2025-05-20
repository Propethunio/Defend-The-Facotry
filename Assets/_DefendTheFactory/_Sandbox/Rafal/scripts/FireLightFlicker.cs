//using UnityEngine;

//public class FireLightFlicker : MonoBehaviour
//{
//    private Light fireLight;
//    private float baseIntensity;
//    public float flickerAmount = 0.3f;
//    public float flickerSpeed = 5f;

//    void Start()
//    {
//        fireLight = GetComponent<Light>();
//        baseIntensity = fireLight.intensity;
//    }

//    void Update()
//    {
//        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
//        fireLight.intensity = baseIntensity + noise * flickerAmount;
//    }
//}
using UnityEngine;

public class FireLightFlicker : MonoBehaviour
{
    private Light fireLight;
    private float baseIntensity;
    private float baseRange;

    public float flickerAmount = 0.3f;
    public float flickerSpeed = 5f;
    public float rangeFlickerAmount = 0.8f; // Now range flickers more visibly

    void Start()
    {
        fireLight = GetComponent<Light>();
        baseIntensity = fireLight.intensity;
        baseRange = fireLight.range;
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        fireLight.intensity = baseIntensity + noise * flickerAmount;
        fireLight.range = baseRange + noise * rangeFlickerAmount;
    }
}

