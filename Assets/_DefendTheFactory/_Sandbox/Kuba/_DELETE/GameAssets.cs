using UnityEngine;

public class GameAssets : MonoBehaviour {
	private static GameAssets _i;

	public ItemSO_Refs itemSO_Refs;
	public Transform pfBeltDebugVisualNode;
	public Transform pfBeltDebugVisualLine;

	[System.Serializable]
	public class ItemSO_Refs {

		public ItemSO ironOre;
	}

	public static GameAssets i {
		get {
			if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
			return _i;
		}
	}

	private void Awake() {
		_i = this;
	}
}