using System.Collections.Generic;
using UnityEngine;

public class Hazmat : Enemy
{
	public static List<Cell> GetValidKnightMoves(Cell cell)
	{
		List<Cell> knightMoves = new List<Cell>();
		for (int i = 0; i < 5; i += 2)
		{
			int n1 = (i + 5) % 6;
			int n2 = (i + 1) % 6;
			if (cell.neighbors[i] != null)
			{
				Cell nightCell1 = cell.neighbors[i].neighbors[n1];
				if (nightCell1 != null && nightCell1.IsEmptyFloor())
				{
					knightMoves.Add(nightCell1);
				}
				Cell nightCell2 = cell.neighbors[i].neighbors[n2];
				if (nightCell2 != null && nightCell2.IsEmptyFloor())
				{
					knightMoves.Add(nightCell2);
				}
			}
		}
		return knightMoves;
	}

	public override void FindNextMove(Cell playerCell, float intelligence)
	{
		List<Cell> bestPath = null;
		int minLength = 10000;
		foreach (Cell cell in GetValidKnightMoves(playerCell))
		{
			List<Cell> path = CellGrid.FindPath(currentCell, cell, false);
			if (path.Count > 0 && path.Count < minLength)
			{
				if (!alreadyTargetedCells.Contains(path[0]))
				{
					minLength = path.Count;
					bestPath = path;
				}
			}
		}
		// Make best move to corner player
		if (bestPath != null && Random.Range(0f, 1f) < intelligence)
		{
			SetNextMoveCell(bestPath[0]);
			return;
		}

		// Try to move towards player
		List<Cell> pathToPlayer = CellGrid.FindPath(currentCell, playerCell, true);
		if (pathToPlayer.Count > 0 && !alreadyTargetedCells.Contains(pathToPlayer[0]))
		{
			SetNextMoveCell(pathToPlayer[0]);
			return;
		}

		// Move randomly
		List<Cell> moveableCells = GetMoveableCells();
		if (moveableCells.Count > 0)
		{
			SetNextMoveCell(moveableCells[Random.Range(0, moveableCells.Count)]);
			return;
		}
		SetNextMoveCell(null);
	}

	public override void UpdateNextAttack()
	{
		foreach (Cell cell in GetAttackableCells())
		{
			if (cell.GetEntityType() == EntityType.Player)
			{
				NextAttackCell = cell;
				return;
			}
		}
		NextAttackCell = null;
	}

	public override List<Cell> GetAttackableCells()
	{
		// Hazmat is a melee enemy, returns cells with no content or player
		List<Cell> attackableCells = new List<Cell>();
		foreach (Cell cell in currentCell.GetNeighbors())
		{
			if (cell.entity == null || cell.entity.Type == EntityType.Player)
			{
				attackableCells.Add(cell);
			}
		}
		return attackableCells;
	}

	public override bool AnimateAttack()
	{
		return animator.AnimateAttack(NextAttackCell);
	}

	public override void ResetAttackAnimation()
	{
		animator.ResetAttackAnimation();
	}
}
