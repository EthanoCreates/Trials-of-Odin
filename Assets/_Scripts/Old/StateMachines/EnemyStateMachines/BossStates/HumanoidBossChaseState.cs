using TrialsOfOdin.State;
using UnityEngine;

public class HumanoidBossChaseState : EnemyState
{
    private float runSpeed;
    private float walkSpeed;
    private float idleSpeed = 0f;
    private float turnSpeed = 10f;

    private float radius;
    private float attackDistanceSqr;
    private float blockDistanceSqr;
    private float closestPlayerDistanceSqr = Mathf.Infinity;

    private bool block;
    private bool canblock;
    private bool canAttack;
    private bool attack;

    private bool isCharging;
    private bool closingDistance;

    private float chaseTimer = 10f;

    private Transform closestPlayer;
    private Transform transform;
    public HumanoidBossChaseState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    public override void EnterState()
    {
        ChaseSetUp();
        ChaseVariationSetup();
    }

    public override void ExitState() { }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) return EnemyStateMachine.EEnemyState.Stunned;
        if (enemyContext.NearPlayers.Count == 0) { enemyContext.LastPlayerPos = closestPlayer.position; return EnemyStateMachine.EEnemyState.Search; }
        if (attack) 
        {
            if (isCharging) { 
                enemyAnimatorRequestor.AnimateCharge(); 
            }
            return EnemyStateMachine.EEnemyState.Attack; 
        }
        if (block) return EnemyStateMachine.EEnemyState.Block;
        return StateKey;
    }

    public override void UpdateState()
    {
        Chase(FindClosestPlayer());
        CheckForTransition();
        CheckForAttack();
        ChaseTime();
    }

    private void Chase(Transform playerTransform)
    {
        if (closestPlayerDistanceSqr < attackDistanceSqr / 2.25f)
        {
            enemyMovementUtility.MoveToPosition(playerTransform.position, idleSpeed);
        }
        else if (closestPlayerDistanceSqr < blockDistanceSqr / 2.25f)
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
                if (closestPlayer != player.transform)
                {
                    enemyContext.ClosestPlayer = closestPlayer = player.transform;
                    //enemyContext.ClosestPlayerContext = closestPlayer.GetComponent<PlayerStateMachine>().Context;
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

        if (closestPlayerDistanceSqr < blockDistanceSqr && canblock) block = true;
    }

    private void CheckForAttack()
    {
        if (!canAttack) return;

        enemyContext.AttackCooldown -= Time.deltaTime;
        if (enemyContext.AttackCooldown < 0 || isCharging || enemyContext.LongRangeAttack)
        {
            attack = true;
        }
    }

    private void ChaseTime()
    {
        if (closingDistance) return;

        chaseTimer -= Time.deltaTime;
        if(chaseTimer < 0)
        {
            closingDistance = true;
            enemyContext.ChargeAttack = false;
            enemyContext.LongRangeAttack = false;

            CloseDistance();
        }
    }

    private void ChaseSetUp()
    {
        chaseTimer = 10f;

        transform = enemyMovementUtility.transform;
        radius = enemyContext.Radius;

        attackDistanceSqr = enemyContext.AttackRange * enemyContext.AttackRange;
        blockDistanceSqr = enemyContext.BlockRange * enemyContext.BlockRange;

        FindClosestPlayer();
        closestPlayerDistanceSqr = enemyMovementUtility.SqrDistance(transform.position, closestPlayer.position);
    }

    private void ResetChaseVariation()
    {
        runSpeed = enemyContext.RunSpeed;
        walkSpeed = enemyContext.WalkSpeed;

        attack = false;
        canAttack = false;
        block = false;
        canblock = false;
        isCharging = false;
        closingDistance = false;

        enemyContext.ChargeAttack = false;
        enemyContext.LongRangeAttack = false;
    }

    private void ChaseVariationSetup()
    {
        if (closestPlayerDistanceSqr > radius * 8)
        {
            CloseDistance();
        }
        else
        {
            ResetChaseVariation();
            int rand = Random.Range(0, 2);
            if (rand < 1) canblock = true;
            else if (enemyContext.Health.CurrentHealth < enemyContext.Health.MaxHealth / 2) canblock = true;
        }

    }

    private void CloseDistance()
    {
        ResetChaseVariation();
        int rand = Random.Range(0, 3);

        if (rand < 1)
        {
            canAttack = true;
            enemyContext.LongRangeAttack = true;
            enemyContext.AttackCooldown = -1f;

        }
        else if (rand < 2)
        {
            walkSpeed = runSpeed;
            enemyContext.ChargeAttack = true;
            canblock = false;
            isCharging = true;
        }
        else
        {
            if (enemyContext.Health.CurrentHealth < enemyContext.Health.MaxHealth / 4)
            {
                canblock = true;
            }
            else
            {
                //close distance by running
                walkSpeed = runSpeed;
            }
        }
    }
}

