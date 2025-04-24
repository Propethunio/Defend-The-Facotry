using UnityEngine;
using UnityEngine.UI;

public class ResourcesUI : MonoBehaviour {
	[SerializeField] private SingleResourceUI resourcePrefab;
	[SerializeField] private LayoutElement resourcesContainer;

	private float additionalHeight;

	private void Start() {
		Injector.Resolve<ItemsManager>().ItemCreated += OnItemCreated;
		resourcesContainer.gameObject.SetActive(false);
		additionalHeight = resourcePrefab.GetComponent<RectTransform>().sizeDelta.y + resourcesContainer.GetComponent<VerticalLayoutGroup>().spacing;
	}

	private void OnDestroy() {
		Injector.Resolve<ItemsManager>().ItemCreated -= OnItemCreated;
	}

	private void OnItemCreated(ItemSO item) {
		Instantiate(resourcePrefab, resourcesContainer.transform).Init(item);
		resourcesContainer.gameObject.SetActive(true);
		CalculateResourcesContainerSize();
	}

	private void CalculateResourcesContainerSize() {
		resourcesContainer.minHeight += additionalHeight;
	}
}