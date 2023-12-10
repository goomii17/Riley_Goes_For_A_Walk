using UnityEngine;

public class SwatAnimator : EntityAnimator
{
	// Prefab bullet
	[SerializeField] GameObject bulletPrefab = null;

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
	private const float IMPACT_ANIMATION_TIME = 0.35f;
	private const float BULLET_SPEED = 150f;
	private float impactAnimationTimer = 0f;
	private Vector3 startPosition;
	GameObject bullet;
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
		impactAnimationTimer = 0f;
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
			case 0: // Start animation
				startPosition = transform.position;
				attackAnimationState = 1;
				return false;
			case 1:
				// Create bullet in start position
				bullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
				bullet.transform.SetParent(transform, false);
				bullet.transform.localScale = new Vector3(1f, 1f, 1f);
				// rotate bullet to player
				Vector3 direction = playerCell.transform.position - startPosition;
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				// end case
				attackAnimationState = 2;
				return false;
			case 2:
				// Move bullet
				Vector3 targetPosition = playerCell.transform.position;

				bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, targetPosition, BULLET_SPEED * Time.deltaTime);

				Debug.Log("bullet.transform.position: " + bullet.transform.position);
				Debug.Log("targetPosition: " + targetPosition);

				// Check if bullet reached player
				if (Vector3.Distance(bullet.transform.position, targetPosition) < 0.1f)
				{
					// destroy bullet
					Destroy(bullet);
					playerCell.entity.GetComponent<SpriteRenderer>().color = Color.red;
					attackAnimationState = 3;
					return false;
				}
				else
				{
					return false;
				}
			case 3:

				//paint player red for IMPACT_ANIMATION_TIME

				impactAnimationTimer += Time.deltaTime;
				if (impactAnimationTimer >= IMPACT_ANIMATION_TIME)
				{
					attackAnimationState = 4;
					return false;
				}
				else
				{
					return false;
				}
			case 4:
				// end animation
				playerCell.entity.GetComponent<SpriteRenderer>().color = Color.white;
				return true;
			default: return true;
		}

	}

}