using UnityEngine;

public class EnemySearchState : EnemyState
{
    public EnemySearchState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    float cancelTimer = 7f;
    float speed;
    bool reachedLastPos = false;
    Vector3 lastPlayerPos;
    float stoppingDistance;
    bool lookedAround;
    bool unEngage;
    Transform transform;
    private float turnSpeed = 5f;

    public override void EnterState()
    {
       SearchSetUp();

        if (enemyContext.IsDamaged && enemyContext.NearPlayers.Count == 0)
        {
            enemyContext.IsDamaged = false;

            lastPlayerPos = enemyContext.suspectedPos;
        }

        enemyMovementUtility.MoveToPosition(lastPlayerPos, speed);
    }

    public override void ExitState()
    {
        if (lookedAround || unEngage) enemyContext.LookAround = true;
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) return EnemyStateMachine.EEnemyState.Stunned;
        if (enemyContext.NearPlayers.Count != 0) return EnemyStateMachine.EEnemyState.Chase;
        if(lookedAround || unEngage) return EnemyStateMachine.EEnemyState.Idle;
        return StateKey;
    }

    public override void UpdateState()
    {
        if (!reachedLastPos)
        {
            cancelTimer -= Time.deltaTime;
            if (cancelTimer < 0) {
                unEngage = true; return; 
            }
            float distanceToWalkPoint = Vector3.Distance(transform.position, lastPlayerPos);

            if (distanceToWalkPoint > stoppingDistance)
            {
                enemyAnimatorRequestor.AnimateMovement(speed);
                enemyMovementUtility.SmoothLookAt(lastPlayerPos, turnSpeed);
            }
            else reachedLastPos = true;
        }
        else  lookedAround = true; 
    }

    private void SearchSetUp()
    {
        cancelTimer = 7f;
        speed = enemyContext.WalkSpeed;
        lastPlayerPos = enemyContext.LastPlayerPos;
        stoppingDistance = enemyContext.StoppingDistance;
        reachedLastPos = false;
        lookedAround = false;
        unEngage = false;
        transform = enemyMovementUtility.transform;
    }
}
