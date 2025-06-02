using UnityEngine;

namespace TrialsOfOdin.State
{
    public class AttackState : PlayerState
    {
        public AttackState(StateUtilities utilites, ECharacterState stateKey) : base(utilites, stateKey) { }

        public override void EnterState()
        {
            Context.IsAttacking = false;
        }

        public override void ExitState()
        {

        }

        public override ECharacterState GetNextState()
        {
            if (Context.IsDamaged) return ECharacterState.Stunned;
            if (CanTransitionToDodge()) return ECharacterState.Dodge;

            if (Context.AnimFinished && !Context.IsCharging) return CheckForStateChange();
            //was using isHolding

            return StateKey;
        }


        public override void UpdateState()
        {

        }
    }
}
