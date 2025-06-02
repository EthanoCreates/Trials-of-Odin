using UnityEngine;

namespace TrialsOfOdin.State
{
    public class WalkState : PlayerState
    {
        private float walkSpeed;
        public WalkState(StateUtilities utilities, ECharacterState estate) : base(utilities, estate) { }

        public override void EnterState()
        {
            WalkSetup();
        }

        public override ECharacterState GetNextState()
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
}
