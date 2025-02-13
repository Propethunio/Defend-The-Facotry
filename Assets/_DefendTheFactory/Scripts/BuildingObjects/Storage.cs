using System;
using UnityEngine;

public class Storage : BaseDataPlacedObject<BaseMachineSO> {

    public event EventHandler OnItemStorageCountChanged;

    private ItemStackList itemStackList;

    public override void Initialize(Vector2Int origin, BuildingDir dir, BaseMachineSO buildableDataSO) {
        BaseDataSet(origin, dir, buildableDataSO);
    }

    protected override void Setup() {
        itemStackList = new ItemStackList();
    }

    public ItemStackList GetItemStackList() {
        return itemStackList;
    }
}