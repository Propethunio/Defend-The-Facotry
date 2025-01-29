using System;
using UnityEngine;

public class ResourceNode : PlacedObject {

    [field: SerializeField] public ResourcesEnum resourceType { get; private set; }
    [field: SerializeField] public int amount { get; private set; }

    public Action<ResourceNode> NodeGatheredCompletly;

    public void MineRsource() {
        amount--;

        if(amount == 0) {
            DestroySelf();
        }
    }

    public override void DestroySelf() {
        NodeGatheredCompletly?.Invoke(this);
        base.DestroySelf();
    }
}