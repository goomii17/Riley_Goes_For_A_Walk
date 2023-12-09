using UnityEngine;

public class PlayerAnimator : EntityAnimator
{
	// Move animation
	private int moveAnimationState = 0; // 0 - init, 1 animate move to next cell, 2 animate attack 1, 3 animate attack 2, 4 animate attack 3, 5 finished

	// Walk animation
	private const float WALK_ANIMATION_TIME = 0.35f;
	private float walkAnimationTimer = 0f;
	private int walkAnimationState = 0; // 0 - init, 1 animate walk to next cell, 2 finished

	// Attack animation
	private const float ATTACK_ANIMATION_TIME = 0.3f;
	private const float ATTACK_INTRUSION_PERCENTAGE = 0.65f;
	private float attackAnimationTimer = 0f;
	private int attackAnimationState = 0; // 0 - init, 1 going forward, 2 backwards, 3 finished

	// Common
	private Vector3 startPosition;

	// Evolution
	[SerializeField] private Sprite[] evolutions = null;
	[SerializeField] private Sprite[] evolutions_idle = null;
	private int evolutionState = 0;


	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 1;
		idleAnimationTimer = Random.Range(0f, IDLE_ANIMATION_TIME);
	}

	public override void ResetMoveAnimation()
	{
		moveAnimationState = 0;
		walkAnimationTimer = 0f;
		walkAnimationState = 0;
		attackAnimationTimer = 0f;
		attackAnimationState = 0;
		spriteRenderer.sprite = IdleSprites[0];
	}

	public bool AnimateMove(Cell targetCell, Cell killMeleeLeft, Cell killMeleeRight, Cell killFrontal)
	{
		if (targetCell == null)
		{
			return true;
		}

		switch (moveAnimationState)
		{
			case 0:
				startPosition = transform.position;
				moveAnimationState = 1;
				return false;
			case 1:
				if (AnimateWalk(startPosition, targetCell.GetEntityPosition()))
				{
					startPosition = targetCell.GetEntityPosition();
					ResetAttackAnimation();
					moveAnimationState = 2;
				}
				return false;
			case 2:
				if (AnimateAttack(killMeleeLeft))
				{
					ResetAttackAnimation();
					moveAnimationState = 3;
				}
				return false;
			case 3:
				if (AnimateAttack(killMeleeRight))
				{
					ResetAttackAnimation();
					moveAnimationState = 4;
				}
				return false;
			case 4:
				if (AnimateAttack(killFrontal))
				{
					moveAnimationState = 5;
				}
				return false;
			case 5:
				return true;

			default: return true;
		}
	}

	private bool AnimateWalk(Vector3 start, Vector3 end)
	{
		switch (walkAnimationState)
		{
			case 0:
				walkAnimationState = 1;
				return false;
			case 1:
				walkAnimationTimer += Time.deltaTime;

				Vector3 newPosition = Vector3.Lerp(start, end, walkAnimationTimer / WALK_ANIMATION_TIME);
				transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

				if (walkAnimationTimer > WALK_ANIMATION_TIME)
				{
					walkAnimationState = 2;
				}

				return false;
			case 2:
				return true;

			default: return true;
		}
	}

	private void ResetAttackAnimation()
	{
		attackAnimationTimer = 0f;
		attackAnimationState = 0;
	}

	private bool AnimateAttack(Cell enemyCell)
	{
		if (enemyCell == null)
		{
			return true;
		}

		switch (attackAnimationState)
		{
			case 0:
				attackAnimationState = 1;
				return false;
			case 1:
				attackAnimationTimer += Time.deltaTime;

				Vector3 targetPosition = startPosition + (enemyCell.GetEntityPosition() - startPosition) * ATTACK_INTRUSION_PERCENTAGE;
				transform.position = Vector3.Lerp(startPosition, targetPosition, 2 * attackAnimationTimer / ATTACK_ANIMATION_TIME);

				if (attackAnimationTimer > ATTACK_ANIMATION_TIME / 2)
				{
					enemyCell.entity.GetComponent<SpriteRenderer>().color = Color.magenta;
					attackAnimationState = 2;
				}

				return false;
			case 2:
				attackAnimationTimer += Time.deltaTime;

				Vector3 targetPosition2 = startPosition + (enemyCell.GetEntityPosition() - startPosition) * ATTACK_INTRUSION_PERCENTAGE;
				transform.position = Vector3.Lerp(targetPosition2, startPosition, 2 * (attackAnimationTimer / ATTACK_ANIMATION_TIME - 0.5f));

				if (attackAnimationTimer > ATTACK_ANIMATION_TIME)
				{
					enemyCell.entity.GetComponent<SpriteRenderer>().color = Color.white;
					attackAnimationState = 3;
				}

				return false;
			case 3:
				return true;

			default: return true;
		}
	}

	public void Evolve()
	{
		if (evolutionState > evolutions.Length - 1)
		{
			Debug.Log("Evolution state is already maxed out!");
			return;
		}
		IdleSprites = new Sprite[2];
		IdleSprites[0] = evolutions[evolutionState];
		IdleSprites[1] = evolutions_idle[evolutionState];
		spriteRenderer.sprite = IdleSprites[0];
		evolutionState++;
	}

}