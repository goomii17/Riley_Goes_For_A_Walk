using UnityEngine;

public class Void : Tile
{
	public Void() : base()
	{
		Type = TileType.Void;
	}

	public override void HighlightTile(Color _)
	{
		// Do nothing
	}

	public override void UnHighlightTile()
	{
		// Do nothing
	}
}
