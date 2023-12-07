using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{

	private Cell killMeleeLeft;
	private Cell killMeleeRight;
	private Cell killFrontal;

	public Player() : base()
	{
		Type = EntityType.Player;
	}

	public void UpdateNextKills(Cell nextCell)
	{
		killMeleeLeft = null;
		killMeleeRight = null;
		killFrontal = null;

		var dx = nextCell.x - currentCell.x;
		var dy = nextCell.y - currentCell.y;

		Debug.Log("DX: " + dx + " DY: " + dy);

		if (dy > 0)
		{
			switch (dx)
			{
				case 0:
					// Left 5, Right is 1, frontal is 0.0
					killMeleeLeft = ValidMeleeCell(currentCell.neighbors[5]) ? currentCell.neighbors[5] : null;
					killMeleeRight = ValidMeleeCell(currentCell.neighbors[1]) ? currentCell.neighbors[1] : null;
					killFrontal = ValidFrontalCell(nextCell, 0) ? nextCell.neighbors[0] : null;
					break;
				case 1:
					// Left 0, Right is 2, frontal is 1.1
					killMeleeLeft = ValidMeleeCell(currentCell.neighbors[0]) ? currentCell.neighbors[0] : null;
					killMeleeRight = ValidMeleeCell(currentCell.neighbors[2]) ? currentCell.neighbors[2] : null;
					killFrontal = ValidFrontalCell(nextCell, 1) ? nextCell.neighbors[1] : null;
					break;
			}
		}
		else if (dy == 0)
		{
			switch (dx)
			{
				case -1:
					// Left 4, Right is 0, frontal is 5.5
					killMeleeLeft = ValidMeleeCell(currentCell.neighbors[4]) ? currentCell.neighbors[4] : null;
					killMeleeRight = ValidMeleeCell(currentCell.neighbors[0]) ? currentCell.neighbors[0] : null;
					killFrontal = ValidFrontalCell(nextCell, 5) ? nextCell.neighbors[5] : null;
					break;
				case 1:
					// Left 1, Right is 3, frontal is 2.2
					killMeleeLeft = ValidMeleeCell(currentCell.neighbors[1]) ? currentCell.neighbors[1] : null;
					killMeleeRight = ValidMeleeCell(currentCell.neighbors[3]) ? currentCell.neighbors[3] : null;
					killFrontal = ValidFrontalCell(nextCell, 2) ? nextCell.neighbors[2] : null;
					break;
			}
		}
		else if (dy < 0)
		{
			switch (dx)
			{
				case 0:
					// Left 2, Right is 4, frontal is 3.3
					killMeleeLeft = ValidMeleeCell(currentCell.neighbors[2]) ? currentCell.neighbors[2] : null;
					killMeleeRight = ValidMeleeCell(currentCell.neighbors[4]) ? currentCell.neighbors[4] : null;
					killFrontal = ValidFrontalCell(nextCell, 3) ? nextCell.neighbors[3] : null;
					break;
				case -1:
					// Left 3, Right is 5, frontal is 4.4
					killMeleeLeft = ValidMeleeCell(currentCell.neighbors[3]) ? currentCell.neighbors[3] : null;
					killMeleeRight = ValidMeleeCell(currentCell.neighbors[5]) ? currentCell.neighbors[5] : null;
					killFrontal = ValidFrontalCell(nextCell, 4) ? nextCell.neighbors[4] : null;
					break;
			}
		}

	}

	public bool ValidMeleeCell(Cell cell)
	{
		return cell != null && cell.content != null && cell.content.Type == EntityType.Enemy;
	}

	public bool ValidFrontalCell(Cell cell, int neighborIndex)
	{
		Debug.Log("Validating frontal cell: " + cell + " with neighbor index " + neighborIndex);
		bool validEmpty = cell != null && cell.IsEmptyFloor();
		if (!validEmpty)
		{
			Debug.Log("Middle cell is not empty");
			return false;
		}
		Cell frontalCell = cell.neighbors[neighborIndex];
		Debug.Log("Frontal cell is " + frontalCell);
		return frontalCell != null && frontalCell.content != null && frontalCell.content.Type == EntityType.Enemy;
	}

	public List<Cell> GetNextKillsCells()
	{
		List<Cell> kills = new List<Cell>();
		if (killMeleeLeft != null)
		{
			kills.Add(killMeleeLeft);
		}
		if (killMeleeRight != null)
		{
			kills.Add(killMeleeRight);
		}
		if (killFrontal != null)
		{
			kills.Add(killFrontal);
		}
		return kills;
	}

}
