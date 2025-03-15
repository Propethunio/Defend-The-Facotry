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
    }
}