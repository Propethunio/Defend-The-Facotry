using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : PlacedObject {

    [SerializeField] private ItemSO itemScriptableObject;


    protected override void Setup() {
        //Debug.Log("ResourceNode.Setup()");
    }

    public ItemSO GetItemScriptableObject() {
        return itemScriptableObject;
    }

}
