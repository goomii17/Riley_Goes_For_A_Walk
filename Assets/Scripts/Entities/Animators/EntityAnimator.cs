using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
	// Idle animation
	[SerializeField] Sprite[] IdleSprites = null;
	private const float IDLE_ANIMATION_TIME = 1.0f;
	private const float RandomFlippingChance = 0.0001f;
	private float idleAnimationTimer = 0f;
	private int idleSpriteIndex = 0;

	// Move animation
	private const float MOVE_ANIMATION_TIME = 0.4f;
	private float moveAnimationTimer = 0f;
	private float startMoveX;
	private float startMoveY;
	private bool isMoving = false;

	public void Awake()
	{
		idleAnimationTimer = Random.Range(0f, IDLE_ANIMATION_TIME);
	}

	public void AnimateIdle()
	{
		// Change sprite every 1.0/lenght of IdleSprites
		idleAnimationTimer += Time.deltaTime;

		if (idleAnimationTimer >= IDLE_ANIMATION_TIME / IdleSprites.Length)
		{
			idleAnimationTimer = 0f;
			idleSpriteIndex++;
			if (idleSpriteIndex >= IdleSprites.Length)
			{
				idleSpriteIndex = 0;
			}
			GetComponent<SpriteRenderer>().sprite = IdleSprites[idleSpriteIndex];
		}

		// Randomly flip sprite
		if (Random.Range(0f, 1f) < RandomFlippingChance)
		{
			GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
		}
	}

	public void ResetMoveAnimation()
	{
		moveAnimationTimer = 0f;
		isMoving = false;
	}

	/// <summary>
	/// Animate move from current cell to target cell. Returns true when animation is finished.
	/// </summary>
	/// <param name="targetCell"></param>
	/// <returns></returns>
	public bool AnimateMove(Cell targetCell)
	{
		// Just move to target updating transform position
		moveAnimationTimer += Time.deltaTime;
		if (moveAnimationTimer >= MOVE_ANIMATION_TIME)
		{
			Debug.Log("Move animation finished");
			return true;
		}
		else
		{
			if (isMoving == false)
			{
				isMoving = true;
				startMoveX = transform.position.x;
				startMoveY = transform.position.y;
			}
			var targetX = targetCell.transform.position.x;
			var targetY = targetCell.transform.position.y + 4.6f;

			var newX = startMoveX + (targetX - startMoveX) * moveAnimationTimer / MOVE_ANIMATION_TIME;
			var newY = startMoveY + (targetY - startMoveY) * moveAnimationTimer / MOVE_ANIMATION_TIME;

			transform.position = new Vector3(newX, newY, transform.position.z);
			return false;
		}
	}

}