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

	protected Cell currentCell;
	protected Cell nextMoveCell;

	public void PositionIn3D()
	{
		Vector3 startPosition = Vector3.up * GameParams.ENTITY_3D_HEIGHT + Vector3.forward * GameParams.ENTITY_3D_DEPTH;
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


	public virtual void SetNextMoveCell(Cell cell)
	{
		nextMoveCell = cell;
	}

	public Cell GetNextMoveCell()
	{
		return nextMoveCell;
	}

	public abstract void MakeMove();

	public abstract void AnimateIdle();
	public abstract void ResetMoveAnimation();
	public abstract bool AnimateMove();

}
