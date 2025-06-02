using UnityEngine;

namespace TrialsOfOdin.State
{
    public class GroundedState : PlayerState
    {
        //Ground check variables
        private float timeTillFall;
        private float timerTillFall;
        private float jumpTimeout;
        private bool isFalling;

        public GroundedState(StateUtilities utilities, ECharacterState estate) : base(utilities, estate)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            GroundedSetup();
            InitializeSubState();
        }

        private void InitializeSubState()
        {
            if (Context.Landing) SetSubState(Utilities.GetStateInstance(ECharacterState.Land));
            else if (CombatSystem.IsBlocking) SetSubState(Utilities.GetStateInstance(ECharacterState.Block));
            else if (CanTransitionToDodge()) SetSubState(Utilities.GetStateInstance(ECharacterState.Dodge));
            else if (GameInput.Instance.IsSprinting() && MovementUtility.Speed > 0) SetSubState(Utilities.GetStateInstance(ECharacterState.Run));
            else if (MovementUtility.Speed > 0) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(ECharacterState.Walk));
            else SetSubState(Utilities.GetStateInstance(ECharacterState.Idle));
        }

        public override ECharacterState GetNextState()
        {
            if (isFalling) return ECharacterState.Falling;
            if (Context.IsJumping && jumpTimeout < 0 && Context.AnimFinished) return ECharacterState.Ascend;

            return StateKey;
        }

        public override void UpdateState()
        {
            HandleStamina();

            if (jumpTimeout >= 0.0f)
            {
                jumpTimeout -= Time.deltaTime;
            }

            if (Context.IsGrounded)
            {
                timerTillFall = timeTillFall;
                return;
            }

            if (timerTillFall >= 0.0f)
            {
                timerTillFall -= Time.deltaTime;
                MovementUtility.ApplyGravity(1f);
            }
            else
            {
                isFalling = true;
            }
        }

        private void HandleStamina()
        {
            if (CurrentSubState.StateKey != ECharacterState.Run)
            {
                Utilities.RecoverStamina();
            }
        }

        public override void ExitState() { }

        private void GroundedSetup()
        {
            AnimationRequestor.AnimateGrounded();

            AnimationRequestor.AnimateLand(MovementUtility.VerticalVelocity);
            AudioRequestor.PlayLandSound();

            jumpTimeout = Context.JumpTimeout;
            timeTillFall = Context.FallTimeout;
            timerTillFall = timeTillFall;

            isFalling = false;
            MovementUtility.VerticalVelocity = -2f; //Ensures player does not clip through ground by lowering downward force
        }
    }
}


