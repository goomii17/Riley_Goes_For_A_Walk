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

	public override void Highlight(Color color)
	{
		spriteRenderer.color = color;
	}

	public override void UnHighlight()
	{
		spriteRenderer.color = Color.white;
	}
}
