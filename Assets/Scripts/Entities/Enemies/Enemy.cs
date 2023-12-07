using System.Collections.Generic;

public abstract class Enemy : Entity
{
	public static List<Cell> alreadyTargetedCells = new List<Cell>();
	protected Cell NextAttackCell;

	public Enemy() : base()
	{
		Type = EntityType.Enemy;
	}

	public override List<Cell> GetMoveableCells()
	{
		List<Cell> moveableCells = new List<Cell>();
		foreach (Cell cell in currentCell.GetNeighbors())
		{
			if (cell.IsEmptyFloor() && !alreadyTargetedCells.Contains(cell))
			{
				moveableCells.Add(cell);
			}
		}
		return moveableCells;
	}

	public override void SetNextMove(Cell cell)
	{
		base.SetNextMove(cell);
		alreadyTargetedCells.Add(cell);
	}

	public abstract void UpdateNextAttack();
	public abstract bool AnimateAttack();
	public abstract void ResetAttackAnimation();
	public abstract bool AttackPlayer();
	public abstract List<Cell> GetAttackableCells();
}
