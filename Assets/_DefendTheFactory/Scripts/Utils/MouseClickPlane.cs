using UnityEngine;

public class MouseClickPlane : MonoBehaviour {

    public static MouseClickPlane Instance;

    void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Setup(int x, int y) {
        transform.localScale = new Vector3(x, 0.01f, y);
        transform.position = new Vector3(x / 2f, 0f, y / 2f);
    }
}