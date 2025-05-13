public class PlayerVaultState : PlayerState
{
    public PlayerVaultState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate) { }
    public override void EnterState()
    {
        MovementUtility.Speed = 0;
        AnimationRequestor.AnimateVault();
        MovementUtility.VerticalVelocity = 0f;
    }

    public override void ExitState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return StateKey;
    }

    public override void UpdateState()
    {
        MovementUtility.MovePlayer();
    }
}
