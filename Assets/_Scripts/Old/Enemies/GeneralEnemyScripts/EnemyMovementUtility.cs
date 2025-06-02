using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementUtility
{
    private EnemyContext enemyContext;
    private EnemyAnimatorRequestor enemyAnimatorRequestor;

    public NavMeshAgent Agent;
    public Transform transform;
    public LayerMask groundLayers;

    public EnemyMovementUtility(EnemyContext enemyContext, EnemyAnimatorRequestor enemyAnimatorRequestor)
    {
        this.enemyContext = enemyContext;
        this.enemyAnimatorRequestor = enemyAnimatorRequestor;

        Agent = enemyContext.Agent;
        transform = enemyContext.Transform;
        groundLayers = enemyContext.GroundLayers;
    }

    public void MoveToPosition(Vector3 position, float speed)
    {
        Agent.isStopped = false;
        Agent.SetDestination(position);
        Agent.speed = speed;
        enemyAnimatorRequestor.AnimateMovement(speed);
    }

    public void StopMoving()
    {
        Agent.isStopped = true;
    }

    public void SmoothLookAt(Vector3 targetPoint, float rotationSpeed)
    {
        // Determine the target rotation by looking at the walk point
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

        // Smoothly interpolate from the current rotation to the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public float SqrDistance(Vector3 pointA, Vector3 pointB)
    {
        return (pointA.Flatten() - pointB.Flatten()).sqrMagnitude;
    }
}
