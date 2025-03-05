using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UtilsClass {
    public static class MyUtils {

        public static bool IsPointerOverUI() {
            PointerEventData pe = new(EventSystem.current) {
                position = Input.mousePosition
            };
            List<RaycastResult> hits = new();
            EventSystem.current.RaycastAll(pe, hits);
            return hits.Count > 0;
        }

        public static float GetAngleFromVectorFloat3D(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if(n < 0) n += 360;

            return n;
        }
    }
}