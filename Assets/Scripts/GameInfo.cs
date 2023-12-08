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
	public Action<int> OnTakeDamage;
	public Action<Enemy, int> OnEnemyKilled;
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

	public void TakeDamage()
	{
		playerHearts--;
		OnTakeDamage?.Invoke(playerHearts);
	}

	public void AddKill(Enemy killedEnemy)
	{
		enemyKills++;
		OnEnemyKilled?.Invoke(killedEnemy, enemyKills);
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
