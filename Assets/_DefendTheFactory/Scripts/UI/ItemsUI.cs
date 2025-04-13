using UnityEngine;

public class ItemsUI : MonoBehaviour {
    [SerializeField] private SingleItemUI itemPrefab;

    private void Start() {
        Injector.Resolve<ItemsManager>().ItemCreated += OnItemCreated;
    }

    private void OnDestroy() {
        Injector.Resolve<ItemsManager>().ItemCreated -= OnItemCreated;
    }

    private void OnItemCreated(ItemSO item) {
        Instantiate(itemPrefab, transform).Init(item);
    }
}