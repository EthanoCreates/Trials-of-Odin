using UnityEngine;

public class PlayerCombatMoveState : PlayerState
{
    public PlayerCombatMoveState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState stateKey) : base(utilities, stateKey) { }

    private float walkSpeed;

    public override void EnterState()
    {
        CombatMovementSetup();
    }

    public override void ExitState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return StateKey;
    }

    public override void UpdateState()
    {
        AnimationRequestor.AnimateLocomotion(MovementUtility.HandleMovement(walkSpeed), MovementUtility.Speed);

        MovementUtility.MovePlayer();
    }

    private void CombatMovementSetup()
    {
        walkSpeed = Context.WalkSpeed;
    }
}
