using System.Collections;
using UnityEngine;

public class PlayerHeavyAttackState : PlayerState
{
    private bool isHolding;

    private float heavyChargedTimer;

    private bool isHeavyAttacking;
    private bool hasChargedAttacked;

    private bool hasSprintAttacked;
    private bool weaponHasSprintAttacks;

    private bool canLightTransition;
    private bool isLightTransition;

    private int comboValue = 0;
    private int heavyComboLimit;


    public PlayerHeavyAttackState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate) { }

    public override void EnterState()
    {
        InitializeSubState();
        HeavyAttackSetUp();
    }

    private void InitializeSubState()
    {
        SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.CombatOrient));
    }

    //Set up and Transition attack checks
    private void HeavyAttackSetUp()
    {
        ResetValues();

        SubscribeToEvents();

        if (!CheckForTransitionAttack()) StarterAttack();
    }

    /// <summary>
    /// Checking if heavy attack can be performed 
    /// </summary>
    /// <param name="sender">This will be on heavy attack button clicked</param>
    /// <param name="e">Empty</param>
    private void PlayerContext_CanPerformHeavyAttack(object sender, System.EventArgs e)
    {
        if (!isHeavyAttacking)
        {
            CheckForChargingAttack();
            HandleHeavyCombo();
        }
    }

    /// <summary>
    /// Setting is holding to true and checking for if player 
    /// holds till charged through coroutine.
    /// </summary>
    private void CheckForChargingAttack()
    {
        CoroutineHelper.Instance.StopCoroutine(ChargingAttack());
        isHolding = true;
        CoroutineHelper.Instance.StartCoroutine(ChargingAttack());
    }

    /// <summary>
    /// Handling how heavy combo is executed
    /// </summary>
    private void HandleHeavyCombo()
    {
        comboValue++;

        //looping back combo to 0 later iterated to 1
        if (comboValue == heavyComboLimit) { ResetCombo(); }

        //Disabling a queued light combo
        if (canLightTransition) canLightTransition = false;

        if (comboValue <= heavyComboLimit) isHeavyAttacking = true;
    }

    /// <summary>
    /// Occurs when player clicks light attack button in heavy attack state
    /// This checks if we can perform a light attack
    /// </summary>
    /// <param name="sender">on light attack button clicked</param>
    /// <param name="e"></param>
    private void PlayerContext_CanPerformLightAttack(object sender, System.EventArgs e)
    {   
        if (!canLightTransition)
        {
            isHeavyAttacking = false;
            canLightTransition = true;
        }
    }

    private void CombatHelper_OnHeavyAttackFinished(object sender, System.EventArgs e)
    {
        isHolding = false;
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (Context.IsDamaged) return PlayerStateMachine.EPlayerState.Stunned;
        if (CombatManager.IsBlocking) return PlayerStateMachine.EPlayerState.Block;
        if (CanTransitionToDodge()) return PlayerStateMachine.EPlayerState.Dodge;

        if (CombatManager.AimAttack()) return PlayerStateMachine.EPlayerState.Aim;
        if (isLightTransition) return PlayerStateMachine.EPlayerState.LightAttack;

        if (Context.AnimFinished && !isHolding) return CheckForStateChange();
        
        return StateKey;
    }

    public override void UpdateState()
    {
        //if player is comboing, the animation plays as soon as the combo becomes unavailable

        if (CombatManager.IsComboAvailable) return;
        
        if (isHeavyAttacking)
        {
            HeavyAttack();
        }
        else if (canLightTransition)
        {
            HandleLightAttack();
        }
    }

    /// <summary>
    /// When timer is over heavyChargedTimer we execute charged attack.
    /// And allow player to chain charged attacks
    /// </summary>
    private IEnumerator ChargingAttack()
    {
        float timer = -heavyChargedTimer / 2f;
        AnimationRequestor.AnimateGroundedAttackStateEnter();

        while (isHolding)
        {
            if (timer < heavyChargedTimer)
            {
                if (!CombatManager.IsComboAvailable) { 
                    timer += Time.deltaTime; 
                    if (canLightTransition) TryFinisherAttackOnHold();
                }
            }
            else
            {
                timer = -heavyChargedTimer;
                ChargedHeavyAttack(); 
            }
            yield return null;
        }
        AnimationRequestor.AnimateGroundedAttackStateExit();
    }

    private void TryFinisherAttackOnHold()
    {
        if(CombatManager.TryFinisherAttack())
        {
            CombatManager.CanMotionWarp = false; 
            CombatManager.ExecuteAttack(CombatManager.GetFinisherAttack());
            CombatManager.CanMotionWarp = true;
            isHolding = false;
            isLightTransition = false;
            canLightTransition = false;
            hasChargedAttacked = true;
        }
    }

    private void HandleLightAttack()
    {
        if (isHolding) return;

        CombatManager.IsComboTransition = true;

        isLightTransition = true;
    }

    private void HeavyAttack()
    {
        if ((hasChargedAttacked || hasSprintAttacked) && CombatManager.HasRecoveryAttack())
        {
            CombatManager.ExecuteAttack(CombatManager.GetRecoveryAttack());
            hasChargedAttacked = false;
            isHeavyAttacking = false;

            //Setting recovery combo transitions 
            comboValue = CombatManager.GetHeavyComboValue() - 1;
            CombatManager.ComboTransitionValue = CombatManager.GetLightComboValue();
        }
        else ExecuteHeavyAttack();
    }

    /// <summary>
    /// Executes Heavy Attack and gets attack assister to help
    /// </summary>
    private void ExecuteHeavyAttack()
    {
        if (CombatManager.exitComboUnlessRecover) return;

        if (!CombatManager.ExecuteAttack(CombatManager.GetHeavyCombo(comboValue))) { Context.AnimationFinished(); }

        CombatManager.ComboTransitionValue = CombatManager.GetLightComboValue();
        isHeavyAttacking = false;
    }

    /// <summary>
    /// Executes a heavy charged attack with attack assist
    /// </summary>
    /// <param name="chargedAttackType">This is which charged attack we want</param>
    private void ChargedHeavyAttack()
    {
        if (!CombatManager.ExecuteAttack(CombatManager.GetHeavyChargedAttack())) return; 

        if (CombatManager.canCombo())
        {
            comboValue = CombatManager.GetHeavyComboValue() - 1;
            CombatManager.ComboTransitionValue = CombatManager.GetLightComboValue();
        }
        else CombatManager.exitComboUnlessRecover = true;

        hasChargedAttacked = true;
    }

    private void ResetCombo()
    {
        comboValue = 0;
    }
    private void FirstHeavyAttack()
    {
        CheckForChargingAttack();
        //this is for the case where the player has queued in light attack
        //It has transfered over but before they have let go, so released won't
        //be triggered in here so we have this to double check
        if (!GameInput.Instance.StillHoldingHeavy()) isHolding = false;
        CoroutineHelper.Instance.StartCoroutine(CancellingChargedAttack());
    }

    /// <summary>
    /// Checking if there is a combo transition if so, we handle it and turn it off.
    /// </summary>
    private bool CheckForTransitionAttack()
    {
        if (CombatManager.IsComboTransition)
        {
            HandleComboTransition();
            return true;
        }
        else if (ShouldHandleSprintAttack())
        {
            HandleSprintAttack();
            return true;
        }
        if (CombatManager.ChargedAttackAfterParry)
        {
            CombatManager.ChargedAttackAfterParry = false;
            ChargedHeavyAttack();
            return true;
        }
        return false;
    }

    private void HandleComboTransition()
    {
        if (CombatManager.IsRecoveringFromAerialAttack)
        {
            CombatManager.IsRecoveringFromAerialAttack = false;

            if (CombatManager.canCombo())
            {
                comboValue = CombatManager.GetHeavyComboValue() - 1;
                CombatManager.ComboTransitionValue = CombatManager.GetLightComboValue();
            }
            else CombatManager.exitComboUnlessRecover = true;

            if (CombatManager.HasRecoveryAttack()) AerialAttackRecovery();
        }
        else
        {
            comboValue = CombatManager.ComboTransitionValue;
            FirstHeavyAttack();
        }

        CombatManager.IsComboingFromAerialAttack = false;
        CombatManager.IsComboTransition = false;
    }

    private IEnumerator CancellingChargedAttack()
    {
        while (isHolding)
        {
            yield return null;
        }

        if (!hasChargedAttacked) { ExecuteHeavyAttack(); }
    }


    private void StarterAttack()
    {
        ResetCombo();
        FirstHeavyAttack();
    }

    private bool ShouldHandleSprintAttack()
    {
        return MovementUtility.Speed >= 8 && weaponHasSprintAttacks;
    }

    private void AerialAttackRecovery()
    {
        CombatManager.ExecuteAttack(CombatManager.GetRecoveryAttack());

        comboValue = CombatManager.GetHeavyComboValue() - 1;
        CombatManager.ComboTransitionValue = CombatManager.GetLightComboValue();

        CombatManager.exitComboUnlessRecover = false;
    }

    private void HandleSprintAttack()
    {
        //sprint attack is a charged attack that can be comboed from
        AnimationRequestor.AnimateGroundedAttackStateExit();
        CombatManager.ExecuteAttack(CombatManager.GetHeavySprintAttack());

        //Handling Sprint combo transitioning
        if (CombatManager.canCombo())
        {
            comboValue = CombatManager.GetHeavyComboValue() - 1;
            CombatManager.ComboTransitionValue = CombatManager.GetLightComboValue();
        }
        else CombatManager.exitComboUnlessRecover = true;

        hasSprintAttacked = true;
    }

    private void ResetValues()
    {
        hasChargedAttacked = false;
        hasSprintAttacked = false;
        canLightTransition = false;
        isLightTransition = false;
        isHeavyAttacking = false;

        Weapon weapon = PlayerStateMachine.LocalInstance.WeaponHolster.ActiveWeapon;

        heavyComboLimit = weapon.HeavyCombo.Count;
        heavyChargedTimer = weapon.HeavyChargedAttack.chargingTimer;
        weaponHasSprintAttacks = weapon.HeavySprintAttack != null;
    }

    public override void ExitState()
    {
        if (!CombatManager.IsComboTransition) CombatManager.exitComboUnlessRecover = false;
        ResetCombo();
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        //Gets thrown when heavy attack starts not on first attack though
        CombatManager.OnHeavyAttackStarted += PlayerContext_CanPerformHeavyAttack;
        CombatManager.OnHeavyAttackFinished += CombatHelper_OnHeavyAttackFinished;
        CombatManager.OnLightAttackStarted += PlayerContext_CanPerformLightAttack;
    }

    private void UnsubscribeFromEvents()
    {
        CombatManager.OnHeavyAttackStarted -= PlayerContext_CanPerformHeavyAttack;
        CombatManager.OnHeavyAttackFinished -= CombatHelper_OnHeavyAttackFinished;
        CombatManager.OnLightAttackStarted -= PlayerContext_CanPerformLightAttack;
    }
}
