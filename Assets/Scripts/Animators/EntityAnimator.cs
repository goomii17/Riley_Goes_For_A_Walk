using UnityEngine;

public abstract class EntityAnimator : MonoBehaviour
{
	// Sprite renderer
	protected SpriteRenderer spriteRenderer;

	// Idle animation
	[SerializeField] protected Sprite[] IdleSprites = null;
	protected const float IDLE_ANIMATION_TIME = 1.0f;
	protected const float RandomFlippingChance = 0.00025f;
	protected float idleAnimationTimer = 0f;
	protected int idleSpriteIndex = 0;

	public virtual void AnimateIdle()
	{
		// Change sprite
		idleAnimationTimer += Time.deltaTime;

		if (idleAnimationTimer >= IDLE_ANIMATION_TIME / IdleSprites.Length)
		{
			idleAnimationTimer = 0f;
			idleSpriteIndex++;
			if (idleSpriteIndex >= IdleSprites.Length)
			{
				idleSpriteIndex = 0;
			}
			spriteRenderer.sprite = IdleSprites[idleSpriteIndex];
		}

		// Randomly flip sprite
		if (Random.Range(0f, 1f) < RandomFlippingChance)
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}

		// Sorting layer is int(y)
		spriteRenderer.sortingOrder = (int)transform.position.y + 52;
	}

	public abstract void ResetMoveAnimation();

}