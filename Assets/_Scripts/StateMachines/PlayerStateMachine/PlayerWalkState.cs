using UnityEngine;

public class PlayerWalkState : PlayerState
{
    private float walkSpeed;
    public PlayerWalkState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate) { }

    public override void EnterState()
    {
        WalkSetup();
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return CheckForStateChange();
    }

    public override void UpdateState()
    {
        Walk();
        MovementUtility.MovePlayer();
    }

    public override void ExitState() { }

    private void Walk()
    {
        AnimationRequestor.AnimateLocomotion(MovementUtility.HandleMovement(walkSpeed), MovementUtility.Speed);
    }

    private void WalkSetup()
    {
        walkSpeed = Context.WalkSpeed;
    }
}
