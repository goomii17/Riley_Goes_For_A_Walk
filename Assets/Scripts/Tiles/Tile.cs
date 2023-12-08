using UnityEngine;

public enum TileType
{
	Floor,
	Void,
	None
}

public abstract class Tile : MonoBehaviour
{
	public TileType Type { get; protected set; }

	public void SetParentCell(Cell cell)
	{
		transform.SetParent(cell.gameObject.transform, false);
	}

	public abstract void Highlight(Color color);
	public abstract void UnHighlight();

}
