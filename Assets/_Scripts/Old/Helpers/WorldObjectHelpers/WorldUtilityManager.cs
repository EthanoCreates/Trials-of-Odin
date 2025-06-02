using UnityEngine;

public class WorldUtilityManager : Singleton<WorldUtilityManager>
{
    [SerializeField] private LayerMask enemyLayer;
    public LayerMask EnemyLayer { get { return enemyLayer; } } 
    [SerializeField] private LayerMask environmentLayer;
    public LayerMask EnvironmentLayer { get { return environmentLayer; } }
    [SerializeField] private LayerMask targetLayer;
    public LayerMask TargetLayer { get { return targetLayer; } }
}
