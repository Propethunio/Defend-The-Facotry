using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UtilsClass {
    public static class MyUtils {

        public static bool IsPointerOverUI() {
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position = Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);
            Debug.Log(hits.Count > 0);
            return hits.Count > 0;
        }
    }
}