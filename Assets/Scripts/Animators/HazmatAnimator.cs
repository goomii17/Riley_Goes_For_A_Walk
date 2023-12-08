using UnityEngine;

public class HazmatAnimator : EntityAnimator
{
	// Walk animation
	[SerializeField] Sprite[] MoveSprites = null;
	private const float MOVE_ANIMATION_TIME = 0.4f;
	private const float MOVE_SPRITE_TIME = 0.2f;
	private float moveAnimationTimer = 0f;
	private float moveSpriteTimer = 0f;
	private int moveSpriteIndex = 0;
	private float startMoveX;
	private float startMoveY;
	private bool isMoving = false;

	// Attack animation
	private const float ATTACK_ANIMATION_TIME = 0.35f;
	private const float ATTACK_INTRUSION_PERCENTAGE = 0.6f;
	private float attackAnimationTimer = 0f;
	private Vector3 startPosition;
	private int attackAnimationState = 0; // 0 - off, 1 going forward, 2 backwards, 3 finished

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		idleAnimationTimer = Random.Range(0f, IDLE_ANIMATION_TIME);
	}

	public override void ResetMoveAnimation()
	{
		moveAnimationTimer = 0f;
		isMoving = false;
		spriteRenderer.sprite = IdleSprites[0];
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

			if (moveSpriteTimer >= MOVE_SPRITE_TIME / IdleSprites.Length)
			{
				moveSpriteTimer = 0f;
				moveSpriteIndex++;
				if (moveSpriteIndex >= IdleSprites.Length)
				{
					moveSpriteIndex = 0;
				}
				spriteRenderer.sprite = IdleSprites[moveSpriteIndex];
			}
			else
			{
				moveSpriteTimer += Time.deltaTime;
			}

			return false;
		}
	}

	public void ResetAttackAnimation()
	{
		attackAnimationTimer = 0f;
		attackAnimationState = 0;
	}

	public bool AnimateAttack(Cell playerCell)
	{
		if (playerCell == null)
		{
			return true;
		}

		switch (attackAnimationState)
		{
			case 0:
				startPosition = transform.position;
				attackAnimationState = 1;
				return false;
			case 1:
				attackAnimationTimer += Time.deltaTime;

				Vector3 targetPosition = startPosition + (playerCell.GetPosition() - startPosition) * ATTACK_INTRUSION_PERCENTAGE;
				Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, 2 * attackAnimationTimer / ATTACK_ANIMATION_TIME);
				transform.position = newPosition;

				if (attackAnimationTimer > ATTACK_ANIMATION_TIME / 2)
				{
					playerCell.entity.GetComponent<SpriteRenderer>().color = Color.red;
					attackAnimationState = 2;
				}

				return false;
			case 2:
				attackAnimationTimer += Time.deltaTime;

				Vector3 targetPosition2 = startPosition + (playerCell.GetPosition() - startPosition) * ATTACK_INTRUSION_PERCENTAGE;
				Vector3 newPosition2 = Vector3.Lerp(targetPosition2, startPosition, 2 * (attackAnimationTimer / ATTACK_ANIMATION_TIME - 0.5f));
				transform.position = newPosition2;

				if (attackAnimationTimer > ATTACK_ANIMATION_TIME)
				{
					playerCell.entity.GetComponent<SpriteRenderer>().color = Color.white;
					attackAnimationState = 3;
				}

				return false;
			case 3:
				return true;

			default: return true;
		}

	}

}