using System.Collections.Generic;

public class Player : Entity
{
	protected PlayerAnimator animator;

	private Cell killMeleeLeft;
	private Cell killMeleeRight;
	private Cell killFrontal;

	public Player() : base()
	{
		Type = EntityType.Player;
	}

	public void Awake()
	{
		animator = GetComponent<PlayerAnimator>();
	}

	public override List<Cell> GetMoveableCells()
	{
		List<Cell> moveableCells = new List<Cell>();
		foreach (Cell cell in currentCell.GetNeighbors())
		{
			if (cell.IsEmptyFloor() || cell.GetStructureType() != StructureType.None)
			{
				moveableCells.Add(cell);
			}
		}
		return moveableCells;
	}

	public void UpdateNextKills(Cell nextCell)
	{
		killMeleeLeft = null;
		killMeleeRight = null;
		killFrontal = null;

		var dx = nextCell.x - currentCell.x;
		var dy = nextCell.y - currentCell.y;

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
		return cell != null && cell.GetEntityType() == EntityType.Enemy;
	}

	public bool ValidFrontalCell(Cell cell, int neighborIndex)
	{
		bool validEmpty = cell != null && cell.IsEmptyFloor();
		if (!validEmpty)
		{
			return false;
		}
		Cell frontalCell = cell.neighbors[neighborIndex];
		return frontalCell != null && frontalCell.GetEntityType() == EntityType.Enemy;
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

	public bool DrainIncubator()
	{
		if (nextMoveCell != null && nextMoveCell.GetStructureType() == StructureType.Incubator)
		{
			Incubator incubator = (Incubator)nextMoveCell.structure;
			if (incubator.IsFilled())
			{
				animator.Evolve();
				incubator.Drain();
				return true;
			}
		}
		return false;
	}

	public bool TakeElevator()
	{
		if (nextMoveCell != null && nextMoveCell.GetStructureType() == StructureType.Elevator)
		{
			Elevator elevator = (Elevator)nextMoveCell.structure;
			elevator.Open();
			return true;
		}
		return false;
	}

	public override void AnimateIdle()
	{
		animator.AnimateIdle();
	}

	public override void ResetMoveAnimation()
	{
		animator.ResetMoveAnimation();
	}

	public override bool AnimateMove()
	{
		if (nextMoveCell == null) return true;
		return animator.AnimateMove(nextMoveCell, killMeleeLeft, killMeleeRight, killFrontal);
	}

	public override void MakeMove()
	{
		if (nextMoveCell != null && nextMoveCell != currentCell)
		{
			currentCell.UnSetEntity();
			nextMoveCell.SetEntity(this);
		}
		nextMoveCell = null;
		killMeleeLeft = null;
		killMeleeRight = null;
		killFrontal = null;
	}
}
