﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapVisual : MonoBehaviour {

    public static TilemapVisual Instance { get; private set; }

    [System.Serializable]
    public struct TilemapSpriteUV {
        public TilemapSprite tilemapSprite;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    struct UVCoords {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private TilemapSpriteUV[] tilemapSpriteUVArray = null;

    public Grid<TilemapCell> grid { get; private set; }

    bool updateMesh;
    Mesh mesh;
    Dictionary<TilemapSprite, UVCoords> uvCoordsDictionary;

    void Awake() {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = texture.width;
        float textureHeight = texture.height;

        uvCoordsDictionary = new Dictionary<TilemapSprite, UVCoords>();

        foreach(TilemapSpriteUV tilemapSpriteUV in tilemapSpriteUVArray) {
            uvCoordsDictionary[tilemapSpriteUV.tilemapSprite] = new UVCoords {
                uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
                uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight),
            };
        }
    }

    void LateUpdate() {
        if(updateMesh) {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    public void Init(int width, int height) {
        grid = new Grid<TilemapCell>(width, height, (Grid<TilemapCell> g, int x, int y) => new TilemapCell(x, y));
        grid.OnGridObjectChanged += Grid_OnGridValueChanged;
        UpdateHeatMapVisual();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void SetTilemapSprite(Vector3 worldPosition, TilemapSprite tilemapSprite) {

        int x = Mathf.FloorToInt(worldPosition.x);
        int z = Mathf.FloorToInt(worldPosition.z);

        TilemapCell tilemapCell = grid.gridArray[x, z];
        if(tilemapCell != null) {
            tilemapCell.SetTilemapSprite(tilemapSprite);
        }
    }

    void Grid_OnGridValueChanged(object sender, Grid<TilemapCell>.OnGridObjectChangedEventArgs e) {
        updateMesh = true;
    }

    void UpdateHeatMapVisual() {
        MeshUtils.CreateEmptyMeshArrays(grid.width * grid.height, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for(int x = 0; x < grid.width; x++) {
            for(int y = 0; y < grid.height; y++) {
                int index = x * grid.height + y;
                Vector3 quadSize = new Vector3(1, 1);
                TilemapCell gridObject = grid.gridArray[x, y];
                TilemapSprite tilemapSprite = gridObject.tilemapSprite;
                Vector2 gridUV00, gridUV11;

                if(tilemapSprite == TilemapSprite.None) {
                    gridUV00 = Vector2.zero;
                    gridUV11 = Vector2.zero;
                    quadSize = Vector3.zero;
                } else {
                    UVCoords uvCoords = uvCoordsDictionary[tilemapSprite];
                    gridUV00 = uvCoords.uv00;
                    gridUV11 = uvCoords.uv11;
                }
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, new Vector3(x, 0, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}