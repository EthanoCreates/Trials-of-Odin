using UnityEngine;

public class EnemyFallingState : EnemyState
{
    private float gravity;
    private float gravityMultiplier;
    private Vector3 fallingVectors = new();
    private float fallSpeed;

    public EnemyFallingState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        FallingSetUp();
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if(enemyContext.IsGrounded) return EnemyStateMachine.EEnemyState.Grounded;

        return StateKey;
    }

    public override void UpdateState()
    {
        fallSpeed = enemyContext.VerticalVelocity += gravity * Time.deltaTime * gravityMultiplier;
        fallingVectors.y = fallSpeed;
        enemyMovementUtility.Agent.Move(fallingVectors);
    }

    private void FallingSetUp()
    {
        gravity = enemyContext.Gravity;
        gravityMultiplier = enemyContext.FallGravityMultiplier;
        enemyAnimatorRequestor.AnimateFalling();
    }
    public override void ExitState() { }
}
