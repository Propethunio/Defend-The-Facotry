using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour {
    public void SetItem(PlacedObjectTypeSO test) {
        BuildingSystem.Instance.Test(test);
    }
}