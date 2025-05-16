using UnityEngine;

public class EnemyStats : CharacterStats
{
    [SerializeField] EnemyDataSO enemyStats;

    private void Start()
    {
        WalkSpeed = enemyStats.walkSpeed;
        RunSpeed = enemyStats.runSpeed;
        GroundLayers = enemyStats.groundLayers;
    }
}