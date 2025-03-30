using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectOutline : MonoBehaviour {
    [SerializeField, DrawWithUnity] private RenderingLayerMask outlineLayer;

    private Renderer[] renderers;
    private uint originalLayer;
    private bool isOutlineActive;

    private void Start() {
        renderers = TryGetComponent<Renderer>(out var meshRenderer) ? new[] { meshRenderer } : GetComponentsInChildren<Renderer>();
        originalLayer = renderers[0].renderingLayerMask;
    }

    public void SetOutline(bool enable) {
        int rendererCount = renderers.Length;

        for (var i = 0; i < rendererCount; i++) {
            var rend = renderers[i];
            if (rend == null) continue;

            rend.renderingLayerMask = enable ? originalLayer | outlineLayer : originalLayer;
        }
    }
}