using UnityEngine;

namespace TrialsOfOdin.State
{
    public class IdleState : PlayerState
    {
        /*
            Sub state class for grounded 
         */
        public IdleState(StateUtilities utilities, ECharacterState estate) : base(utilities, estate) { }

        public override void EnterState() { AnimationRequestor.ResetLocomotion(); }

        public override ECharacterState GetNextState()
        {
            return CheckForStateChange();
        }

        public override void UpdateState()
        {
            MovementUtility.MovePlayer();
        }

        public override void ExitState() { }
    }
}