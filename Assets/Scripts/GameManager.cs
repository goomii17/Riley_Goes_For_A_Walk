using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	Menu,
	PlayerTurn,
	AnimateAndMovePlayer,
	AnimateEnemyAttacks,
	EnemyTurn,
	AnimateAndMoveEnemies,
	TakeElevator,
	GameOver,
	Victory
}

public class GameManager : MonoBehaviour
{
	// Singleton
	public static GameManager Instance { get; private set; }

	// Canvases
	public GameObject menuCanvas;
	public GameObject gameInfoCanvas;
	public GameObject gameOverCanvas;
	public GameObject victoryCanvas;

	// Sound player
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip menuMusic;
	[SerializeField] private AudioClip gameMusic;

	private GameState gameState;

	[SerializeField] private GameInfo gameInfo;

	public CellGrid cellGrid;

	Cell focusedCell;
	Cell targetCell;

	public void Awake()
	{
		// Singleton
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}

		menuCanvas.SetActive(true);
		gameInfoCanvas.SetActive(false);
		gameOverCanvas.SetActive(false);
		victoryCanvas.SetActive(false);

		audioSource.clip = menuMusic;
		audioSource.loop = true;
		audioSource.Play();

		gameState = GameState.Menu;
	}

	public void Update()
	{
		switch (gameState)
		{
			case GameState.Menu:
				// Animations or whatever
				break;
			case GameState.PlayerTurn:
				// Play Idle animations
				AnimateIdleEntities();
				HandlePlayerClick();
				break;
			case GameState.AnimateAndMovePlayer:
				AnimateIdleEntities();
				if (cellGrid.player.AnimateMove())
				{
					gameInfo.AddTurn();
					cellGrid.player.ResetMoveAnimation();

					// kill enemies
					foreach (Cell killedEnemyCell in cellGrid.player.GetNextKillsCells())
					{
						Enemy killedEnemy = (Enemy)killedEnemyCell.entity;
						cellGrid.enemies.Remove(killedEnemy);
						killedEnemyCell.KillEntity();
						gameInfo.AddKill(killedEnemy.headPrefab);
					}

					// Try to drain mutant incubator
					if (cellGrid.player.DrainIncubator())
					{
						gameInfo.AddHeart();
					}

					// Try to take elevator
					if (cellGrid.player.TakeElevator())
					{
						cellGrid.player.MakeMove();
						gameState = GameState.TakeElevator;
						break;
					}

					// If player cannot move, game over
					if (cellGrid.player.GetNextMoveCell() == null)
					{
						gameOverCanvas.SetActive(true);
						audioSource.clip = menuMusic;
						audioSource.loop = true;
						audioSource.Play();
						gameState = GameState.GameOver;
						break;
					}

					// Make move
					cellGrid.player.MakeMove();

					// Enemies calculate next move
					foreach (Enemy enemy in cellGrid.enemies)
					{
						enemy.UpdateNextAttack();
					}

					gameState = GameState.AnimateEnemyAttacks;
				}
				break;
			case GameState.AnimateEnemyAttacks:
				// Animate all enemies
				bool finishedAll = true;
				foreach (Enemy enemy in cellGrid.enemies)
				{
					bool finishedAnimation = enemy.AnimateAttack();
					finishedAll = finishedAll && finishedAnimation;
				}
				if (finishedAll)
				{
					foreach (Enemy enemy in cellGrid.enemies)
					{
						enemy.ResetAttackAnimation();
					}

					// Attack player
					foreach (Enemy enemy in cellGrid.enemies)
					{
						if (enemy.AttackPlayer())
						{
							gameInfo.TakeDamage();
							//gameInfo.AddHeart();
						}
					}

					if (gameInfo.IsPlayerDead())
					{
						gameOverCanvas.SetActive(true);
						audioSource.clip = menuMusic;
						audioSource.loop = true;
						audioSource.Play();
						gameState = GameState.GameOver;
						break;
					}

					gameState = GameState.EnemyTurn;
				}
				break;
			case GameState.EnemyTurn:
				// Calculate next move of enemies
				Enemy.alreadyTargetedCells.Clear();
				foreach (Enemy enemy in cellGrid.enemies)
				{
					enemy.FindNextMove(cellGrid.player.GetCurrentCell(), (float)gameInfo.GetCurrentLevel() / GameParams.NUMBERS_OF_LEVELS);
				}
				gameState = GameState.AnimateAndMoveEnemies;
				break;
			case GameState.AnimateAndMoveEnemies:
				AnimateIdleEntities();
				// Animate all enemies
				bool finishedMoving = true;
				foreach (Enemy enemy in cellGrid.enemies)
				{
					bool finishedAnimation = enemy.AnimateMove();
					finishedMoving = finishedMoving && finishedAnimation;
				}
				if (finishedMoving)
				{
					// Make move
					foreach (Enemy enemy in cellGrid.enemies)
					{
						enemy.ResetMoveAnimation();
						enemy.MakeMove();
					}
					gameState = GameState.PlayerTurn;
				}
				break;
			case GameState.TakeElevator:
				AnimateIdleEntities();
				if (cellGrid.elevator.FinishedOpening())
				{
					// If there are no more levels, victory
					if (gameInfo.GetCurrentLevel() + 1 >= GameParams.NUMBERS_OF_LEVELS)
					{
						audioSource.clip = menuMusic;
						audioSource.loop = true;
						audioSource.Play();
						gameInfoCanvas.SetActive(false);
						victoryCanvas.SetActive(true);
						gameState = GameState.Victory;
						break;
					}
					NextLevel();
					gameState = GameState.PlayerTurn;
				}
				break;
			case GameState.GameOver:
				break;
			case GameState.Victory:
				break;
		}
	}

	private void ResetGame()
	{
		gameInfo.ResetGameInfo();
		cellGrid.ResetGrid();
		cellGrid.FillGrid(level: 0);
	}

	private void NextLevel()
	{
		gameInfo.NextLevel();
		cellGrid.ResetGrid(destroyPlayer: false);
		cellGrid.FillGrid(level: gameInfo.GetCurrentLevel(), createPlayer: false);
	}

	public void OnPlayButtonClicked()
	{
		menuCanvas.SetActive(false);
		gameOverCanvas.SetActive(false);
		victoryCanvas.SetActive(false);
		gameInfoCanvas.SetActive(true);
		ResetGame();

		// Play soundtrack
		audioSource.clip = gameMusic;
		audioSource.loop = true;
		audioSource.Play();

		gameState = GameState.PlayerTurn;
	}

	public void OnCellClicked(Cell cell)
	{
		if (gameState == GameState.PlayerTurn || gameState == GameState.AnimateAndMovePlayer)
		{
			targetCell = cell;
		}
	}

	/// <summary>
	/// Transitions to AnimateAndMovePlayer GameState if player makes a valid move.
	/// Highlights cells based on click.
	/// We can click on: Void, Floor, Enemy, Player, Elevator or Incubator
	/// </summary>
	private void HandlePlayerClick()
	{
		// No click
		if (targetCell == null)
		{
			// If structure is focused, there are no enemies and there is a path, keep moving
			if (focusedCell != null && focusedCell.GetStructureType() != StructureType.None)
			{
				if (cellGrid.enemies.Count == 0 && cellGrid.highlightedPathCells.Count > 0)
				{
					// Move player
					cellGrid.player.SetNextMoveCell(cellGrid.highlightedPathCells[0]);
					cellGrid.player.UpdateNextKills(cellGrid.highlightedPathCells[0]);

					// Unhighlight previous cell
					cellGrid.highlightedPathCells[0].UnHighlight();
					cellGrid.highlightedPathCells.RemoveAt(0);

					gameState = GameState.AnimateAndMovePlayer;
				}
			}
			return;
		}

		// Click on highlightedPathCell
		if (cellGrid.highlightedPathCells.Contains(targetCell))
		{
			// Move player
			cellGrid.player.SetNextMoveCell(cellGrid.highlightedPathCells[0]);
			cellGrid.player.UpdateNextKills(cellGrid.highlightedPathCells[0]);
			cellGrid.UnHighLightAllCells();
			focusedCell = null;
			targetCell = null;
			gameState = GameState.AnimateAndMovePlayer;
			return;
		}

		// Unfocus all highlighted cells
		cellGrid.UnHighLightAllCells();

		// Click same cell or void
		if (targetCell == focusedCell || targetCell.tile.Type == TileType.Void)
		{
			focusedCell = null;
			targetCell = null;
			return;
		}

		// Click on empty floor
		if (targetCell.IsEmptyFloor())
		{
			HandleClickOnFloor();
		}
		// Click on entity
		else if (targetCell.GetEntityType() != EntityType.None)
		{
			HandleClickOnEntity();
		}
		else
		{
			HandleClickOnStructure();
		}
	}

	public void HandleClickOnFloor()
	{
		// If player can move to floor directly, move it
		if (cellGrid.player.GetMoveableCells().Contains(targetCell))
		{
			cellGrid.player.SetNextMoveCell(targetCell);
			cellGrid.player.UpdateNextKills(targetCell);
			focusedCell = null;
			targetCell = null;
			gameState = GameState.AnimateAndMovePlayer;
			return;
		}

		// Click with focus NOT on Player
		if (focusedCell == null || focusedCell.GetEntityType() != EntityType.Player)
		{
			// Highlight path to target cell
			List<Cell> path = CellGrid.FindPath(cellGrid.player.GetCurrentCell(), targetCell, false);
			foreach (Cell cell in path)
			{
				// Light blue
				cell.Highlight(Color.cyan);
				cellGrid.highlightedPathCells.Add(cell);
			}
			// Focus new cell
			focusedCell = targetCell;
			targetCell = null;
		}
		// Click with focus ON Player
		else
		{
			Player player = (Player)focusedCell.entity;

			// Make move if valid and exit PlayerTurn
			if (player.GetMoveableCells().Contains(targetCell))
			{
				player.SetNextMoveCell(targetCell);
				player.UpdateNextKills(targetCell);
				gameState = GameState.AnimateAndMovePlayer;
			}

			focusedCell = null;
			targetCell = null;
		}
	}

	public void HandleClickOnEntity()
	{
		// Click on enemy
		if (targetCell.GetEntityType() == EntityType.Enemy)
		{
			// Highlight Attackable cells
			Enemy enemy = (Enemy)targetCell.entity;
			foreach (Cell cell in enemy.GetAttackableCells())
			{
				cell.Highlight(Color.yellow);
				cellGrid.highlightedAttackCells.Add(cell);
			}
		}
		else if (targetCell.GetEntityType() == EntityType.Player)
		{
			// Highlight Moveable/Attackable cells
			foreach (Cell cell in targetCell.entity.GetMoveableCells())
			{
				cell.Highlight(Color.yellow);
				cellGrid.highlightedAttackCells.Add(cell);
			}
		}

		focusedCell = targetCell;
		targetCell = null;
	}

	private void HandleClickOnStructure()
	{
		// If player can move to structue directly
		if (cellGrid.player.GetMoveableCells().Contains(targetCell))
		{
			cellGrid.player.SetNextMoveCell(targetCell);
			// Focus new cell
			focusedCell = null;
			targetCell = null;
			gameState = GameState.AnimateAndMovePlayer;
			return;
		}

		// Click on elevator
		if (targetCell.GetStructureType() == StructureType.Elevator)
		{
			// Highlight path to target cell
			List<Cell> path = CellGrid.FindPath(cellGrid.player.GetCurrentCell(), targetCell, true);
			foreach (Cell cell in path)
			{
				// Light blue
				cell.Highlight(Color.cyan);
				cellGrid.highlightedPathCells.Add(cell);
			}
			// Add elevator to path
			targetCell.Highlight(Color.cyan);
			cellGrid.highlightedPathCells.Add(targetCell);
			// Focus new cell
			focusedCell = targetCell;
			targetCell = null;
		}
		// Click on incubator
		else if (targetCell.GetStructureType() == StructureType.Incubator)
		{
			// Highlight path to target cell
			List<Cell> path = CellGrid.FindPath(cellGrid.player.GetCurrentCell(), targetCell, true);
			foreach (Cell cell in path)
			{
				// Light blue
				cell.Highlight(Color.cyan);
				cellGrid.highlightedPathCells.Add(cell);
			}
			// Add incubator to path
			targetCell.Highlight(Color.cyan);
			cellGrid.highlightedPathCells.Add(targetCell);
			// Focus new cell
			focusedCell = targetCell;
			targetCell = null;
		}
	}

	private void AnimateIdleEntities()
	{
		// Play idle animations
		foreach (Cell cell in cellGrid.GetCells())
		{
			if (cell.entity != null)
			{
				cell.entity.AnimateIdle();
			}
		}
	}

	public void DoExitGame()
	{
		Application.Quit();
	}

}