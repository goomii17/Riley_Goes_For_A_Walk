using UnityEngine;

public class LabFloor : Tile
{

	private SpriteRenderer spriteRenderer;

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public LabFloor() : base()
	{
		Type = TileType.Floor;
	}

	public override void HighlightTile(Color color)
	{
		spriteRenderer.color = color;
	}

	public override void UnHighlightTile()
	{
		spriteRenderer.color = Color.white;
	}
}
