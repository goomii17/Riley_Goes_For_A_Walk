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
		Debug.Log("Subscribe");
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
		Debug.Log("ResetInfo");
		UpdateHearts(GameParams.STARTING_PLAYER_HEARTS);
		UpdateKills(null, 0);
	}

	private void UpdateHearts(int playerHearts)
	{
		Debug.Log("PlayerHearts: " + playerHearts);
		//playerHearts = Mathf.Max(0, playerHearts);
		//heartsText.text = "= " + playerHearts.ToString();
		if (playerHearts == 0)
		{
			foreach (Transform child in headContainer.transform)
			{
				Destroy(child.gameObject);
			}
			return;
		}

		Debug.Log("UpdateHearts");
		for (int i = 0; i < playerHearts; i++)
		{
			GameObject heart = Instantiate(heartPrefab, Vector3.right * i * 40, Quaternion.identity);
			heart.transform.SetParent(heartContainer.transform, false);
			Debug.Log("Heart " + i);
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
		GameObject head = Instantiate(headPrefab, Vector3.right * (nKills - 1) * 12, Quaternion.identity);
		head.transform.SetParent(headContainer.transform, false);
	}

	private void UpdateScore(int score)
	{
		Debug.Log("UpdateScore");
	}

}
