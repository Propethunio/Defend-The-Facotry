using UnityEngine;
using UnityEngine.UI;

public class ResourcesUI : MonoBehaviour {
	[SerializeField] private SingleResourceUI resourcePrefab;
	[SerializeField] private LayoutElement resourcesContainer;

	private ItemsManager itemsManager;
	private float additionalHeight;
	private int resourceUiCount;

	private void Start() {
		itemsManager = Injector.Resolve<ItemsManager>();
		itemsManager.ItemCreated += OnItemCreated;
		resourcesContainer.gameObject.SetActive(false);
		additionalHeight = resourcePrefab.GetComponent<RectTransform>().sizeDelta.y + resourcesContainer.GetComponent<VerticalLayoutGroup>().spacing;
	}

	private void OnDestroy() {
		itemsManager.ItemCreated -= OnItemCreated;
	}

	private void OnItemCreated(ItemSO item) {
		SingleResourceUI res = Instantiate(resourcePrefab, resourcesContainer.transform);
		res.Init(item);
		res.Removed += OnResourceRemoved;
		resourceUiCount++;
		resourcesContainer.gameObject.SetActive(true);
		IncreaseResourcesContainerSize();
	}

	private void IncreaseResourcesContainerSize() {
		resourcesContainer.minHeight += additionalHeight;
	}

	private void DecreaseResourcesContainerSize() {
		resourcesContainer.minHeight -= additionalHeight;
	}

	private void OnResourceRemoved() {
		DecreaseResourcesContainerSize();
		resourceUiCount--;
		
		if (resourceUiCount == 0) {
			resourcesContainer.gameObject.SetActive(false);
		}
	}
}