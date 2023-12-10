using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
	[Header("Prefabs")]
	public GameObject cellPrefab;
	public GameObject voidPrefab;
	public GameObject floorPrefab;
	public GameObject elevatorPrefab;
	public GameObject incubatorPrefab;
	public GameObject playerPrefab;
	public GameObject[] enemyPrefabs;

	// Grid of cells
	private GameObject[,] grid;

	// Player and enemies
	public Player player;
	public Elevator elevator;
	public List<Enemy> enemies = new List<Enemy>();

	// Highlighted cells
	public List<Cell> highlightedPathCells = new List<Cell>();
	public List<Cell> highlightedAttackCells = new List<Cell>();

	public void Awake()
	{
		DestroyChildren();
		InitGrid();
		SetNeighbors();
		//ResetGrid();
		//FillGrid();
	}

	public static bool IsInGrid(int i, int j)
	{
		int start = Mathf.Max(i + 1 - GameParams.GRID_HEIGHT, 0);
		int end = Mathf.Min(GameParams.GRID_WIDTH + i, GameParams.MATRIX_WIDTH);
		return i >= 0 && i < GameParams.MATRIX_HEIGHT && j >= start && j < end;
	}

	public List<Cell> GetCells()
	{
		List<Cell> cells = new List<Cell>();
		foreach (Transform child in transform)
		{
			cells.Add(child.GetComponent<Cell>());
		}
		return cells;
	}

	public void UnHighLightAllCells()
	{
		foreach (Cell cell in highlightedPathCells)
		{
			cell.UnHighlight();

		}
		highlightedPathCells.Clear();
		foreach (Cell cell in highlightedAttackCells)
		{
			cell.UnHighlight();
		}
		highlightedAttackCells.Clear();
	}

	public void DestroyChildren()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}

	/// <summary>
	/// Initialize grid, instantiate all cells and assign them to the grid.
	/// </summary>
	private void InitGrid()
	{
		grid = new GameObject[GameParams.MATRIX_HEIGHT, GameParams.MATRIX_WIDTH];
		// Instantiate and initialize cells
		for (int i = 0; i < GameParams.MATRIX_HEIGHT; i++)
		{
			int start = Mathf.Max(i + 1 - GameParams.GRID_HEIGHT, 0);
			int end = Mathf.Min(GameParams.GRID_WIDTH + i, GameParams.MATRIX_WIDTH);
			for (int j = start; j < end; j++)
			{
				// Calculate the center of the hexagon
				float x = j * (GameParams.CELL_WIDTH / 2 + GameParams.HEX_SIZE / 2);
				float y = (GameParams.CELL_HEIGHT - 1) * i - j * (GameParams.CELL_HEIGHT - 1) / 2;

				GameObject cellObject = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity);
				cellObject.transform.parent = transform;
				cellObject.GetComponent<Cell>().SetCoordinates(j, i);

				grid[i, j] = cellObject;
			}
		}

	}

	/// <summary>
	/// Sets the neighbors of each cell.
	/// </summary>
	public void SetNeighbors()
	{
		// Set the neighbors of each cell
		for (int i = 0; i < GameParams.MATRIX_HEIGHT; i++)
		{
			int start = Mathf.Max(i + 1 - GameParams.GRID_HEIGHT, 0);
			int end = Mathf.Min(GameParams.GRID_WIDTH + i, GameParams.MATRIX_WIDTH);
			for (int j = start; j < end; j++)
			{
				Cell cell = grid[i, j].GetComponent<Cell>();
				cell.neighbors = new Cell[6];
				cell.neighbors[0] = IsInGrid(i + 1, j) ? grid[i + 1, j].GetComponent<Cell>() : null;
				cell.neighbors[1] = IsInGrid(i + 1, j + 1) ? grid[i + 1, j + 1].GetComponent<Cell>() : null;
				cell.neighbors[2] = IsInGrid(i, j + 1) ? grid[i, j + 1].GetComponent<Cell>() : null;
				cell.neighbors[3] = IsInGrid(i - 1, j) ? grid[i - 1, j].GetComponent<Cell>() : null;
				cell.neighbors[4] = IsInGrid(i - 1, j - 1) ? grid[i - 1, j - 1].GetComponent<Cell>() : null;
				cell.neighbors[5] = IsInGrid(i, j - 1) ? grid[i, j - 1].GetComponent<Cell>() : null;
			}
		}
	}

	private static void PutTileOnCell(Cell cell, GameObject tilePrefab)
	{
		GameObject tileObject = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
		cell.SetTile(tileObject.GetComponent<Tile>());
	}

	private static void PutStructureOnCell(Cell cell, GameObject structurePrefab)
	{
		GameObject structureObject = Instantiate(structurePrefab, Vector3.zero, Quaternion.identity);
		Structure structure = structureObject.GetComponent<Structure>();
		cell.SetStructure(structure);
	}

	private static void PutEntityOnCell(Cell cell, GameObject entityPrefab)
	{
		GameObject entityObject = Instantiate(entityPrefab, Vector3.zero, Quaternion.identity);
		Entity entity = entityObject.GetComponent<Entity>();
		cell.SetEntity(entity);
	}

	/// <summary>
	/// Procedurally generate levels.
	/// </summary>
	public void FillGrid(int level = 0, bool createPlayer = true)
	{
		// Put player on the grid
		Cell playerCell = grid[1, 4].GetComponent<Cell>();
		if (createPlayer)
		{
			PutEntityOnCell(playerCell, playerPrefab);
		}
		else
		{
			Cell prevPlayerCell = player.GetCurrentCell();
			prevPlayerCell.UnSetEntity();
			playerCell.SetEntity(player);
		}
		player = playerCell.entity as Player;

		// Put elevator on the grid
		Cell elevatorCell = grid[9, 4].GetComponent<Cell>();
		PutStructureOnCell(elevatorCell, elevatorPrefab);
		elevator = elevatorCell.structure as Elevator;

		// List of banned cells for placing enemies and voids
		List<Cell> protectedCells = new List<Cell>(){
			playerCell,
			elevatorCell,
			grid[2, 4].GetComponent<Cell>(),
			grid[2, 5].GetComponent<Cell>(),
			grid[1, 5].GetComponent<Cell>(),
			grid[0, 4].GetComponent<Cell>(),
			grid[0, 3].GetComponent<Cell>(),
			grid[1, 3].GetComponent<Cell>(),
		};

		// Random walk to elevator || findPath -> banned cells
		Cell upperCell = grid[10, 4].GetComponent<Cell>();
		List<Cell> path = RandomUpWalk(playerCell, upperCell);
		foreach (Cell cell in path)
		{
			if (cell != playerCell && cell != upperCell)
			{
				protectedCells.Add(cell);
			}
		}


		// We choose 3 random cells to start the void
		List<Cell> voidCells = new List<Cell>();
		int voidCellsCount = 3;
		while (voidCellsCount > 0)
		{
			int i = Random.Range(0, GameParams.MATRIX_HEIGHT);
			int start = Mathf.Max(i + 1 - GameParams.GRID_HEIGHT, 0);
			int end = Mathf.Min(GameParams.GRID_WIDTH + i, GameParams.MATRIX_WIDTH);
			int j = Random.Range(start, end);
			Cell cell = grid[i, j].GetComponent<Cell>();
			if (cell.entity == null && !protectedCells.Contains(cell))
			{
				voidCells.Add(cell);
				protectedCells.Add(cell);
				voidCellsCount--;
			}
		}

		// We add 7 + level more cells near those 3 void cells
		for (int i = 0; i < 7 + level; i++)
		{
			// Get all neighbors of void cells
			List<Cell> neighbors = new List<Cell>();
			foreach (Cell cell in voidCells)
			{
				foreach (Cell neighbor in cell.GetNeighbors())
				{
					if (!protectedCells.Contains(neighbor))
					{
						neighbors.Add(neighbor);
					}
				}
			}
			// Add one random neighbor
			if (neighbors.Count > 0)
			{
				int index = Random.Range(0, neighbors.Count);
				voidCells.Add(neighbors[index]);
				protectedCells.Add(neighbors[index]);
			}
		}

		// Create the voids
		foreach (Cell cell in voidCells)
		{
			PutTileOnCell(cell, voidPrefab);
		}

		// Get all cells that are not protected
		List<Cell> remainingCells = new List<Cell>();
		foreach (Cell cell in GetCells())
		{
			if (!protectedCells.Contains(cell))
			{
				remainingCells.Add(cell);
			}
		}

		// Shuffle the remaining cells
		for (int i = 0; i < remainingCells.Count; i++)
		{
			Cell temp = remainingCells[i];
			int randomIndex = Random.Range(i, remainingCells.Count);
			remainingCells[i] = remainingCells[randomIndex];
			remainingCells[randomIndex] = temp;
		}

		// Put incubator on the grid
		int putCellCount = 0;
		PutStructureOnCell(remainingCells[putCellCount], incubatorPrefab);
		putCellCount++;

		// Put enemies on the grid
		for (int i = 0; i < enemyPrefabs.Length; i++)
		{
			for (int j = 0; j < GameParams.LEVEL_ENEMY_COUNT[level, i]; j++)
			{
				PutEntityOnCell(remainingCells[putCellCount], enemyPrefabs[i]);
				enemies.Add(remainingCells[putCellCount].entity as Enemy);
				putCellCount++;
			}
		}

		// Fill the grid with floor
		for (int i = 0; i < GameParams.MATRIX_HEIGHT; i++)
		{
			int start = Mathf.Max(i + 1 - GameParams.GRID_HEIGHT, 0);
			int end = Mathf.Min(GameParams.GRID_WIDTH + i, GameParams.MATRIX_WIDTH);
			for (int j = start; j < end; j++)
			{
				Cell cell = grid[i, j].GetComponent<Cell>();

				if (!voidCells.Contains(cell))
				{
					GameObject floorObject = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
					cell.SetTile(floorObject.GetComponent<LabFloor>());
				}

			}
		}
	}

	/// <summary>
	/// Destroys all content and tiles of the grid if they exist.
	/// </summary>
	public void ResetGrid(bool destroyPlayer = true)
	{
		foreach (Transform child in transform)
		{
			Cell cell = child.GetComponent<Cell>();

			foreach (Transform grandChild in child)
			{
				if (grandChild.GetComponent<Player>() != null && !destroyPlayer)
				{
					continue;
				}
				Destroy(grandChild.gameObject);
			}
		}
		if (destroyPlayer)
		{
			player = null;
		}
		enemies.Clear();
	}

	/// <summary>
	/// Finds the shortest path from startCell to endCell using BFS. Makes use of a Queue to make it more efficient.
	/// </summary>
	public static List<Cell> FindPath(Cell startCell, Cell endCell, bool adjacentTarget)
	{
		// List of cells to visit
		Queue<Cell> openCells = new Queue<Cell>();
		foreach (Cell neighbor in startCell.GetNeighbors())
		{
			if (neighbor.IsEmptyFloor())
			{
				openCells.Enqueue(neighbor);
			}
		}

		// List of visited cells
		List<Cell> closedCells = new List<Cell>
		{
			startCell
		};

		// Dictionary of parents
		Dictionary<Cell, Cell> parents = new Dictionary<Cell, Cell>();
		foreach (Cell cell in openCells)
		{
			parents.Add(cell, startCell);
		}

		// BFS
		while (openCells.Count > 0)
		{
			Cell currentCell = openCells.Dequeue();
			closedCells.Add(currentCell);

			if (currentCell == endCell)
			{
				return RecreatePath(startCell, endCell, parents);
			}
			else if (adjacentTarget && endCell.GetNeighbors().Contains(currentCell))
			{
				return RecreatePath(startCell, currentCell, parents);
			}

			foreach (Cell neighbor in currentCell.GetNeighbors())
			{
				if (!closedCells.Contains(neighbor) && !openCells.Contains(neighbor))
				{
					if (neighbor.IsEmptyFloor())
					{
						openCells.Enqueue(neighbor);
						parents.Add(neighbor, currentCell);
					}
				}
			}
		}
		// No path found
		return new List<Cell>();
	}

	private static List<Cell> RecreatePath(Cell startCell, Cell endCell, Dictionary<Cell, Cell> parents)
	{
		// Path found
		List<Cell> path = new List<Cell>();
		Cell current = endCell;
		while (current != startCell)
		{
			path.Add(current);
			current = parents[current];
		}
		path.Reverse();
		return path;
	}

	public List<Cell> RandomUpWalk(Cell startCell, Cell endCell)
	{
		List<Cell> path = new List<Cell>
		{
			startCell
		};
		Cell currentCell = startCell;
		while (currentCell != endCell)
		{
			List<Cell> possibleCells = new List<Cell>
			{
				currentCell.neighbors[0],
				currentCell.neighbors[1],
				currentCell.neighbors[5]
			};
			// remove nulls
			possibleCells.RemoveAll(cell => cell == null);

			// choose one random
			int index = Random.Range(0, possibleCells.Count);
			currentCell = possibleCells[index];
			path.Add(currentCell);

		}
		return path;
	}

}
