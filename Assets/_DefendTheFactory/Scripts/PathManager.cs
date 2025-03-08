using System;
using UnityEngine;

public class PathManager : MonoBehaviour {
    public static PathManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GeneratePath(int width, int height) {
        
    }
}