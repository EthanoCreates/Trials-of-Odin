using UnityEngine;

public class HumanoidBossAttackState : EnemyState
{
    public HumanoidBossAttackState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    private Transform transform;
    private Transform closestPlayer;
    private Vector3 playerPos = new(0f, 1.6f, 0f);

    private float closestPlayerDistance = Mathf.Infinity;

    public override void EnterState()
    {
        AttackSetUp();
        PerformAttack();
    }

    private void PerformAttack()
    {
        closestPlayerDistance = enemyMovementUtility.SqrDistance(transform.position, closestPlayer.position);

        if (enemyContext.ChargeAttack)
        {
            enemyContext.ChargeAttack = false;
            enemyAnimatorRequestor.AnimateAttack(3);
        }
        else if (enemyContext.LongRangeAttack)
        {
            enemyContext.LongRangeAttack = false;
            enemyAnimatorRequestor.AnimateSpecialAttack(1);
        }
        else if (closestPlayerDistance < enemyContext.Radius * 2.25)
        {
            int randomAttackWeighting = Random.Range(0, 7);
            PerformCloseAttack(randomAttackWeighting);
        }
        else
        {
            int randomAttackWeighting = Random.Range(0, 7);
            PerformNormalDistanceAttack(randomAttackWeighting);
        }
    }

    private void PerformCloseAttack(int randomAttackWeighting)
    {  
        if (randomAttackWeighting < 3)
        {
            //Stomp - close range attack
            enemyAnimatorRequestor.AnimateSpecialAttack(0);
        }
        else
        {
            switch (randomAttackWeighting)
            {
                case 3: enemyAnimatorRequestor.AnimateAttack(0); break;
                case 4: enemyAnimatorRequestor.AnimateAttack(1); break;
                case 5: enemyAnimatorRequestor.AnimateAttack(2); break;
                case 6: enemyAnimatorRequestor.AnimateSpecialAttack(1); break;
            }
        }
    }

    private void PerformNormalDistanceAttack(int randomAttackWeighting)
    {
        switch (randomAttackWeighting)
        {
            case 0: enemyAnimatorRequestor.AnimateAttackCombo(0); break;
            case 1: enemyAnimatorRequestor.AnimateAttackCombo(0); break;
            case 2: enemyAnimatorRequestor.AnimateSpecialAttack(2); break;
            case 3: enemyAnimatorRequestor.AnimateAttack(0); break;
            case 4: enemyAnimatorRequestor.AnimateAttack(1); break;
            case 5: enemyAnimatorRequestor.AnimateAttack(2); break;
            case 6: enemyAnimatorRequestor.AnimateSpecialAttack(1); break;
        }
    }

    public override void ExitState()
    {
        enemyContext.AttackCooldown = Random.Range(enemyContext.MinTimeBetweenAttacks, enemyContext.MaxTimeBetweenAttacks);
        enemyMovementUtility.Agent.isStopped = false;
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) return EnemyStateMachine.EEnemyState.Stunned;
        if (enemyContext.AnimFinished)
        {
            return EnemyStateMachine.EEnemyState.Chase;
        }
        return StateKey;
    }

    public override void UpdateState()
    {
        if (closestPlayer == null) return;

        playerPos.x = closestPlayer.position.x;
        playerPos.y = transform.position.y;
        playerPos.z = closestPlayer.position.z;
        enemyMovementUtility.SmoothLookAt(playerPos, 1f);
    }


    private void AttackSetUp()
    {
        enemyAnimatorRequestor.AnimateBlock();
        transform = enemyMovementUtility.transform;
        enemyMovementUtility.Agent.isStopped = true;
        closestPlayer = enemyContext.ClosestPlayer;
    }
}
