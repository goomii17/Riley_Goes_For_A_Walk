using UnityEngine;

public enum TileType
{
	Floor,
	Void,
}

public abstract class Tile : MonoBehaviour
{
	public TileType Type { get; set; }


	public abstract void HighlightTile(Color color);
	public abstract void UnHighlightTile();

	public void AutoDestroy()
	{
		Destroy(gameObject);
	}

	public void SetCurrentCell(Cell cell)
	{
		transform.SetParent(cell.gameObject.transform, false);
	}

}
