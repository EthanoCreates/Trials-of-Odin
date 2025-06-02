namespace TrialsOfOdin.State
{
    public class PlayerClimbState : PlayerState
    {
        public PlayerClimbState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }
        public override void EnterState()
        {
            MovementUtility.Speed = 0;
            AnimationRequestor.AnimateClimb();
            MovementUtility.VerticalVelocity = 0f;
        }

        public override void ExitState() { }

        public override ECharacterState GetNextState()
        {
            return StateKey;
        }

        public override void UpdateState()
        {
            //can control climb height with vertical velocity
            MovementUtility.MovePlayer();
        }
    }
}