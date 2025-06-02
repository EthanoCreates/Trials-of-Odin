using UnityEngine;

namespace TrialsOfOdin.State
{
    public class BlockState : PlayerState
    {
        float parryTimer;
        bool hasParried;
        bool heavyCounter;
        bool lightCounter;

        public BlockState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }

        public override void EnterState()
        {
            InitializeSubState();
            AnimationRequestor.AnimateBlock(true);
            parryTimer = .2f;
            //Utilities.OnDamageBlocked += Health_OnDamageBlocked;
            //CombatManager.OnLightAttackStarted += CombatHelper_OnLightAttackStarted;
            //CombatManager.OnHeavyAttackStarted += CombatHelper_OnHeavyAttackStarted;
            hasParried = false;
            heavyCounter = false;
            lightCounter = false;
        }

        private void CombatHelper_OnHeavyAttackStarted(object sender, System.EventArgs e)
        {
            if (hasParried) heavyCounter = true;
        }

        private void CombatHelper_OnLightAttackStarted(object sender, System.EventArgs e)
        {
            if (hasParried) lightCounter = true;
        }

        private void Health_OnDamageBlocked()
        {
            if (parryTimer > 0)
            {
                if (PlayerStateMachine.LocalInstance.WeaponHolster.HasShieldEquipped)
                {
                    hasParried = true;
                    AnimationRequestor.AnimateParry();
                    //CombatManager.IsComboAvailable = true;
                    //e.enemyHealth.TakeDamage(20, e.collisionPosition, -1);
                }
            }
            else AnimationRequestor.AnimateBlockImpact();
        }

        public override void ExitState()
        {
            //Utilities.OnDamageBlocked -= Health_OnDamageBlocked;
            //CombatManager.OnLightAttackStarted -= CombatHelper_OnLightAttackStarted;
            //CombatManager.OnHeavyAttackStarted -= CombatHelper_OnHeavyAttackStarted;

            //uses the player context so this will work
            AnimationRequestor.AnimateBlock(false);
            AnimationRequestor.AnimationExit();
        }

        public override ECharacterState GetNextState()
        {
            if (CanTransitionToDodge()) return ECharacterState.Dodge;
            //if (Context.IsDamaged) { CombatManager.IsBlocking = false; return PlayerStateMachine.EPlayerState.Stunned; }

            if (hasParried)
            {
                //if (!CombatManager.IsComboAvailable)
                //{
                //    if (heavyCounter)
                //    {
                //        CombatManager.ChargedAttackAfterParry = true;
                //        //return PlayerStateMachine.EPlayerState.HeavyAttack;
                //    }

                //    if (lightCounter)
                //    {
                //        CombatManager.ChargedAttackAfterParry = true;
                //        //return PlayerStateMachine.EPlayerState.LightAttack;
                //    }

                //    return CheckForStateChange();
                //}
            }
            //else if (!CombatManager.IsBlocking) return CheckForStateChange();
            return StateKey;
        }

        private void InitializeSubState()
        {
            SetSubState(Utilities.GetStateInstance(ECharacterState.CombatMovement));
        }

        public override void UpdateState()
        {

            if (parryTimer > 0) parryTimer -= Time.deltaTime;
            if (MovementUtility.TurnToFaceAimDirection())
            {
                AnimationRequestor.AnimateTurnStart();
                AnimationRequestor.AnimateTurn(MovementUtility.TurnAmount);
            }
            else AnimationRequestor.AnimationExit();
        }
    }
}