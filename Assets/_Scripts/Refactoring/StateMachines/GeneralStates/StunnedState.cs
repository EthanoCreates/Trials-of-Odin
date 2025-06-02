using UnityEngine;

namespace TrialsOfOdin.State
{
    public class StunnedState : PlayerState
    {
        public StunnedState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }

        public override void EnterState() { }

        public override ECharacterState GetNextState()
        {
            if (Context.AnimFinished)
            {
                Context.IsDamaged = false;
                if (CombatSystem.IsBlocking) return ECharacterState.Block;
                if (CanTransitionToDodge()) return ECharacterState.Dodge;
                //if (CanTransitionToHeavyAttack()) return PlayerStateMachine.EPlayerState.HeavyAttack;
                //if (CanTransitionToLightAttack()) return PlayerStateMachine.EPlayerState.LightAttack;
                if (GameInput.Instance.GetMovementInput().ReadValue<Vector2>().magnitude > 0 && GameInput.Instance.IsSprinting()) return ECharacterState.Run;
                if (GameInput.Instance.GetMovementInput().ReadValue<Vector2>().magnitude > 0) return ECharacterState.Walk;
                return ECharacterState.Idle;
            }
            return StateKey;
        }

        public override void UpdateState() { }

        public override void ExitState() { }
    }
}