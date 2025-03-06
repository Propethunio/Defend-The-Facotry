using UnityEngine;

public class ItemsUI : MonoBehaviour {

    [SerializeField] SingleItemUI itemPrefab;

    void Start() {
        ItemsManager.Instance.ItemCreated += OnItemCreated;
    }

    void OnDestroy() {
        ItemsManager.Instance.ItemCreated -= OnItemCreated;
    }

    void OnItemCreated(ItemSO item) {
        Instantiate(itemPrefab, transform).Init(item);
    }
}