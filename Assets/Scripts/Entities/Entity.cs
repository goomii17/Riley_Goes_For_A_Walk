using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
	Player,
	Enemy,
	None
}

public abstract class Entity : MonoBehaviour
{

	public EntityType Type { get; protected set; }

	private EntityAnimator animator;

	protected Cell currentCell;
	protected Cell nextMoveCell;

	public void Awake()
	{
		animator = GetComponent<EntityAnimator>();
		// sprite renderer cast shadow
		GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
	}

	public void PositionIn3D()
	{
		Vector3 startPosition = Vector3.up * 4.6f - Vector3.forward * 3.8f;
		Quaternion startRotation = Quaternion.Euler(-15, 0, 0);
		transform.SetLocalPositionAndRotation(startPosition, startRotation);
	}

	public void SetCurrentCell(Cell cell)
	{
		transform.SetParent(cell.gameObject.transform, false);
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

	public virtual void SetNextMoveCell(Cell cell)
	{
		nextMoveCell = cell;
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
		if (nextMoveCell != null && nextMoveCell != currentCell)
		{
			currentCell.UnSetEntity();
			nextMoveCell.SetEntity(this);
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
