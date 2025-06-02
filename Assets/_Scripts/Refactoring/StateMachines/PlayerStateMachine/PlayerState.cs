using UnityEngine;
using TrialsOfOdin.Combat;

namespace TrialsOfOdin.State
{
    public abstract class PlayerState : BaseState<ECharacterState>
    {
        /*
            This class provides a logic extension for this use case for the abstract base class. 
            States in the state machine can inherit this class and use this more specific functionality
         */

        protected readonly StateUtilities Utilities;
        protected CharacterContext Context => Utilities.Context;
        protected CombatSystemPro CombatSystem => Utilities.CombatSystem;
        protected AnimationRequestor AnimationRequestor => Utilities.AnimationRequestor;
        protected PlayerMovementUtility MovementUtility => Utilities.MovementUtility;
        protected SoundRequestor AudioRequestor => Utilities.SoundRequestor;

        public PlayerState(StateUtilities utilites, ECharacterState stateKey) : base(stateKey)
        {
            this.Utilities = utilites;
        }

        public override void UpdateSubState()
        {
            Utilities.UpdateCurrentState(this);
        }

        protected ECharacterState CheckForStateChange()
        {
            if (Context.IsDamaged) return ECharacterState.Stunned;
            if (CanTransitionToDodge()) return ECharacterState.Dodge;
            if (CombatSystem.IsBlocking) return ECharacterState.Block;
            if (CombatSystem.IsAiming) return ECharacterState.Aim;

            //if (CanTransitionToHeavyAttack()) return PlayerStateMachine.EPlayerState.HeavyAttack;
            //if (CanTransitionToLightAttack()) return PlayerStateMachine.EPlayerState.LightAttack;
            if (Context.IsAttacking) return ECharacterState.Attack; 

            //Movement States. Walk has to be last here due to setup
            if (GameInput.Instance.GetMovementInput().ReadValue<Vector2>().magnitude == 0 && MovementUtility.Speed == 0) return ECharacterState.Idle;
            if (GameInput.Instance.IsSprinting()) return ECharacterState.Run;
            return ECharacterState.Walk;
        }

        protected bool CanTransitionToDodge() { return (Context.IsDodging && Utilities.HasSufficientStamina(10f)); }
    }
}
