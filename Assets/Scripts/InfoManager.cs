using UnityEngine;

public class InfoManager : MonoBehaviour
{
	private GameInfo gameInfo;

	[SerializeField] private TMPro.TextMeshProUGUI heartsText;

	public void Awake()
	{
		gameInfo = GetComponent<GameInfo>();
	}

	public void OnEnable()
	{
		gameInfo.OnResetInfo += ResetInfo;
		gameInfo.OnTakeDamage += UpdateHearts;
		gameInfo.OnEnemyKilled += UpdateKills;
		gameInfo.OnUpdateScore += UpdateScore;
	}

	public void OnDisable()
	{
		gameInfo.OnResetInfo -= ResetInfo;
		gameInfo.OnTakeDamage -= UpdateHearts;
		gameInfo.OnEnemyKilled -= UpdateKills;
		gameInfo.OnUpdateScore -= UpdateScore;
	}

	private void ResetInfo()
	{
		UpdateHearts(GameParams.STARTING_PLAYER_HEARTS);
		UpdateKills(null);
	}

	private void UpdateHearts(int playerHearts)
	{
		Debug.Log("UpdateHearts");
		playerHearts = Mathf.Clamp(playerHearts, 0, GameParams.STARTING_PLAYER_HEARTS);
		heartsText.text = "= " + playerHearts.ToString();
	}

	private void UpdateKills(Enemy enemy)
	{
		Debug.Log("UpdateKills");
	}

	private void UpdateScore(int score)
	{
		Debug.Log("UpdateScore");
	}

}
