using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    private Transform transform;
    private Transform closestPlayer;
    private Vector3 playerPos = new(0f, 1.6f, 0f);

    public override void EnterState()
    {
        transform = enemyMovementUtility.transform;
        enemyMovementUtility.Agent.isStopped = true;
        int attackNum;
        int randomAttack = Random.Range(0, 7);

        if (randomAttack > 5)
        {
            attackNum = Random.Range(0, enemyContext.PrimaryWeapon.EnemyCombos.Count);
            enemyAnimatorRequestor.AnimateAttackCombo(attackNum);
        }
        else if (randomAttack > 2)
        {
            attackNum = Random.Range(0, enemyContext.PrimaryWeapon.SpecialEnemyAttacks.Count);
            enemyAnimatorRequestor.AnimateSpecialAttack(attackNum);
        }
        else
        {
            attackNum = Random.Range(0, enemyContext.PrimaryWeapon.EnemyAttacks.Count);
            enemyAnimatorRequestor.AnimateAttack(attackNum);
        }

        closestPlayer = enemyContext.ClosestPlayer;
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
        if(closestPlayer == null) return;
        playerPos.x = closestPlayer.position.x;
        playerPos.y = transform.position.y;
        playerPos.z = closestPlayer.position.z;
        enemyMovementUtility.SmoothLookAt(playerPos, 1f);
    }
}
