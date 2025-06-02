namespace TrialsOfOdin.State
{
    public class PlayerVaultState : PlayerState
    {
        public PlayerVaultState(StateUtilities utilities, ECharacterState estate) : base(utilities, estate) { }
        public override void EnterState()
        {
            MovementUtility.Speed = 0;
            AnimationRequestor.AnimateVault();
            MovementUtility.VerticalVelocity = 0f;
        }

        public override void ExitState() { }

        public override ECharacterState GetNextState()
        {
            return StateKey;
        }

        public override void UpdateState()
        {
            MovementUtility.MovePlayer();
        }
    }
}
