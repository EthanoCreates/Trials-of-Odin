using UnityEngine;

namespace TrialsOfOdin.Stats
{
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
}