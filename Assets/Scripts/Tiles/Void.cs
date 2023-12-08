using UnityEngine;

public class Void : Tile
{
	public Void() : base()
	{
		Type = TileType.Void;
	}

	public override void Highlight(Color _)
	{
		// Do nothing
	}

	public override void UnHighlight()
	{
		// Do nothing
	}
}
