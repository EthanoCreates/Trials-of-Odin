using Sirenix.OdinInspector;
using UnityEngine;

namespace TrialsOfOdin.Stats
{
    public class PlayerStats : CharacterStats
    {
        [PropertyOrder(-1), SerializeField, InlineEditor] PlayerDataSO basePlayerStats;

        private void Awake()
        {
            WalkSpeed = basePlayerStats.walkSpeed;
            RunSpeed = basePlayerStats.runSpeed;
            SprintSpeed = basePlayerStats.sprintSpeed;
            SprintTime = basePlayerStats.sprintTime;
            JumpHeight = basePlayerStats.jumpHeight;
            GroundLayers = basePlayerStats.groundLayers;
        }

        public void BuffStats()
        {
            WalkSpeed *= 2;
            RunSpeed *= 2;
            SprintSpeed *= 2;
            JumpHeight *= 2;
            SprintTime *= 2;
        }

        public void RevertStats()
        {
            WalkSpeed /= 2;
            RunSpeed /= 2;
            SprintSpeed /= 2;
            JumpHeight /= 2;
            SprintTime /= 2;
        }
    }
}
