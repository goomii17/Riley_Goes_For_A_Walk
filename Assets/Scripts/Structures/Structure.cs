using UnityEngine;

public enum StructureType
{
	Elevator,
	Incubator,
	None
}

public class Structure : MonoBehaviour
{
	public StructureType Type { get; protected set; }

	public virtual void PositionIn3D()
	{
		Vector3 startPosition = Vector3.up * 10f - Vector3.forward * 3.8f;
		Quaternion startRotation = Quaternion.Euler(-20f, 0, 0);
		transform.SetLocalPositionAndRotation(startPosition, startRotation);
	}

	public void SetParentCell(Cell cell)
	{
		transform.SetParent(cell.gameObject.transform, false);
	}

}
