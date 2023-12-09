using UnityEngine;

public class InfoManager : MonoBehaviour
{
	private GameInfo gameInfo;

	[SerializeField] private TMPro.TextMeshProUGUI heartsText;

	[SerializeField] private GameObject headContainer;

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
		playerHearts = Mathf.Max(0, playerHearts);
		heartsText.text = "= " + playerHearts.ToString();
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
