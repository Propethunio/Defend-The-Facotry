using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Buildable Objects/Gathering Machine")]
public class GatheringMachineSO : BaseMachineSO {

    [field: SerializeField] public ResourcesEnum gatheredResource { get; private set; }
    [field: SerializeField] public float resourceSearchRange { get; private set; }
    [field: SerializeField] public int ticksForGather { get; private set; }
    [field: SerializeField] public ItemSO producedItem { get; private set; }
}