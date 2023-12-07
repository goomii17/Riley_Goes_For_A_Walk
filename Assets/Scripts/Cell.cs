using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
	// Coordinates of the cell
	public int x { get; private set; }
	public int y { get; private set; }

	// Neighbors of the cell
	public Cell[] neighbors;

	// Base of the cell
	public Tile tile;

	// Content of the cell
	public Entity content;

	void OnMouseDown()
	{
		var disNuts = this;
		GameManager.Instance.OnCellClicked(disNuts);
	}

	public void SetCoordinates(int x, int y)
	{
		// Set the coordinates of the cell
		this.x = x;
		this.y = y;
	}


	public bool IsEmptyFloor()
	{
		if (tile.Type == TileType.Floor && content == null) return true;
		return false;
	}

	public List<Cell> GetNeighbors()
	{
		List<Cell> neighbors = new List<Cell>();
		foreach (Cell neighbor in this.neighbors)
		{
			if (neighbor != null)
			{
				neighbors.Add(neighbor);
			}
		}
		return neighbors;
	}

	public void KillContent()
	{
		if (content == null)
		{
			Debug.Log("ERROR: Trying to kill null content");
			return;
		}
		// Kill the content of the cell
		content.AutoDestroy();
		UnSetContent();
	}

	public void UnSetContent()
	{
		// Unset the content of the cell
		content = null;
	}

	public void SetContent(Entity entity)
	{
		// Set the content of the cell
		content = entity;
		// Set entity's current cell
		entity.SetCurrentCell(this);
	}

	public void SetTile(Tile tile)
	{
		// Set the tile of the cell
		this.tile = tile;

		// Set tiles parent
		tile.SetCurrentCell(this);
	}

	public void HighlightCell(Color color)
	{
		// Highlight the cell or change sprite in content
		tile.HighlightTile(color);
	}

	public void UnHighlightCell()
	{
		// Unhighlight the cell or change sprite in content
		tile.UnHighlightTile();
	}

}
