using UnityEngine;

public class PlayerStats : CharacterStats
{
    [SerializeField] PlayerDataSO basePlayerStats;

    private void Start()
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
