using UnityEngine;

public class EnemyGroundedState : EnemyState
{
    private float timeTillFall;
    private float timerTillFall;

    private Vector3 fallingVectors = new();
    private float fallSpeed;
    private float gravity;
    private bool isFalling;

    public EnemyGroundedState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GroundedSetUp();
        InitializeSubState();
    }

    public override void ExitState() { }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (isFalling) return EnemyStateMachine.EEnemyState.Falling;

        return StateKey;
    }

    private void InitializeSubState()
    {
        SetSubState(enemyContext.EnemyStateMachine.GetStateInstance(EnemyStateMachine.EEnemyState.Land));
    }

    public override void UpdateState()
    {
        if (enemyContext.IsGrounded) { timerTillFall = timeTillFall; return; }

        if (timerTillFall >= 0.0f)
        {
            timerTillFall -= Time.deltaTime;

            fallSpeed = enemyContext.VerticalVelocity += gravity * Time.deltaTime;
            fallingVectors.y = fallSpeed;
            enemyMovementUtility.Agent.Move(fallingVectors);
        }
        else
        {
            isFalling = true;
        }
    }

    private void GroundedSetUp()
    {
        gravity = enemyContext.Gravity;
        timerTillFall = enemyContext.FallTimeout;
        timeTillFall = timerTillFall;
        isFalling = false;
    }
}
