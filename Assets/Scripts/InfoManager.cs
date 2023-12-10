using UnityEngine;

public class InfoManager : MonoBehaviour
{
	private GameInfo gameInfo;

	[SerializeField] private GameObject heartContainer;
	[SerializeField] private GameObject headContainer;

	[SerializeField] private GameObject heartPrefab;

	public void Awake()
	{
		gameInfo = GetComponent<GameInfo>();
	}

	public void OnEnable()
	{
		gameInfo.OnResetInfo += ResetInfo;
		gameInfo.OnHeartChange += UpdateHearts;
		gameInfo.OnEnemyKilled += UpdateKills;
		gameInfo.OnUpdateScore += UpdateScore;
	}

	public void OnDisable()
	{
		gameInfo.OnResetInfo -= ResetInfo;
		gameInfo.OnHeartChange -= UpdateHearts;
		gameInfo.OnEnemyKilled -= UpdateKills;
		gameInfo.OnUpdateScore -= UpdateScore;
	}

	private void ResetInfo()
	{
		UpdateHearts(GameParams.STARTING_PLAYER_HEARTS);
		UpdateKills(null, 0);
	}

	private void UpdateHearts(int playerHearts)
	{
		foreach (Transform child in heartContainer.transform)
		{
			Destroy(child.gameObject);
		}

		for (int i = 0; i < playerHearts; i++)
		{
			GameObject heart = Instantiate(heartPrefab, Vector3.right * i * 40, Quaternion.identity);
			heart.transform.SetParent(heartContainer.transform, false);
		}
	}

	private void UpdateKills(GameObject headPrefab, int nKills)
	{
		if (nKills == 0)
		{
			foreach (Transform child in headContainer.transform)
			{
				Destroy(child.gameObject);
			}
			return;
		}

		int MAX_HEADS = 8;
		int SEPARATION = 15;
		int HEIGHT = 35;
		int x = ((nKills - 1) % MAX_HEADS) * SEPARATION;
		int y = ((nKills - 1) / MAX_HEADS) * HEIGHT;
		GameObject head = Instantiate(headPrefab, new Vector3(x, -y, 0), Quaternion.identity);
		head.transform.SetParent(headContainer.transform, false);
	}

	private void UpdateScore(int score)
	{
		Debug.Log("UpdateScore");
	}

}
