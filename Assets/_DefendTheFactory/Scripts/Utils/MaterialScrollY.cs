using UnityEngine;

public class MaterialScrollY : MonoBehaviour {

    [SerializeField] float scrollSpeed;
    [SerializeField] Material targetMaterial;

    float currentOffset;
    Vector2 textureOffset;

    void Start() {
        textureOffset = targetMaterial.mainTextureOffset;
    }

    void Update() {
        currentOffset += scrollSpeed * Time.deltaTime;

        if(currentOffset > 1f) {
            currentOffset -= 1f;
        } else if(currentOffset < 1f) {
            currentOffset += 1f;
        }

        textureOffset.y = currentOffset;
        targetMaterial.mainTextureOffset = textureOffset;
    }
}