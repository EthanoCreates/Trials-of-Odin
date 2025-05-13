using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private float runSpeed;
    private float walkSpeed;
    private float idleSpeed = 0f;
    private float turnSpeed = 10f;
    private bool canAttack;
    private bool attack;
    private float attackDistanceSqr;
    private bool block;
    private float blockDistanceSqr;
    private Transform transform;

    private Transform closestPlayer;
    private float closestPlayerDistanceSqr = Mathf.Infinity;

    public EnemyChaseState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    public override void EnterState()
    {
        ChaseSetUp();
    }

    public override void ExitState() { }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) return EnemyStateMachine.EEnemyState.Stunned;
        if (enemyContext.NearPlayers.Count == 0) { enemyContext.LastPlayerPos = closestPlayer.position; return EnemyStateMachine.EEnemyState.Search; }
        if (attack) return EnemyStateMachine.EEnemyState.Attack;
        if (block) return EnemyStateMachine.EEnemyState.Block;
        return StateKey;
    }

    public override void UpdateState()
    {
        Chase(FindClosestPlayer());
        CheckForTransition();
        CheckForAttack();
    }

    private void Chase(Transform playerTransform)
    {
        if (closestPlayerDistanceSqr < attackDistanceSqr / 1.44f)
        {
            enemyMovementUtility.MoveToPosition(playerTransform.position, idleSpeed);
        }
        else if (closestPlayerDistanceSqr < blockDistanceSqr)
        {
            enemyMovementUtility.MoveToPosition(playerTransform.position, walkSpeed);
        }
        else
        {
            enemyMovementUtility.MoveToPosition(playerTransform.position, runSpeed);
        }

        enemyMovementUtility.SmoothLookAt(playerTransform.position, turnSpeed);
    }

    private Transform FindClosestPlayer()
    {
        foreach (var player in enemyContext.NearPlayers)
        {
            float distanceToPlayerSqr = enemyMovementUtility.SqrDistance(transform.position, player.transform.position);
            if (distanceToPlayerSqr < closestPlayerDistanceSqr)
            {
                if(closestPlayer != player.transform)
                {
                    enemyContext.ClosestPlayer = closestPlayer = player.transform;
                    enemyContext.ClosestPlayerContext = closestPlayer.GetComponent<PlayerStateMachine>().Context;
                }
                closestPlayerDistanceSqr = distanceToPlayerSqr;
            }
        }

        return closestPlayer;
    }

    private void CheckForTransition()
    {
        if (closestPlayerDistanceSqr < attackDistanceSqr) canAttack = true;
        else canAttack = false;

        if (closestPlayerDistanceSqr < blockDistanceSqr) block = true;
    }

    private void CheckForAttack()
    {
        if (!canAttack) return;

        enemyContext.AttackCooldown -= Time.deltaTime;
        if (enemyContext.AttackCooldown < 0)
        {
            attack = true;
        }
    }

    private void ChaseSetUp()
    {
        attack = false;
        canAttack = false;
        block = false;

        runSpeed = enemyContext.RunSpeed;
        walkSpeed = enemyContext.WalkSpeed;
        transform = enemyMovementUtility.transform;

        attackDistanceSqr = enemyContext.AttackRange * enemyContext.AttackRange;
        blockDistanceSqr = enemyContext.BlockRange * enemyContext.BlockRange;

        FindClosestPlayer();
        closestPlayerDistanceSqr = enemyMovementUtility.SqrDistance(transform.position, closestPlayer.position);
    }
}
