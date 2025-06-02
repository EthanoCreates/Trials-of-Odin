using System.Collections;
using UnityEngine;

namespace TrialsOfOdin.State
{
    public class JumpState : PlayerState
    {
        private LayerMask groundLayers;
        private Transform transform;
        private float gravity;

        public JumpState(StateUtilities utilities, ECharacterState estate) : base(utilities, estate) { }

        public override void EnterState()
        {
            JumpSetUp();
            InitializeSubState();
        }

        public override ECharacterState GetNextState()
        {
            return StateKey;
        }

        public override void UpdateState()
        {
            MovementUtility.ApplyGravity(1f);

            MovementUtility.MovePlayer();
        }
        private void InitializeSubState()
        {
            SetSubState(Utilities.GetStateInstance(ECharacterState.AerialMovement));
        }

        public override void ExitState()
        {
            PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnApplyJumpVelocity -= ApplyJumpVelocity;
        }

        private void JumpSetUp()
        {
            PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnApplyJumpVelocity += ApplyJumpVelocity;

            //playing jump anim turning off grounded
            AnimationRequestor.AnimateJump();

            AudioRequestor.PlayJumpSound();
            MovementUtility.VerticalVelocity = 0f;

            transform = MovementUtility.Transform;
            gravity = MovementUtility.Gravity;

            groundLayers = Context.GroundLayers;
            //CombatManager.IsAerialAttack = false;
        }

        private void ApplyJumpVelocity()
        {
            MovementUtility.VerticalVelocity = Mathf.Sqrt(Context.JumpHeight * -2f * gravity);
        }
    }
}