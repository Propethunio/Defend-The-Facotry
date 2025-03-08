using UnityEngine;

public class ItemsUI : MonoBehaviour {
    [SerializeField] private SingleItemUI itemPrefab;

    private void Start() {
        ItemsManager.Instance.ItemCreated += OnItemCreated;
    }

    private void OnDestroy() {
        ItemsManager.Instance.ItemCreated -= OnItemCreated;
    }

    private void OnItemCreated(ItemSO item) {
        Instantiate(itemPrefab, transform).Init(item);
    }
}