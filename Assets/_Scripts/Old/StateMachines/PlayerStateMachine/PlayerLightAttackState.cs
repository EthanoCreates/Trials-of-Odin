using System.Collections;
using TrialsOfOdin.Combat;
using UnityEngine;
namespace TrialsOfOdin.State
{ 
public class PlayerLightAttackState : PlayerState
{
    private bool isHolding;

    private float lightChargedTimer;

    private bool isLightAttacking;
    private bool hasChargedAttacked;

    private bool hasSprintAttacked;
    private bool weaponHasSprintAttacks;

    private bool canHeavyTransition;
    private bool isHeavyTransition;

    private int comboValue = 0;
    private int lightComboLimit;

    public PlayerLightAttackState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override ECharacterState GetNextState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    //public override void EnterState()
    //{
    //    InitializeSubState();
    //    LightAttackSetUp();
    //}

    //private void InitializeSubState()
    //{
    //    SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.CombatOrient));
    //}

    ////Set up and Transition attack checks
    //private void LightAttackSetUp()
    //{
    //    ResetValues();

    //    SubscribeToEvents();

    //    if (!CheckForTransitionAttack()) StarterAttack();
    //}
    ///// <summary>
    ///// Checking if light attack can be performed
    ///// </summary>
    ///// <param name="sender">on light attack button clicked</param>
    ///// <param name="e"></param>
    //private void PlayerContext_CanPerformLightAttack(object sender, System.EventArgs e)
    //{
    //    if (!isLightAttacking)
    //    {
    //        CheckForChargingAttack();
    //        HandleLightAttack();
    //    }
    //}

    ///// <summary>
    ///// Setting is holding to true and checking for if player 
    ///// holds till charged through coroutine.
    ///// </summary>
    //private void CheckForChargingAttack()
    //{
    //    CoroutineHelper.Instance.StopCoroutine(ChargingAttack());
    //    isHolding = true;
    //    CoroutineHelper.Instance.StartCoroutine(ChargingAttack());
    //}

    ///// <summary>
    ///// Handling how light combo is executed
    ///// </summary>
    //private void HandleLightAttack()
    //{
    //    comboValue++;

    //    if (comboValue == lightComboLimit) { ResetCombo(); }

    //    if (canHeavyTransition) canHeavyTransition = false;

    //    if (comboValue <= lightComboLimit) isLightAttacking = true;
    //}

    ////Transition
    //private void PlayerContext_CanPerformHeavyAttack(object sender, System.EventArgs e)
    //{
    //    if (!canHeavyTransition)
    //    {
    //        isLightAttacking = false;
    //        canHeavyTransition = true;
    //    }
    //}

    //private void CombatHelper_OnLightAttackReleased(object sender, System.EventArgs e)
    //{
    //    isHolding = false;
    //}

    //public override PlayerStateMachine.EPlayerState GetNextState()
    //{
    //    if (Context.IsDamaged) return PlayerStateMachine.EPlayerState.Stunned;
    //    if (CombatManager.IsBlocking) return PlayerStateMachine.EPlayerState.Block;
    //    if (CanTransitionToDodge()) return PlayerStateMachine.EPlayerState.Dodge;

    //    if (CombatManager.AimAttack()) return PlayerStateMachine.EPlayerState.Aim;
    //    //if (isHeavyTransition) return PlayerStateMachine.EPlayerState.HeavyAttack;

    //    if (Context.AnimFinished && !isHolding) return CheckForStateChange();

    //    return StateKey;
    //}

    //public override void UpdateState()
    //{
    //    if (CombatManager.IsComboAvailable) return;

    //    if (isLightAttacking)
    //    {
    //        LightAttack();
    //    }
    //    else if (canHeavyTransition)
    //    {
    //        HandleHeavyAttack();
    //    }
    //}

    ///// <summary>
    ///// When timer is over lightChargedTimer we execute charged attack.
    ///// And allow player to chain charged attacks
    ///// </summary>
    //private IEnumerator ChargingAttack()
    //{
    //    float timer = -lightChargedTimer / 2f;

    //    AnimationRequestor.AnimateGroundedAttackStateEnter();

    //    while (isHolding)
    //    {
    //        if (timer < lightChargedTimer)
    //        {
    //            if (!CombatManager.IsComboAvailable) { 
    //                timer += Time.deltaTime; 
    //                if (canHeavyTransition) TryFinisherAttackOnHold();
    //            }
    //        }
    //        else
    //        {
    //            timer = -lightChargedTimer;
    //            ChargedLightAttack();
    //        }

    //        yield return null;
    //    }
    //    AnimationRequestor.AnimateGroundedAttackStateExit();
    //}

    //private void TryFinisherAttackOnHold()
    //{
    //    if (CombatManager.TryFinisherAttack())
    //    {
    //        CombatManager.ExecuteAttack(CombatManager.GetFinisherAttack());
    //        isHolding = false;
    //        isHeavyTransition = false;
    //        canHeavyTransition = false;
    //        hasChargedAttacked = true;
    //    }
    //}

    //private void HandleHeavyAttack()
    //{
    //    if (isHolding) return;

    //    CombatManager.IsComboTransition = true;

    //    isHeavyTransition = true;
    //}

    //private void LightAttack()
    //{
    //    if((hasChargedAttacked || hasSprintAttacked) && CombatManager.HasRecoveryAttack())
    //    {
    //        CombatManager.ExecuteAttack(CombatManager.GetRecoveryAttack());
    //        hasChargedAttacked = false;
    //        isLightAttacking = false;

    //        //Setting recovery combo transitions 
    //        comboValue = CombatManager.GetLightComboValue() - 1;
    //        CombatManager.ComboTransitionValue = CombatManager.GetHeavyComboValue();
    //    }
    //    else ExecuteLightAttack();
    //}

    //private void ExecuteLightAttack()
    //{
    //    if (CombatManager.exitComboUnlessRecover) return;

    //    if (!CombatManager.ExecuteAttack(CombatManager.GetLightCombo(comboValue))) { Context.AnimationFinished(); }

    //    CombatManager.ComboTransitionValue = CombatManager.GetHeavyComboValue();

    //    isLightAttacking = false;
    //}

    //private void ChargedLightAttack()
    //{
    //    if (!CombatManager.ExecuteAttack(CombatManager.GetLightChargedAttack())) return;

    //    if (CombatManager.canCombo())
    //    {
    //        comboValue = CombatManager.GetLightComboValue() - 1;
    //        CombatManager.ComboTransitionValue = CombatManager.GetHeavyComboValue();
    //    }
    //    else CombatManager.exitComboUnlessRecover = true;

    //    hasChargedAttacked = true;
    //}

    //private void ResetCombo()
    //{
    //    comboValue = 0;
    //}

    //private void FirstLightAttack()
    //{
    //    CheckForChargingAttack();
    //    //this is for the case where the player has queued in heavy attack
    //    //It has transfered over but before they have let go, so released won't
    //    //be triggered in here so we have this to double check
    //    if (!GameInput.Instance.StillHoldingLight()) isHolding = false;
    //    CoroutineHelper.Instance.StartCoroutine(CancellingChargedAttack());
    //}

    ///// <summary>
    ///// Checking if there is a combo transition if so, we handle it and turn it off.
    ///// </summary>
    //private bool CheckForTransitionAttack()
    //{
    //    if (CombatManager.IsComboTransition)
    //    {
    //        HandleComboTransition();
    //        return true;
    //    }
    //    if(ShouldHandleSprintAttack())
    //    {
    //        HandleSprintAttack();
    //        return true;
    //    }
    //    if (CombatManager.ChargedAttackAfterParry)
    //    {
    //        CombatManager.ChargedAttackAfterParry = false;
    //        ChargedLightAttack();
    //        return true;
    //    }
    //    return false;
    //}

    //private void HandleComboTransition()
    //{
    //    if (CombatManager.IsRecoveringFromAerialAttack)
    //    {
    //        CombatManager.IsRecoveringFromAerialAttack = false;

    //        if (CombatManager.HasRecoveryAttack()) AerialAttackRecovery();

    //        if (CombatManager.canCombo())
    //        {
    //            comboValue = CombatManager.GetLightComboValue() - 1;
    //            CombatManager.ComboTransitionValue = CombatManager.GetHeavyComboValue();
    //        }
    //        else CombatManager.exitComboUnlessRecover = true;

    //    }
    //    else
    //    {
    //        comboValue = CombatManager.ComboTransitionValue;
    //        FirstLightAttack();
    //    }
    //    CombatManager.IsComboingFromAerialAttack = false;
    //    CombatManager.IsComboTransition = false;
    //}
    //private IEnumerator CancellingChargedAttack()
    //{
    //    while (isHolding)
    //    {
    //        yield return null;
    //    }

    //    if (!hasChargedAttacked) { ExecuteLightAttack(); }
    //}

    //private void StarterAttack()
    //{
    //    ResetCombo();
    //    FirstLightAttack();
    //}

    //private bool ShouldHandleSprintAttack()
    //{
    //    return MovementUtility.Speed >= 8 && weaponHasSprintAttacks;
    //}

    //private void AerialAttackRecovery()
    //{
    //    CombatManager.ExecuteAttack(CombatManager.GetRecoveryAttack());

    //    comboValue = CombatManager.GetLightComboValue() - 1;
    //    CombatManager.ComboTransitionValue = CombatManager.GetHeavyComboValue();

    //    CombatManager.exitComboUnlessRecover = false;
    //}

    //private void HandleSprintAttack()
    //{
    //    //sprint attack is a charged attack that can be comboed from
    //    AnimationRequestor.AnimateGroundedAttackStateExit();
    //    CombatManager.ExecuteAttack(CombatManager.GetLightSprintAttack());

    //    //Handling Sprint combo transitioning
    //    if (CombatManager.canCombo())
    //    {
    //        comboValue = CombatManager.GetLightComboValue() - 1;
    //        CombatManager.ComboTransitionValue = CombatManager.GetHeavyComboValue();
    //    }
    //    else CombatManager.exitComboUnlessRecover = true;

    //    hasSprintAttacked = true;
    //}

    //private void ResetValues()
    //{
    //    isLightAttacking = false;
    //    hasChargedAttacked = false;
    //    isHeavyTransition = false;
    //    canHeavyTransition = false;

    //    Weapon weapon = PlayerStateMachine.LocalInstance.WeaponHolster.ActiveWeapon;

    //    lightComboLimit = weapon.LightCombo.Count;
    //    lightChargedTimer = weapon.LightChargedAttack.chargingTimer;
    //    weaponHasSprintAttacks = weapon.LightSprintAttack != null;
    //}

    //public override void ExitState()
    //{
    //    if (!CombatManager.IsComboTransition) CombatManager.exitComboUnlessRecover = false;
    //    ResetCombo();
    //    UnSubscribeFromEvents();
    //}

    //private void SubscribeToEvents()
    //{
    //    //Gets thrown when light attack starts not on first attack though
    //    CombatManager.OnLightAttackStarted += PlayerContext_CanPerformLightAttack;
    //    CombatManager.OnLightAttackFinished += CombatHelper_OnLightAttackReleased;
    //    CombatManager.OnHeavyAttackStarted += PlayerContext_CanPerformHeavyAttack;
    //}

    //private void UnSubscribeFromEvents()
    //{
    //    CombatManager.OnLightAttackStarted -= PlayerContext_CanPerformLightAttack;
    //    CombatManager.OnLightAttackFinished -= CombatHelper_OnLightAttackReleased;
    //    CombatManager.OnHeavyAttackStarted -= PlayerContext_CanPerformHeavyAttack;
    //}
}
}