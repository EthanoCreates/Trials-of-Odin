using System;
using UnityEngine;

namespace TrialsOfOdin.State
{
    public class AerialAttackState : PlayerState
    {
        public AerialAttackState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }

        public override void EnterState()
        {
            //preventing double queueing of aerial attacks
            //if (CombatManager.IsAerialAttack) return;
            //AerialAttackSetUp();

            //if (CombatManager.IsHeavyAerialAttack) CombatManager.ExecuteAttack(CombatManager.GetHeavyAerialAttack());
            //else CombatManager.ExecuteAttack(CombatManager.GetLightAerialAttack());

            //CombatManager.OnLightAttackStarted += CombatHelper_OnLightAttackStarted;
            //CombatManager.OnHeavyAttackStarted += PlayerContext_OnHeavyAttack;
        }

        private void CombatHelper_OnLightAttackStarted(object sender, EventArgs e)
        {
            //CombatManager.IsRecoveringFromAerialAttack = true;
        }

        private void PlayerContext_OnHeavyAttack(object sender, EventArgs e)
        {
            //CombatManager.IsRecoveringFromAerialAttack = true;
        }

        public override ECharacterState GetNextState()
        {
            if (Context.AnimFinished) return ECharacterState.AerialMovement;
            return StateKey;
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            Context.Landing = true;
            //CombatManager.OnHeavyAttackStarted -= PlayerContext_OnHeavyAttack;
            //CombatManager.OnLightAttackStarted -= CombatHelper_OnLightAttackStarted;
        }

        private void AerialAttackSetUp()
        {
            //CombatManager.IsAerialAttack = true;
            //CombatManager.IsRecoveringFromAerialAttack = false;
            //CombatManager.IsComboingFromAerialAttack = false;
        }
    }
}
