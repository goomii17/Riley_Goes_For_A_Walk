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

	[SerializeField] private GameInfo gameInfo;

	public GameState gameState;

	public CellGrid cellGrid;

	Cell focusedCell;
	Cell targetCell;

	List<Cell> highlightedPathCells = new List<Cell>();
	List<Cell> highlightedAttackCells = new List<Cell>();

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
				HandlePlayerInstruction();
				break;
			case GameState.AnimateAndMovePlayer:
				if (cellGrid.player.AnimateMove())
				{
					cellGrid.player.ResetMoveAnimation();
					foreach (Cell killedEnemyCell in cellGrid.player.GetNextKillsCells())
					{
						cellGrid.enemies.Remove((Enemy)killedEnemyCell.content);
						killedEnemyCell.KillContent();
					}
					Debug.Log("Player makes move");
					cellGrid.player.MakeMove();
					Debug.Log("Updating enemies next attack");
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
					if (!enemy.AnimateAttack())
					{
						finishedAll = false;
					}
				}

				if (finishedAll)
				{
					foreach (Enemy enemy in cellGrid.enemies)
					{
						enemy.ResetAttackAnimation();
					}
					Debug.Log("Enemy attack animations finished");
					foreach (Enemy enemy in cellGrid.enemies)
					{
						if (enemy.AttackPlayer())
						{
							gameInfo.TakeDamage();
						}
					}

					if (gameInfo.IsPlayerDead())
					{
						gameState = GameState.GameOver;
					}
					else
					{
						gameState = GameState.EnemyTurn;
					}
				}
				break;
			case GameState.EnemyTurn:
				// Add a Delay
				Enemy.alreadyTargetedCells.Clear();
				foreach (Enemy enemy in cellGrid.enemies)
				{
					enemy.FindNextMove();
				}
				gameState = GameState.AnimateAndMoveEnemies;
				break;
			case GameState.AnimateAndMoveEnemies:
				// Make the animation and make the move
				bool finishedMoving = true;
				foreach (Enemy enemy in cellGrid.enemies)
				{
					if (!enemy.AnimateMove())
					{
						finishedMoving = false;
					}
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

	public void ResetGame()
	{
		gameInfo.ResetGameInfo();
		cellGrid.ResetGrid();
		cellGrid.FillGrid(level: 1);
	}

	/// <summary>
	/// Exit Menu GameState
	/// </summary>
	public void OnPlayButtonClicked()
	{
		// Initialize the game
		ResetGame();

		// Hide menu
		menuCanvas.SetActive(false);

		// Transition state
		gameState = GameState.PlayerTurn;
	}

	public void OnCellClicked(Cell cell)
	{
		if (gameState == GameState.PlayerTurn)
		{
			targetCell = cell;
		}
	}

	private void UnHighLightAll()
	{
		foreach (Cell cell in highlightedPathCells)
		{
			cell.UnHighlightCell();

		}
		highlightedPathCells.Clear();
		foreach (Cell cell in highlightedAttackCells)
		{
			cell.UnHighlightCell();
		}
		highlightedAttackCells.Clear();
	}

	/// <summary>
	/// Highlights cells based on click.
	/// Transitions to AnimateAndMovePlayer GameState if player makes a valid move.
	/// We can click on: Void, Floor, Enemy, Player, Elevator
	/// </summary>
	public void HandlePlayerInstruction()
	{
		// No click
		if (targetCell == null)
		{
			return;
		}

		// Click on highlightedPathCell
		if (highlightedPathCells.Contains(targetCell))
		{
			Debug.Log("Clicked on highlightedPathCell");
			// Move player
			cellGrid.player.SetNextMove(highlightedPathCells[0]);
			cellGrid.player.UpdateNextKills(highlightedPathCells[0]);
			gameState = GameState.AnimateAndMovePlayer;
			UnHighLightAll();
			focusedCell = null;
			targetCell = null;
			return;
		}

		// Unfocus cell and all highlighted cells
		UnHighLightAll();

		// Click same cell or void
		if (targetCell == focusedCell || targetCell.tile.Type == TileType.Void)
		{
			focusedCell = null;
			targetCell = null;
			return;
		}

		// Click on floor
		if (targetCell.content == null)
		{
			Debug.Log("Floor clicked");
			HandleClickOnFloor();
		}
		else if (targetCell.content.Type == EntityType.Elevator)
		{
			Debug.Log("Elevator clicked");
		}
		// Click on enemy or player
		else
		{
			Debug.Log("Entity clicked");
			HandleClickOnEntity();
		}
	}

	/// <summary>
	/// May exit PlayerTurn GameState if player makes a valid move
	/// </summary>
	public void HandleClickOnFloor()
	{
		// Click with focus NOT on Player
		if (focusedCell == null || focusedCell.content == null || focusedCell.content.Type != EntityType.Player)
		{
			// Highlight path to target cell
			List<Cell> path = cellGrid.FindPath(cellGrid.player.GetCurrentCell(), targetCell);
			foreach (Cell cell in path)
			{
				// Light blue
				cell.HighlightCell(Color.cyan);
				highlightedPathCells.Add(cell);
			}
			// Focus new cell
			focusedCell = targetCell;
			targetCell = null;
			return;
		}

		// Click with focus ON Player
		Player player = (Player)focusedCell.content;

		bool validMove = player.GetMoveableCells().Contains(targetCell);
		if (validMove)
		{
			Debug.Log("Player updating next move");
			player.SetNextMove(targetCell);
			player.UpdateNextKills(targetCell);
			gameState = GameState.AnimateAndMovePlayer;
		}
		else
		{
			Debug.Log("Player could not move to cell");
		}
		focusedCell = null;
		targetCell = null;
	}

	public void HandleClickOnEntity()
	{
		Debug.Log("Clicked on type: " + targetCell.content.Type);
		// Show enemy attacks
		if (targetCell.content.Type == EntityType.Enemy)
		{
			Enemy enemy = (Enemy)targetCell.content;
			foreach (Cell cell in enemy.GetAttackableCells())
			{
				cell.HighlightCell(Color.yellow);
				highlightedAttackCells.Add(cell);
			}
		}
		else if (targetCell.content.Type == EntityType.Player)
		{
			foreach (Cell cell in targetCell.content.GetMoveableCells())
			{
				cell.HighlightCell(Color.yellow);
				highlightedAttackCells.Add(cell);
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
			if (cell.content != null)
			{
				cell.content.AnimateIdle();
			}
		}
	}

}