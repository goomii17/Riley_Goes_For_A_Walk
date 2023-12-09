using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
	protected EnemyAnimator animator;

	[SerializeField] public GameObject headPrefab;

	public static List<Cell> alreadyTargetedCells = new List<Cell>();

	protected Cell NextAttackCell;

	public Enemy() : base()
	{
		Type = EntityType.Enemy;
	}

	public void Awake()
	{
		animator = GetComponent<EnemyAnimator>();
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

	public virtual void FindNextMove(Cell playerCell, float intelligence)
	{
		List<Cell> path = CellGrid.FindPath(currentCell, playerCell, true);
		//List<Cell> path = new List<Cell>();
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
		return animator.AnimateMove(nextMoveCell);
	}

	public abstract List<Cell> GetAttackableCells();
	public abstract void UpdateNextAttack();

	public abstract bool AnimateAttack();
	public abstract void ResetAttackAnimation();

	public bool AttackPlayer()
	{
		return NextAttackCell != null;
	}

}
