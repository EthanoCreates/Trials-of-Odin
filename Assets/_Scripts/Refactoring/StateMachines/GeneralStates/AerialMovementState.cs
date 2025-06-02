using System;
using UnityEngine;
namespace TrialsOfOdin.State 
{ 
    public class AerialMovementState : PlayerState
    {
        private float walkSpeed;
        private float runSpeed;
        private float terminalVelocity;
        private LayerMask groundLayers;
        private Transform transform;
        float targetSpeed;

        public AerialMovementState(StateUtilities utilities , ECharacterState stateKey) : base(utilities, stateKey) { }

        public override void EnterState()
        {
            AerialMovementSetUp();
            //CombatManager.OnHeavyAttackStarted += PlayerContext_OnHeavyAttack;
            //CombatManager.OnLightAttackStarted += CombatHelper_OnLightAttackStarted;
        }

        public override ECharacterState GetNextState()
        {
            return StateKey;
        }

        private void PlayerContext_OnHeavyAttack(object sender, EventArgs e)
        {
            TryAerialAttack(true);
        }

        private void CombatHelper_OnLightAttackStarted(object sender, EventArgs e)
        {
            TryAerialAttack(false);
        }

        private void TryAerialAttack(bool isHeavy)
        {
            //if (CombatManager.IsAerialAttack)
            //{
            //    CombatManager.IsHeavyAttackAfterAerial = isHeavy;

            //    CombatManager.AerialTransitionAttack(isHeavy);
            //}
            //else if (ValidAerialAttackHeight() && MovementUtility.VerticalVelocity < terminalVelocity)
            //{
            //    //Handles aerial attack
            //    CombatManager.IsHeavyAerialAttack = isHeavy;
            //    //CurrentSuperState.SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.AerialAttack));
            //}
        }


        public override void UpdateState()
        {
            if (GameInput.Instance.IsSprinting()) targetSpeed = runSpeed;
            else targetSpeed = walkSpeed;

            AnimationRequestor.AnimateLocomotion(MovementUtility.HandleMovement(targetSpeed), MovementUtility.Speed);
        }

        public override void ExitState()
        { 
            //CombatManager.OnHeavyAttackStarted -= PlayerContext_OnHeavyAttack;
            //CombatManager.OnLightAttackStarted -= CombatHelper_OnLightAttackStarted;
        }

        private void AerialMovementSetUp()
        {
            walkSpeed = Context.WalkSpeed;
            runSpeed = Context.RunSpeed;

            transform = MovementUtility.Transform;

            terminalVelocity = Context.TerminalVelocity;
            groundLayers = Context.GroundLayers;
        }

        private bool ValidAerialAttackHeight()
        {
            if (Physics.Raycast(transform.position, -transform.up, 10f, groundLayers))
            {
                return true;
            }
            return false;
        }
    }
}