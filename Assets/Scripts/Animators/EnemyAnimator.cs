using UnityEngine;

public abstract class EnemyAnimator : EntityAnimator
{
	// Walk animation
	private const float WALK_ANIMATION_TIME = 0.35f;
	private float walkAnimationTimer = 0f;
	private int walkAnimationState = 0; // 0 - init, 1 animate walk to next cell, 2 finished
	[SerializeField] AudioClip footSteps;


	private Vector3 startWalkPosition;

	public override void ResetMoveAnimation()
	{
		walkAnimationTimer = 0f;
		walkAnimationState = 0;
		spriteRenderer.sprite = IdleSprites[0];
	}

	public bool AnimateMove(Cell targetCell)
	{
		if (targetCell == null)
		{
			return true;
		}
		//AudioSource.PlayClipAtPoint(footSteps, Camera.main.transform.position, 0.5f);
		return AnimateWalk(targetCell.GetEntityPosition());
	}

	private bool AnimateWalk(Vector3 end)
	{
		switch (walkAnimationState)
		{
			case 0:
				startWalkPosition = transform.position;
				walkAnimationState = 1;
				return false;
			case 1:
				walkAnimationTimer += Time.deltaTime;

				transform.position = Vector3.Lerp(startWalkPosition, end, walkAnimationTimer / WALK_ANIMATION_TIME);

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

	public abstract void ResetAttackAnimation();
	public abstract bool AnimateAttack(Cell playerCell);
}