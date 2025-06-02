using UnityEngine;

namespace TrialsOfOdin.State
{
    public class FallingState : PlayerState
    {
        private LayerMask groundLayers;
        private Transform transform;

        public FallingState(StateUtilities utilities, ECharacterState estate) : base(utilities, estate)
        {
            IsRootState = true;
        }

        private float fallGravityMultiplier;
        private float terminalvelocity;

        public override void EnterState()
        {
            FallingSetUp();
            InitializeSubState();
        }

        private void InitializeSubState()
        {
            SetSubState(Utilities.GetStateInstance(ECharacterState.AerialMovement));
        }

        public override ECharacterState GetNextState()
        {
            if (Context.IsGrounded) return ECharacterState.Grounded;
            return StateKey;
        }

        public override void UpdateState()
        {
            //falling logic
            if (MovementUtility.VerticalVelocity < terminalvelocity)
            {
                // add to vertical velocity
                MovementUtility.ApplyGravity(fallGravityMultiplier);
            }
            MovementUtility.MovePlayer();
        }

        public override void ExitState()
        {
            AnimationRequestor.AnimateExitFalling();
            CombatSystem.IsAerialing = false;
            if (MovementUtility.VerticalVelocity < -15f) Context.Landing = true;
        }

        private bool ValidAerialAttackHeight()
        {
            if (Physics.Raycast(transform.position, -transform.up, 10f, groundLayers))
            {
                return true;
            }
            return false;
        }

        private void FallingSetUp()
        {
            AnimationRequestor.AnimateFalling();
            CombatSystem.IsAerialing = true;
            fallGravityMultiplier = Context.FallGravityMultiplier;
            terminalvelocity = Context.TerminalVelocity;

            transform = MovementUtility.Transform;

            groundLayers = Context.GroundLayers;
        }
    }
}