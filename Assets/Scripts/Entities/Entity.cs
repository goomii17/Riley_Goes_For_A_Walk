using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
	Player,
	Enemy,
	Elevator,
}

public abstract class Entity : MonoBehaviour
{

	private EntityAnimator animator;

	public void Awake()
	{
		animator = GetComponent<EntityAnimator>();
	}

	protected Cell currentCell;
	protected Cell nextMoveCell;

	public EntityType Type { get; protected set; }

	public void AutoDestroy()
	{
		Destroy(gameObject);
	}

	public void ResetTransform()
	{
		Vector3 startPosition = Vector3.up * 4.6f - Vector3.forward * 3.8f;
		Quaternion startRotation = Quaternion.Euler(-15, 0, 0);
		transform.SetLocalPositionAndRotation(startPosition, startRotation);
	}

	public void SetCurrentCell(Cell cell)
	{
		transform.SetParent(cell.gameObject.transform, false);
		ResetTransform();
		currentCell = cell;
	}

	public Cell GetCurrentCell()
	{
		return currentCell;
	}

	public void AnimateIdle()
	{
		animator.AnimateIdle();
	}

	public virtual void SetNextMove(Cell cell)
	{
		nextMoveCell = cell;
	}

	public virtual void FindNextMove()
	{
		// List<Cell> moveableCells = GetMoveableCells();
		// if (moveableCells.Count == 0)
		// {
		// 	SetNextMove(null);
		// 	return;
		// }
		// Cell targetCell = moveableCells[Random.Range(0, moveableCells.Count)];

		// CellGrid findPath:
		Cell playerCell = GameManager.Instance.cellGrid.player.GetCurrentCell();
		List<Cell> path = GameManager.Instance.cellGrid.FindPath(currentCell, playerCell);
		Cell targetCell = path[0];
		SetNextMove(targetCell);
	}

	public bool AnimateMove()
	{
		if (nextMoveCell == null) return true;
		return animator.AnimateMove(nextMoveCell);
	}

	public void ResetMoveAnimation()
	{
		animator.ResetMoveAnimation();
	}

	public void MakeMove()
	{
		if (nextMoveCell != null)
		{
			currentCell.UnSetContent();
			nextMoveCell.SetContent(this);
		}
	}

	public virtual List<Cell> GetMoveableCells()
	{
		List<Cell> moveableCells = new List<Cell>();
		foreach (Cell cell in currentCell.GetNeighbors())
		{
			if (cell.IsEmptyFloor())
			{
				moveableCells.Add(cell);
			}
		}
		return moveableCells;
	}

}
