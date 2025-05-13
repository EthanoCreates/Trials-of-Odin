public class PlayerClimbState : PlayerState
{
    public PlayerClimbState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState stateKey) : base(utilities, stateKey) { }
    public override void EnterState()
    {
        MovementUtility.Speed = 0;
        AnimationRequestor.AnimateClimb();
        MovementUtility.VerticalVelocity = 0f;
    }

    public override void ExitState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return StateKey;
    }

    public override void UpdateState()
    {
        //can control climb height with vertical velocity
        MovementUtility.MovePlayer();
    }
}
