using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
	// Coordinates of the cell
	public int x { get; private set; }
	public int y { get; private set; }

	// Neighbors of the cell
	public Cell[] neighbors;

	// Content of the cell
	public Tile tile { get; private set; }
	public Structure structure { get; private set; }
	public Entity entity { get; private set; }

	public void SetCoordinates(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector3 GetEntityPosition()
	{
		Vector3 position = transform.position;
		position.y += GameParams.ENTITY_3D_HEIGHT;
		position.z += GameParams.ENTITY_3D_DEPTH;
		return position;
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

	public void SetTile(Tile tile)
	{
		this.tile = tile;
		tile.SetParentCell(this);
	}

	public TileType GetTileType()
	{
		return tile == null ? TileType.None : tile.Type;
	}

	public void SetStructure(Structure structure)
	{
		this.structure = structure;
		structure.SetParentCell(this);
		structure.PositionIn3D();
	}

	public StructureType GetStructureType()
	{
		return structure == null ? StructureType.None : structure.Type;
	}

	public void SetEntity(Entity entity)
	{
		this.entity = entity;
		entity.SetCurrentCell(this);
		entity.PositionIn3D();
	}

	public void UnSetEntity()
	{
		entity = null;
	}

	public EntityType GetEntityType()
	{
		return entity == null ? EntityType.None : entity.Type;
	}

	public void KillEntity()
	{
		if (entity == null)
		{
			Debug.Log("ERROR: Trying to kill null entity");
			return;
		}
		Destroy(entity.gameObject);
		UnSetEntity();
	}

	public bool IsEmptyFloor()
	{
		return GetTileType() == TileType.Floor && entity == null && structure == null;
	}

	public void Highlight(Color color)
	{
		tile.Highlight(color);
	}

	public void UnHighlight()
	{
		tile.UnHighlight();
	}

	void OnMouseDown()
	{
		var disNuts = this;
		GameManager.Instance.OnCellClicked(disNuts);
	}

}
