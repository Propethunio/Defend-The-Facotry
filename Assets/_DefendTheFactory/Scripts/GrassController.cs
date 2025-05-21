using UnityEngine;

public class GrassController : MonoBehaviour {
	[SerializeField] private Vector2Int grassAmountRange;
	[SerializeField] private GameObject[] grassPrefabs;
	[SerializeField] private Vector2 randomScaleRange;

	private const float randomOffset = 0.45f;

	private void Start() {
		SpawnGrassOnTile();
	}

	private void SpawnGrassOnTile() {
		int grassAmount = Random.Range(grassAmountRange.x, grassAmountRange.y + 1);

		for (int i = 0; i < grassAmount; i++) {
			Vector3 finalPos = transform.position + new Vector3(Random.Range(-randomOffset, randomOffset), 0f, Random.Range(-randomOffset, randomOffset));
			Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
			GameObject grass = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Length)], finalPos, rotation, transform);
			float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
			grass.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
		}
	}
}