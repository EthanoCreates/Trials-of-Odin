using UnityEngine;

public class PlayerIdleState : PlayerState
{
    /*
        Sub state class for grounded 
     */
    public PlayerIdleState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate) { }

    public override void EnterState() { AnimationRequestor.ResetLocomotion(); }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return CheckForStateChange();
    }

    public override void UpdateState()
    {
        MovementUtility.MovePlayer();
    }

    public override void ExitState() { }
}
