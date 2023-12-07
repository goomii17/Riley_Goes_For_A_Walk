using System.Collections.Generic;

public class Hazmat : Enemy
{

	public override void UpdateNextAttack()
	{
		foreach (Cell cell in GetAttackableCells())
		{
			if (cell.content != null && cell.content.Type == EntityType.Player)
			{
				NextAttackCell = cell;
				return;
			}
		}
		NextAttackCell = null;
	}

	public override bool AnimateAttack()
	{
		// no attack animation
		return true;
	}

	public override void ResetAttackAnimation()
	{
		// no attack animation
	}

	public override bool AttackPlayer()
	{
		if (NextAttackCell == null) return false;
		return true;
	}

	public override List<Cell> GetAttackableCells()
	{
		// Hazmat is a melee enemy, returns cells with no content or player
		List<Cell> attackableCells = new List<Cell>();
		foreach (Cell cell in currentCell.GetNeighbors())
		{
			if (cell.content == null || cell.content.Type == EntityType.Player)
			{
				attackableCells.Add(cell);
			}
		}
		return attackableCells;
	}

}
