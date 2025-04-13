using UnityEngine;

public class MouseClickPlane : DependencyMonoBehaviour<MouseClickPlane> {
    public void Setup(int x, int y) {
        transform.localScale = new Vector3(x, 0.01f, y);
        transform.position = new Vector3(x / 2f, 0, y / 2f);
    }
}