using UnityEngine;

public class SwatAnimator : EnemyAnimator
{
	// Bullet shot audio
	[SerializeField] AudioClip bulletShotAudio = null;

	// Prefab bullet
	[SerializeField] GameObject bulletPrefab = null;

	// Attack animation
	private const float IMPACT_ANIMATION_TIME = 0.35f;
	private const float BULLET_SPEED = 200f;
	private float impactAnimationTimer = 0f;
	private Vector3 startPosition;
	GameObject bullet;
	private int attackAnimationState = 0; // 0 - off, 1 going forward, 2 backwards, 3 finished

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		idleAnimationTimer = Random.Range(0f, IDLE_ANIMATION_TIME);
	}

	public override void ResetAttackAnimation()
	{
		impactAnimationTimer = 0f;
		attackAnimationState = 0;
	}

	public override bool AnimateAttack(Cell playerCell)
	{
		if (playerCell == null)
		{
			return true;
		}

		switch (attackAnimationState)
		{
			case 0:
				// Start animation
				startPosition = transform.position;
				attackAnimationState = 1;
				return false;
			case 1:
				// Create bullet in start position
				bullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
				bullet.transform.SetParent(transform, false);
				bullet.GetComponent<SpriteRenderer>().sortingOrder = 250 - (int)transform.position.y + 1;
				// rotate bullet to player
				Vector3 direction = playerCell.GetEntityPosition() - startPosition;
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

				// Play bullet shot audio
				AudioSource.PlayClipAtPoint(bulletShotAudio, Camera.main.transform.position);

				attackAnimationState = 2;
				return false;
			case 2:
				// Move bullet
				Vector3 targetPosition = playerCell.GetEntityPosition();
				bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, targetPosition, BULLET_SPEED * Time.deltaTime);

				// Check if bullet reached player
				if (Vector3.Distance(bullet.transform.position, targetPosition) < 0.1f)
				{
					// destroy bullet and paint player red
					Destroy(bullet);
					playerCell.entity.GetComponent<SpriteRenderer>().color = Color.red;
					attackAnimationState = 3;
				}
				return false;
			case 3:
				// paint player red for IMPACT_ANIMATION_TIME seconds
				impactAnimationTimer += Time.deltaTime;
				if (impactAnimationTimer >= IMPACT_ANIMATION_TIME)
				{
					playerCell.entity.GetComponent<SpriteRenderer>().color = Color.white;
					attackAnimationState = 4;
					return false;
				}
				else
				{
					return false;
				}
			case 4:
				return true;
			default: return true;
		}

	}

}