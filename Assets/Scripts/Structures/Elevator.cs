using UnityEngine;

public class Elevator : Structure
{
	[SerializeField] private Sprite openSprite;
	[SerializeField] private Sprite closedSprite;

	public Elevator() : base()
	{
		Type = StructureType.Elevator;
	}

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = closedSprite;
	}

	public override void PositionIn3D()
	{
		Vector3 startPosition = Vector3.up * 12f - Vector3.forward * 3.8f;
		Quaternion startRotation = Quaternion.Euler(-20f, 0, 0);
		transform.SetLocalPositionAndRotation(startPosition, startRotation);
	}

	public void Open()
	{
		spriteRenderer.sprite = openSprite;
	}

	public void Close()
	{
		spriteRenderer.sprite = closedSprite;
	}

}
