using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
	// Cell prefab
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
	public List<Enemy> enemies = new List<Enemy>();

	public void Awake()
	{
		DestroyChildren();
		InitGrid();
		SetNeighbors();
		ResetGrid();
		FillGrid();
	}

	public static bool IsInGrid(int i, int j)
	{
		int start = Mathf.Max(i + 1 - GameParams.GRID_HEIGHT, 0);
		int end = Mathf.Min(GameParams.GRID_WIDTH + i, GameParams.MATRIX_WIDTH);
		return i >= 0 && i < GameParams.MATRIX_HEIGHT && j >= start && j < end;
	}

	public void DestroyChildren()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}

	/// <summary>
	/// Initialize grid, instantiate all cells and add them to the grid.
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

	private void PutEntityOnCell(Cell cell, GameObject entityPrefab)
	{
		GameObject entityObject = Instantiate(entityPrefab, Vector3.zero, Quaternion.identity);
		Entity entity = entityObject.GetComponent<Entity>();
		entity.ResetTransform();
		cell.SetContent(entity);
	}

	private void PutTileOnCell(Cell cell, GameObject tilePrefab)
	{
		GameObject tileObject = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
		cell.SetTile(tileObject.GetComponent<Tile>());
	}

	/// <summary>
	/// Fills the grid with player, enemies, elevator, floor and void.
	/// </summary>
	/// <param name="level"></param>
	public void FillGrid(int level = 1)
	{
		// Put player on the grid
		Cell playerCell = grid[1, 4].GetComponent<Cell>();
		PutEntityOnCell(playerCell, playerPrefab);
		player = playerCell.content as Player;

		// Put elevator on the grid
		Cell elevatorCell = grid[9, 4].GetComponent<Cell>();
		PutEntityOnCell(elevatorCell, elevatorPrefab);

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
			if (cell.content == null && !protectedCells.Contains(cell))
			{
				voidCells.Add(cell);
				protectedCells.Add(cell);
				voidCellsCount--;
			}
		}

		Debug.Log("Void cells length: " + voidCells.Count + "");

		// We add 7 more cells near those 3 void cells
		for (int i = 0; i < 7; i++)
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

		Debug.Log("Void cells length: " + voidCells.Count + "");

		// Create the voids
		foreach (Cell cell in voidCells)
		{
			Debug.Log("Adding void cell " + cell);
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
		// not implemented yet
		// Put enemies on the grid
		for (int i = 0; i < 5; i++)
		{
			PutEntityOnCell(remainingCells[i], enemyPrefabs[0]);
			enemies.Add(remainingCells[i].content as Enemy);
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
					// Add floor
					GameObject floorObject = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
					cell.SetTile(floorObject.GetComponent<LabFloor>());
				}

			}
		}
	}

	/// <summary>
	/// Destroys all content and tiles of the grid if they exist.
	/// </summary>
	public void ResetGrid()
	{
		foreach (Transform child in transform)
		{
			Cell cell = child.GetComponent<Cell>();

			if (cell.content != null)
			{
				cell.content.AutoDestroy();
				cell.content = null;
			}
			if (cell.tile != null)
			{
				cell.tile.AutoDestroy();
				cell.tile = null;
			}
		}
		player = null;
		enemies.Clear();
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

	/// <summary>
	/// Finds the path from startCell to endCell.
	/// </summary>
	/// <param name="startCell"></param>
	/// <param name="endCell"></param>
	public List<Cell> FindPath(Cell startCell, Cell endCell)
	{
		// List of cells to visit
		Debug.Log("Finding path from " + startCell + " to " + endCell);
		List<Cell> openCells = new List<Cell>();
		foreach (Cell neighbor in startCell.GetNeighbors())
		{
			if (neighbor.tile.Type == TileType.Floor && neighbor.content == null)
			{
				openCells.Add(neighbor);
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

		while (openCells.Count > 0)
		{
			Cell currentCell = openCells[0];
			openCells.RemoveAt(0);
			closedCells.Add(currentCell);

			if (currentCell == endCell)
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

			foreach (Cell neighbor in currentCell.GetNeighbors())
			{
				if (!closedCells.Contains(neighbor) && !openCells.Contains(neighbor))
				{
					if (neighbor.tile.Type == TileType.Floor)
					{
						openCells.Add(neighbor);
						parents.Add(neighbor, currentCell);

					}
				}
			}
		}
		// No path found
		return new List<Cell>();
	}

	/// <summary>
	/// Finds a random walk that just goes up (3 directions), from startCell to endCell.
	/// </summary>
	public List<Cell> RandomUpWalk(Cell startCell, Cell endCell)
	{
		// List path
		List<Cell> path = new List<Cell>();
		path.Add(startCell);
		Cell currentCell = startCell;
		while (currentCell != endCell)
		{
			List<Cell> possibleCells = new List<Cell>();
			// neighbors  0, 1 and 5
			possibleCells.Add(currentCell.neighbors[0]);
			possibleCells.Add(currentCell.neighbors[1]);
			possibleCells.Add(currentCell.neighbors[5]);
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
