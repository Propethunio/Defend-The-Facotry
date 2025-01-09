using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : PlacedObject {

    [field: SerializeField] public ResourcesEnum ResourceType { get; private set; }
}