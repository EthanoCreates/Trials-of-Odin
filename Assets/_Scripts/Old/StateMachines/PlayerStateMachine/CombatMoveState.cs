using UnityEngine;

namespace TrialsOfOdin.State
{
    public class CombatMoveState : PlayerState
    {
        public CombatMoveState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }

        private float walkSpeed;

        public override void EnterState()
        {
            CombatMovementSetup();
        }

        public override void ExitState() { }

        public override ECharacterState GetNextState()
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
}