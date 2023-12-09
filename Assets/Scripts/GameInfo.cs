using System;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
	private int currentLevel;
	private int playerHearts;
	private int enemyKills;
	private int turnsTaken;
	private int currentScore;

	public Action OnResetInfo;
	public Action<int> OnHeartChange;
	public Action<GameObject, int> OnEnemyKilled;
	public Action<int> OnUpdateScore;

	public void Awake()
	{
		ResetGameInfo();
	}

	public void ResetGameInfo()
	{
		currentLevel = 0;
		playerHearts = GameParams.STARTING_PLAYER_HEARTS;
		enemyKills = 0;
		currentScore = 0;
		OnResetInfo?.Invoke();
	}

	public void NextLevel()
	{
		currentLevel++;
	}

	public int GetCurrentLevel()
	{
		return currentLevel;
	}

	public void TakeDamage()
	{
		playerHearts--;
		OnHeartChange?.Invoke(playerHearts);
	}

	public void AddHeart()
	{
		playerHearts++;
		OnHeartChange?.Invoke(playerHearts);
	}

	public void AddKill(GameObject enemyHeadPrefab)
	{
		enemyKills++;
		OnEnemyKilled?.Invoke(enemyHeadPrefab, enemyKills);
	}

	public void AddTurn()
	{
		turnsTaken++;
	}

	public void UpdateScore()
	{
		currentScore = currentLevel * 100 + enemyKills * 10 + playerHearts * 10;
		OnUpdateScore?.Invoke(currentScore);
	}

	public bool IsPlayerDead()
	{
		return playerHearts <= 0;
	}

}
