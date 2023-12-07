public class Elevator : Entity
{
	public Elevator() : base()
	{
		Type = EntityType.Elevator;
	}

	public override void FindNextMove()
	{
		// Elevators don't move
	}

}
