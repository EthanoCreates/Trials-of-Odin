using System.Collections;
using UnityEngine;

namespace TrialsOfOdin.State
{
    public class AscendState : PlayerState
    {
        //These sub states can't switch so store which one is active to allow easier state switching with having to check player state machine every time
        private bool jump;
        private bool climb;
        private bool vault;

        private bool hasJumped;

        public AscendState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            AscendSetUp();
            InitializeSubState();
        }

        private void InitializeSubState()
        {
            //if (CheckForVaultable(playerCamera)) {SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Vault)); vault = true; }
            //else if (CheckForClimbable(playerCamera)) { SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Climb)); climb = true; }
            //else if (!climb && !vault)  {
            if (!Utilities.HasSufficientStamina(5f)) return;

            SetSubState(Utilities.GetStateInstance(ECharacterState.Jump));
            jump = true;
            PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnApplyJumpVelocity += Player_OnApplyJumpVelocity;
            //}
        }

        private void Player_OnApplyJumpVelocity()
        {
            hasJumped = true;
        }

        public override ECharacterState GetNextState()
        {
            if (MovementUtility.VerticalVelocity < 0 && jump && hasJumped) return ECharacterState.Falling;
            if (Context.AnimFinished && (climb || vault)) return ECharacterState.Grounded;
            return StateKey;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            Context.IsJumping = false;
            CombatSystem.IsAerialing = false;
        }

        private void AscendSetUp()
        {
            jump = false;
            vault = false;
            climb = false;
            CombatSystem.IsAerialing = true;
            hasJumped = false;
        }

        private bool CheckForVaultable(Transform playerCamera)
        {
            //if (Physics.Raycast(playerCamera.position, playerCamera.forward, 10f, Context.VaultLayer))
            //{
            //    return true;
            //}
            return false;
        }

        private bool CheckForClimbable(Transform playerCamera)
        {
            //if (Physics.Raycast(playerCamera.position, playerCamera.forward, 10f, Context.ClimbLayer))
            //{
            //    return true;
            //}
            return false;
        }
    }
}