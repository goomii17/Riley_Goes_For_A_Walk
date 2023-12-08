using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
	public static List<Cell> alreadyTargetedCells = new List<Cell>();

	protected Cell NextAttackCell;

	public Enemy() : base()
	{
		Type = EntityType.Enemy;
	}

	public override void SetNextMoveCell(Cell cell)
	{
		base.SetNextMoveCell(cell);
		alreadyTargetedCells.Add(cell);
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

	public virtual void FindNextMove(Cell playerCell)
	{
		List<Cell> path = CellGrid.FindPath(currentCell, playerCell, true);
		if (path.Count > 0 && !alreadyTargetedCells.Contains(path[0]))
		{
			SetNextMoveCell(path[0]);
		}
		else
		{
			List<Cell> moveableCells = GetMoveableCells();
			if (moveableCells.Count == 0)
			{
				SetNextMoveCell(null);
				return;
			}
			SetNextMoveCell(moveableCells[Random.Range(0, moveableCells.Count)]);
		}
	}

	public abstract void UpdateNextAttack();
	public abstract bool AnimateAttack();
	public abstract void ResetAttackAnimation();
	public abstract bool AttackPlayer();
	public abstract List<Cell> GetAttackableCells();
}
