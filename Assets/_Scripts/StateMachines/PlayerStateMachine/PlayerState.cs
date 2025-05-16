using UnityEngine;
using TrialsOfOdin.Combat;

public abstract class PlayerState : BaseState<PlayerStateMachine.EPlayerState>
{
    /*
        This class provides a logic extension for this use case for the abstract base class. 
        States in the state machine can inherit this class and use this more specific functionality
     */

    protected readonly PlayerUtilities Utilities;
    protected PlayerContext Context => Utilities.Context;
    protected PlayerCombatManager CombatManager => Utilities.CombatManager;
    protected PlayerAnimationRequestor AnimationRequestor => Utilities.AnimationRequestor;
    protected PlayerMovementUtility MovementUtility => Utilities.MovementUtility;
    protected PlayerSoundRequestor AudioRequestor => Utilities.AudioRequestor;

    public PlayerState(PlayerUtilities utilites, PlayerStateMachine.EPlayerState stateKey) : base (stateKey)
    {
        this.Utilities = utilites;
    }

    public override void UpdateSubState()
    {
        PlayerStateMachine.LocalInstance.CurrentState(this);
    }

   protected PlayerStateMachine.EPlayerState CheckForStateChange()
   {
        if (Context.IsDamaged) return PlayerStateMachine.EPlayerState.Stunned;
        if (CanTransitionToDodge()) return PlayerStateMachine.EPlayerState.Dodge;
        if (CombatManager.IsBlocking) return PlayerStateMachine.EPlayerState.Block;
        if (CombatManager.AimAttack()) return PlayerStateMachine.EPlayerState.Aim;
        if (CanTransitionToHeavyAttack()) return PlayerStateMachine.EPlayerState.HeavyAttack;
        if (CanTransitionToLightAttack()) return PlayerStateMachine.EPlayerState.LightAttack;
        //Movement States. Walk has to be last here due to setup
        if (GameInput.Instance.GetMovementInput().ReadValue<Vector2>().magnitude == 0 && MovementUtility.Speed == 0) return PlayerStateMachine.EPlayerState.Idle;
        if (GameInput.Instance.IsSprinting()) return PlayerStateMachine.EPlayerState.Run;
        return PlayerStateMachine.EPlayerState.Walk;
   }

    protected bool CanTransitionToDodge() { return (Context.IsDodging && Utilities.HasSufficientStamina(10f)); }
    protected bool CanTransitionToHeavyAttack() { return CombatManager.IsHeavyAttacking && Utilities.CouldPerformActionWithStamina(CombatManager.GetHeavyCombo(0).staminaCost); }
    protected bool CanTransitionToLightAttack() { return CombatManager.IsLightAttacking && Utilities.CouldPerformActionWithStamina(CombatManager.GetLightCombo(0).staminaCost); }
}
