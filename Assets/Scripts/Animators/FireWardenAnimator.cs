using UnityEngine;

public class FireWardenAnimator : EnemyAnimator
{
	// fire shot audio
	[SerializeField] AudioClip fireAudio = null;

	// Prefab fire
	[SerializeField] GameObject firePrefab = null;

	// Attack animation
	private const float IMPACT_ANIMATION_TIME = 0.35f;
	private const float FIRE_SPEED = 100f;
	private float impactAnimationTimer = 0f;
	private Vector3 startPosition;
	GameObject fire;
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
				attackAnimationState = 1;
				return false;
			case 1:
				// Create fire in start position
				fire = Instantiate(firePrefab, Vector3.zero, Quaternion.identity);
				fire.transform.SetParent(transform, false);
				fire.GetComponent<SpriteRenderer>().sortingOrder = 250 - (int)transform.position.y + 1;
				// rotate fire to player
				Vector3 direction = playerCell.GetEntityPosition() - startPosition;
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				fire.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

				// Play fire shot audio
				AudioSource.PlayClipAtPoint(fireAudio, Camera.main.transform.position);

				attackAnimationState = 2;
				return false;
			case 2:
				// Move fire
				Vector3 targetPosition = playerCell.GetEntityPosition();
				fire.transform.position = Vector3.MoveTowards(fire.transform.position, targetPosition, FIRE_SPEED * Time.deltaTime);

				// Check if fire reached player
				if (Vector3.Distance(fire.transform.position, targetPosition) < 0.1f)
				{
					// destroy fire and paint player red
					Destroy(fire);
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