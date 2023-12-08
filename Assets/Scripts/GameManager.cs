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
	GameOver
}

public class GameManager : MonoBehaviour
{
	// Singleton
	public static GameManager Instance { get; private set; }

	// Menu canvas
	public GameObject menuCanvas;

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
			Debug.Log("GameManager created");
			Instance = this;
		}

		menuCanvas.SetActive(true);
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
				if (cellGrid.player.AnimateMove())
				{
					cellGrid.player.ResetMoveAnimation();
					foreach (Cell killedEnemyCell in cellGrid.player.GetNextKillsCells())
					{
						cellGrid.enemies.Remove((Enemy)killedEnemyCell.entity);
						killedEnemyCell.KillEntity();
					}
					cellGrid.player.MakeMove();
					foreach (Enemy enemy in cellGrid.enemies)
					{
						enemy.UpdateNextAttack();
					}
					gameState = GameState.AnimateEnemyAttacks;
				}
				break;
			case GameState.AnimateEnemyAttacks:
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

					foreach (Enemy enemy in cellGrid.enemies)
					{
						if (enemy.AttackPlayer())
						{
							gameInfo.TakeDamage();
						}
					}

					gameState = gameInfo.IsPlayerDead() ? GameState.GameOver : GameState.EnemyTurn;
				}
				break;
			case GameState.EnemyTurn:
				// Add a Delay
				Enemy.alreadyTargetedCells.Clear();
				foreach (Enemy enemy in cellGrid.enemies)
				{
					enemy.FindNextMove(cellGrid.player.GetCurrentCell());
				}
				gameState = GameState.AnimateAndMoveEnemies;
				break;
			case GameState.AnimateAndMoveEnemies:
				// Make the animation and make the move
				bool finishedMoving = true;
				foreach (Enemy enemy in cellGrid.enemies)
				{
					bool finishedAnimation = enemy.AnimateMove();
					finishedMoving = finishedMoving && finishedAnimation;
				}
				if (finishedMoving)
				{
					foreach (Enemy enemy in cellGrid.enemies)
					{
						enemy.ResetMoveAnimation();
						enemy.MakeMove();
					}
					gameState = GameState.PlayerTurn;
				}
				break;
			case GameState.GameOver:
				// Show gameover canvas with score and play again
				menuCanvas.SetActive(true);
				gameState = GameState.Menu;
				break;
		}
	}

	private void ResetGame()
	{
		gameInfo.ResetGameInfo();
		cellGrid.ResetGrid();
		cellGrid.FillGrid(level: 1);
	}

	/// <summary>
	/// Menu -> PlayerTurn
	/// </summary>
	public void OnPlayButtonClicked()
	{
		ResetGame();
		menuCanvas.SetActive(false);
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
			// If elevator is focused, there are no enemies and there is a path to the elevator
			if (focusedCell != null && focusedCell.GetStructureType() == StructureType.Elevator)
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
		else
		{
			HandleClickOnEntity();
		}
	}

	/// <summary>
	/// May exit PlayerTurn GameState if player makes a valid move
	/// </summary>
	public void HandleClickOnFloor()
	{
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
			bool validMove = player.GetMoveableCells().Contains(targetCell);
			if (validMove)
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
		// Click on elevator or incubator
		else
		{
			Debug.Log("Click on elevator or incubator");
			// Highlight path to target cell
			List<Cell> path = CellGrid.FindPath(cellGrid.player.GetCurrentCell(), targetCell, true);
			Debug.Log("Path length: " + path.Count);
			foreach (Cell cell in path)
			{
				// Light blue
				cell.Highlight(Color.cyan);
				cellGrid.highlightedPathCells.Add(cell);
			}
		}

		focusedCell = targetCell;
		targetCell = null;
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

}