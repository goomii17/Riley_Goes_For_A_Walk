using UnityEngine;

public class HazmatAnimator : EnemyAnimator
{
	// Attack animation
	private const float ATTACK_ANIMATION_TIME = 0.35f;
	private const float ATTACK_INTRUSION_PERCENTAGE = 0.6f;
	private float attackAnimationTimer = 0f;
	private int attackAnimationState = 0; // 0 - off, 1 going forward, 2 backwards, 3 finished
	private Vector3 startAttackPosition;

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		idleAnimationTimer = Random.Range(0f, IDLE_ANIMATION_TIME);
	}

	public override void ResetAttackAnimation()
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
				startAttackPosition = transform.position;
				attackAnimationState = 1;
				return false;
			case 1:
				attackAnimationTimer += Time.deltaTime;

				Vector3 targetPosition = startAttackPosition + (playerCell.GetEntityPosition() - startAttackPosition) * ATTACK_INTRUSION_PERCENTAGE;
				transform.position = Vector3.Lerp(startAttackPosition, targetPosition, 2 * attackAnimationTimer / ATTACK_ANIMATION_TIME); ;

				if (attackAnimationTimer > ATTACK_ANIMATION_TIME / 2)
				{
					playerCell.entity.GetComponent<SpriteRenderer>().color = Color.red;
					attackAnimationState = 2;
				}

				return false;
			case 2:
				attackAnimationTimer += Time.deltaTime;

				Vector3 targetPosition2 = startAttackPosition + (playerCell.GetEntityPosition() - startAttackPosition) * ATTACK_INTRUSION_PERCENTAGE;
				transform.position = Vector3.Lerp(targetPosition2, startAttackPosition, 2 * (attackAnimationTimer / ATTACK_ANIMATION_TIME - 0.5f)); ;

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