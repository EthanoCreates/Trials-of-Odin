namespace TrialsOfOdin.State
{
    public class AimState : PlayerState
    {
        public AimState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }

        public override void EnterState()
        {
            AimSetUp();
            InitializeSubState();
        }

        private void InitializeSubState()
        {
            //SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.AimAttack));
        }

        public override void ExitState()
        {
            AnimationRequestor.AnimateAimExit();
            AnimationRequestor.AnimationExit();
        }

        public override ECharacterState GetNextState()
        {
            if (Context.IsDamaged) return ECharacterState.Stunned;

            //if (!CombatManager.IsComboAvailable)
            //{
            //    if (CombatManager.IsBlocking) return PlayerStateMachine.EPlayerState.Block;
            //    if (CanTransitionToDodge()) return PlayerStateMachine.EPlayerState.Dodge;
            //}

            //if(Context.AnimFinished)
            //{
            //    if (CombatManager.SwitchFromAimWeapon)
            //    {
            //        PlayerStateMachine.LocalInstance.WeaponHolster.ReleaseWeaponFromHolster();
            //        CombatManager.SwitchFromAimWeapon = false;
            //        return CheckForStateChange();
            //    }
            //    if(!CombatManager.IsAiming)
            //    {
            //        return CheckForStateChange();
            //    }
            //}
            return StateKey;
        }

        public override void UpdateState()
        {
            if (MovementUtility.TurnToFaceAimDirection())
            {
                AnimationRequestor.AnimateTurnStart();
                AnimationRequestor.AnimateTurn(MovementUtility.TurnAmount);
            }
            else AnimationRequestor.AnimationExit();
        }

        private void AimSetUp()
        {
            AnimationRequestor.AnimateAim();
        }
    }
}