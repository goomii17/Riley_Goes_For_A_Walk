using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Swat : Enemy
{
	public static List<Cell> GetValidKnightMoves(Cell cell)
	{
		List<Cell> knightMoves = new List<Cell>();
		for (int i = 0; i < 5; i += 2)
		{
			int n1 = (i + 5) % 5;
			int n2 = (i + 1) % 5;
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

	public override void FindNextMove(Cell playerCell)
	{
		List<Cell> bestPath = null;
		int minLength = 10000;
		foreach (Cell cell in GetValidKnightMoves(playerCell))
		{
			List<Cell> path = CellGrid.FindPath(currentCell, cell, true);
			if (path.Count > 0 && path.Count < minLength)
			{
				if (!alreadyTargetedCells.Contains(path[0]))
				{
					minLength = path.Count;
					bestPath = path;
				}
			}
		}

		if (bestPath != null)
		{
			SetNextMoveCell(bestPath[0]);
		}
		else
		{
			SetNextMoveCell(null);
		}

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
		// Swat can attack in all 6 directions in a range of 5
		List<Cell> attackableCells = new List<Cell>();

		for (int i = 0; i < 6; i++)
		{
			Cell cell = currentCell.neighbors[i];
			if (cell == null)
			{
				continue;
			}
			if (cell.GetEntityType() == EntityType.Enemy)
			{
				continue;
			}
			for (int j = 0; j < 5; j++)
			{
				cell = cell.neighbors[i];
				if (cell == null)
				{
					break;
				}
				if (cell.GetEntityType() == EntityType.Enemy)
				{
					break;
				}
				attackableCells.Add(cell);
			}
		}
		return attackableCells;
	}


	public override bool AnimateAttack()
	{
		return (animator as SwatAnimator).AnimateAttack(NextAttackCell);
	}

	public override void ResetAttackAnimation()
	{
		(animator as SwatAnimator).ResetAttackAnimation();
	}
}
