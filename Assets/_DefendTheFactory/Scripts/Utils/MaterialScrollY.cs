using UnityEngine;

public class MaterialScrollY : MonoBehaviour {
    [SerializeField] private float scrollSpeed;
    [SerializeField] private Material targetMaterial;

    private float currentOffset;
    private Vector2 textureOffset;

    private void Start() {
        textureOffset = targetMaterial.mainTextureOffset;
    }

    private void Update() {
        currentOffset += scrollSpeed * Time.deltaTime;

        if (currentOffset > 1f) {
            currentOffset -= 1f;
        }
        else if (currentOffset < 1f) {
            currentOffset += 1f;
        }

        textureOffset.y = currentOffset;
        targetMaterial.mainTextureOffset = textureOffset;
    }

    private void OnDestroy() {
        targetMaterial.mainTextureOffset = Vector2.zero;
    }
}