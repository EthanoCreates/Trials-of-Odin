using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    private float walkSpeed;
    private Vector3 walkPoint;
    private Vector3 enemyPosExclY;
    private Vector3 walkPointExclY;
    private float walkPointRange;
    private bool walkPointSet;
    private float turnSpeed = 5f;
    private bool goIdle;
    private float stoppingDistance;
    private Transform transform;

    public EnemyPatrolState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    public override void EnterState()
    {
        PatrolSetUp();
    }

    public override void ExitState() { }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) return EnemyStateMachine.EEnemyState.Stunned;
        if (enemyContext.NearPlayers.Count != 0) return EnemyStateMachine.EEnemyState.Chase;
        if (goIdle) return EnemyStateMachine.EEnemyState.Idle;

        return StateKey;
    }

    public override void UpdateState()
    {
        Patrolling();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            enemyAnimatorRequestor.AnimateMovement(walkSpeed);
            enemyMovementUtility.SmoothLookAt(walkPoint, turnSpeed);

            enemyPosExclY = transform.position.Flatten();

            float distanceToWalkPoint = Vector3.Distance(enemyPosExclY, walkPointExclY);
            
            if (distanceToWalkPoint < stoppingDistance)
            {
                walkPointSet = false;
                int rand = Random.Range(0, 2);
                if (rand == 0) goIdle = true;
            }
        }
    }

    private void SearchWalkPoint()
    {
        bool validPointFound = false;
        int maxAttempts = 10; // Limit attempts to prevent infinite loops
        int attemptCount = 0;

        while (!validPointFound && attemptCount < maxAttempts)
        {
            attemptCount++;

            int positiveChance = Random.Range(0, 2);
            float multiplier = positiveChance == 1 ? 1 : -1;

            float randomZ = Random.Range(2, walkPointRange) * multiplier;
            float randomX = Random.Range(2, walkPointRange) * multiplier;

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // Check if the position is on the NavMesh
            if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, walkPointRange, NavMesh.AllAreas))
            {
                // Verify if the point is reachable
                NavMeshPath path = new();
                if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        walkPoint = hit.position;
                        walkPointExclY.x = walkPoint.x;
                        walkPointExclY.z = walkPoint.z;
                        walkPointSet = true;
                        enemyMovementUtility.MoveToPosition(walkPoint, walkSpeed);
                        validPointFound = true;
                    }
                }
            }
        }

        if (!validPointFound)
        {
            Debug.LogWarning("Unable to find a valid walk point.");
            goIdle = true;
            walkPointSet = false;
        }
    }

    private void PatrolSetUp()
    {
        walkPointSet = false;
        walkSpeed = enemyContext.WalkSpeed;
        walkPointRange = enemyContext.WalkPointRange;
        stoppingDistance = enemyContext.StoppingDistance;
        goIdle = false;
        transform = enemyMovementUtility.transform;
    }
}
