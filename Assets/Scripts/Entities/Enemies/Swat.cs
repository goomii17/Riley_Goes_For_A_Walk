using System.Collections.Generic;
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
		List<Cell> attackableCellsCoords = new List<Cell>();
		List<List<int>> directions = new List<List<int>>();
		directions.Add(new List<int> { 0, 1 });
		directions.Add(new List<int> { 1, 0 });
		directions.Add(new List<int> { 1, -1 });
		directions.Add(new List<int> { 0, -1 });
		directions.Add(new List<int> { -1, 0 });
		directions.Add(new List<int> { -1, 1 });
		int x = currentCell.x;
		int y = currentCell.y;
		foreach (List<int> direction in directions)
		{
			for (int i = 0; i < 5; i++)
			{
				x += direction[0];
				y += direction[1];
				if (CellGrid.IsInGrid(y, x))
				{
					GameObject cellObject = GameManager.Instance.cellGrid.GetCell(y, x);
					Cell cell = cellObject.GetComponent<Cell>();
					if (cell.IsEmptyFloor())
					{
						attackableCells.Add(cell);
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			x = currentCell.x;
			y = currentCell.y;
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
