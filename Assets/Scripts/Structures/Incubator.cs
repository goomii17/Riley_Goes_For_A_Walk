using UnityEngine;

public class Incubator : Structure
{

	[SerializeField] private Sprite filledSprite;
	[SerializeField] private Sprite drainedSprite;

	private bool filled = true;

	public Incubator() : base()
	{
		Type = StructureType.Incubator;
	}

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = filledSprite;
	}

	public void Drain()
	{
		spriteRenderer.sprite = drainedSprite;
		filled = false;
	}

	public bool IsFilled()
	{
		return filled;
	}

}
