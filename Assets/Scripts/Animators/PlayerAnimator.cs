using UnityEngine;

public class PlayerAnimator : EntityAnimator
{
	// Move animation
	[SerializeField] Sprite[] MoveSprites = null;
	private const float MOVE_ANIMATION_TIME = 0.45f;
	private float moveAnimationTimer = 0f;
	private const float MOVE_SPRITE_TIME = 0.15f;
	private float moveSpriteTimer = 0f;
	private int moveSpriteIndex = 0;
	private float startMoveX;
	private float startMoveY;
	private bool isMoving = false;

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		idleAnimationTimer = Random.Range(0f, IDLE_ANIMATION_TIME);
	}

	public override void ResetMoveAnimation()
	{
		moveAnimationTimer = 0f;
		isMoving = false;
	}

	/// <summary>
	/// Animate move from current cell to target cell. Returns true when animation is finished.
	/// </summary>
	/// <param name="targetCell"></param>
	/// <returns></returns>
	public override bool AnimateMove(Cell targetCell)
	{
		// Just move to target updating transform position
		moveAnimationTimer += Time.deltaTime;
		if (moveAnimationTimer >= MOVE_ANIMATION_TIME)
		{
			spriteRenderer.sprite = IdleSprites[0];
			// Delay
			if (moveAnimationTimer >= MOVE_ANIMATION_TIME + 0.1f)
			{
				return true;
			}
			return false;
		}
		else
		{
			if (isMoving == false)
			{
				isMoving = true;
				startMoveX = transform.position.x;
				startMoveY = transform.position.y;

				// Check for flip
				float dx = targetCell.transform.position.x - transform.position.x;
				if (dx < 0)
				{
					spriteRenderer.flipX = true;
				}
				else if (dx > 0)
				{
					spriteRenderer.flipX = false;
				}
			}
			var targetX = targetCell.transform.position.x;
			var targetY = targetCell.transform.position.y + 4.6f;

			var newX = startMoveX + (targetX - startMoveX) * moveAnimationTimer / MOVE_ANIMATION_TIME;
			var newY = startMoveY + (targetY - startMoveY) * moveAnimationTimer / MOVE_ANIMATION_TIME;

			transform.position = new Vector3(newX, newY, transform.position.z);

			if (moveSpriteTimer >= MOVE_SPRITE_TIME / MoveSprites.Length)
			{
				moveSpriteTimer = 0f;
				moveSpriteIndex++;
				if (moveSpriteIndex >= MoveSprites.Length)
				{
					moveSpriteIndex = 0;
				}
				spriteRenderer.sprite = MoveSprites[moveSpriteIndex];
			}
			else
			{
				moveSpriteTimer += Time.deltaTime;
			}

			return false;
		}
	}
}