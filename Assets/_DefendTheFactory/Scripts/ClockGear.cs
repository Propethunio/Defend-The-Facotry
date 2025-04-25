using UnityEngine;
using UnityEngine.UI;

public class ClockGear : MonoBehaviour {
	[SerializeField] private Image highlight;

	public void ToggleHighlight(bool isActive) {
		highlight.enabled = isActive;
	}
}