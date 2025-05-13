using UnityEngine;

public class HumanoidBossBlockState : EnemyState
{
    private float walkSpeed;
    private float turnSpeed = 10f;

    private float attackDistanceSqr;
    private float blockDistanceSqr;
    private float closestPlayerDistanceSqr = Mathf.Infinity;

    private bool block;
    private bool canAttack;
    private bool attack;

    private Transform closestPlayer;
    private Transform transform;

    public HumanoidBossBlockState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }
    public override void EnterState()
    {
        SetUpBlock();
        enemyContext.Shield.EnableShield();
        enemyAnimatorRequestor.AnimateBlock();
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) { return EnemyStateMachine.EEnemyState.Stunned; }

        if (attack) return EnemyStateMachine.EEnemyState.Attack;
        if (!block) return EnemyStateMachine.EEnemyState.Chase;

        return StateKey;
    }

    public override void UpdateState()
    {
        if (closestPlayer == null) { block = false; return; }

        closestPlayerDistanceSqr = enemyMovementUtility.SqrDistance(transform.position, closestPlayer.position);

        MoveType();

        enemyMovementUtility.SmoothLookAt(closestPlayer.position, turnSpeed);

        CheckForTransition();

        CheckForAttack();
    }

    private void MoveType()
    {
        if (closestPlayerDistanceSqr < attackDistanceSqr / 2.25f)
        {
            enemyMovementUtility.StopMoving();
        }
        else
        {
            enemyMovementUtility.MoveToPosition(closestPlayer.position, walkSpeed);
        }
    }

    private void CheckForTransition()
    {
        if (closestPlayerDistanceSqr > blockDistanceSqr)
        {
            block = false;
        }

        if (closestPlayerDistanceSqr < attackDistanceSqr)
        {
            canAttack = true;
        }
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

    public override void ExitState()
    {
        enemyAnimatorRequestor.AnimateExitBlock();
        enemyContext.Shield.DisableShield();
    }

    private void SetUpBlock()
    {
        block = true;
        attack = false;
        canAttack = false;

        walkSpeed = enemyContext.WalkSpeed;
        closestPlayer = enemyContext.ClosestPlayer;
        transform = enemyMovementUtility.transform;

        attackDistanceSqr = enemyContext.AttackRange * enemyContext.AttackRange;
        blockDistanceSqr = enemyContext.BlockRange * enemyContext.BlockRange;

        closestPlayerDistanceSqr = enemyMovementUtility.SqrDistance(transform.position, closestPlayer.position);
    }
}
