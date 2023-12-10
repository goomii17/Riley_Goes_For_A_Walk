using UnityEngine;

public class Elevator : Structure
{
	[SerializeField] private Sprite openSprite;
	[SerializeField] private Sprite closedSprite;

	[SerializeField] private AudioClip openSound;

	private float timeOpened = 0f;

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
		spriteRenderer.sortingOrder = 250 - (int)transform.position.y - 1;
	}

	public void Open()
	{
		// Save time in which elevator was opened
		timeOpened = Time.time;
		AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position, 0.8f);
		spriteRenderer.sprite = openSprite;
	}

	public bool FinishedOpening()
	{
		return Time.time - timeOpened > openSound.length;
	}

	public void Close()
	{
		spriteRenderer.sprite = closedSprite;
	}

}
