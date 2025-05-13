using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour, IAnimatorHelper
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speedChangeRate = .2f;
    [SerializeField] private OwnerNetworkAnimator ownerNetworkAnimator;

    public event EventHandler OnAnimStarted;
    public event EventHandler OnAnimFinished;

    private float combatCrossFadeTime = .2f;
    private int combatLayer = 2;

    // animation IDs
    private int animIDGroundedAttackState;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDHorizontal;
    private int animIDVertical;
    private int animIDClimb;
    private int animIDVault;
    private int animIDLandType;
    private int animIDDodge;
    private int animIDDodgeHorizontal;
    private int animIDDodgeVertical;
    private int animIDBlocking;
    private int animIDAim;
    private int animIDCombatSpeed;
    private int animIDExitTurn;
    private int animIDTurn;
    private int animIDInteruptCombat;
    private int animIDInteruptUB;


    private void Start()
    {
        PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;
        if (localPlayerStateMachine.gameObject == this.gameObject)
        {
            PlayerAnimationRequestor animationRequestor = localPlayerStateMachine.AnimationRequestor;

            animationRequestor.OnPlayerJump += LocalInstance_OnPlayerJump;
            animationRequestor.OnPlayerMove += LocalInstance_OnPlayerMove;
            animationRequestor.OnResetPlayerMovement += AnimationRequestor_OnResetPlayerMovement;
            animationRequestor.OnGrounded += LocalInstance_OnGrounded;
            animationRequestor.OnPlayerLand += LocalInstance_OnPlayerLand;
            animationRequestor.OnFreeFalling += LocalInstance_OnFreeFalling;
            animationRequestor.OnExitFreeFalling += LocalInstance_OnExitFreeFalling;
            animationRequestor.OnVault += LocalInstance_OnVault;
            animationRequestor.OnClimb += LocalInstance_OnClimb;
            animationRequestor.OnDodge += LocalInstance_OnDodge;
            animationRequestor.OnBlock += LocalInstance_OnBlock;
            animationRequestor.OnParry += AnimationRequestor_OnParry;
            animationRequestor.OnBlockImpact += LocalInstance_OnBlockImpact;
            animationRequestor.OnImpact += LocalInstance_OnImpact;
            animationRequestor.OnGroundedAttackStateEnter += LocalInstance_OnGroundedAttackStateEnter;
            animationRequestor.OnGroundedAttackStateExit += LocalInstance_OnGroundedAttackStateExit;
            animationRequestor.OnAttackAnim += LocalInstance_OnAttack;
            animationRequestor.OnAim += AnimationRequestor_OnAim;
            animationRequestor.OnAimExit += AnimationRequestor_OnAimExit;
            animationRequestor.OnReloadCancel += AnimExit;
            animationRequestor.OnReload += AnimationRequestor_OnReload;
            animationRequestor.OnNewActiveWeapon += AnimationRequestor_OnNewActiveWeapon;
            animationRequestor.OnEquip += AnimationRequestor_OnEquip;
            animationRequestor.OnUnEquip += AnimationRequestor_OnUnEquip;
            animationRequestor.OnEquipShield += AnimationRequestor_OnEquipShield;
            animationRequestor.OnUnEquipShield += AnimationRequestor_OnUnEquipShield;
            animationRequestor.OnResetAnimator += LocalInstance_OnResetAnimator;
            animationRequestor.OnAnimExit += AnimExit;
            animationRequestor.OnTurn += AnimationRequestor_OnTurn;
            animationRequestor.OnTurnStart += AnimationRequestor_OnTurnStart;
            animationRequestor.OnRage += AnimationRequestor_OnRage;
            AssignAnimationIDs();
        }
    }

    private void AnimationRequestor_OnRage(object sender, EventArgs e)
    {
        animator.Play("Rage");
    }

    private void AnimationRequestor_OnUnEquipShield(object sender, System.EventArgs e)
    {
        animator.Play("UnEquipShield");
    }

    private void AnimationRequestor_OnEquipShield(object sender, System.EventArgs e)
    {
        animator.Play("EquipShield");
    }

    private void AnimationRequestor_OnParry(object sender, System.EventArgs e)
    {
        animator.Play("Parry");
    }

    private void AnimationRequestor_OnResetPlayerMovement(object sender, System.EventArgs e)
    {
        animator.SetFloat(animIDHorizontal, 0f);
        animator.SetFloat(animIDVertical, 0f);
    }

    private void AnimationRequestor_OnTurnStart(object sender, System.EventArgs e)
    {
        animator.Play("Turn");
    }

    private void AnimationRequestor_OnTurn(object sender, PlayerAnimationRequestor.OnTurnEventArgs e)
    {
        animator.SetFloat(animIDTurn, e.turnBlend);
    }

    private void AnimationRequestor_OnAimExit(object sender, System.EventArgs e)
    {
        animator.SetBool(animIDAim, false);
    }

    private void AnimationRequestor_OnAim(object sender, System.EventArgs e)
    {
        animator.SetBool(animIDAim, true);
    }

    private void AnimationRequestor_OnReload(object sender, System.EventArgs e)
    {
        animator.Play("Reload");
    }

    private void AnimExit(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDExitTurn);
    }

    private void AnimationRequestor_OnUnEquip(object sender, System.EventArgs e)
    {
        animator.SetFloat(animIDCombatSpeed, 1);
        animator.Play("UnEquip",1);
    }

    private void AnimationRequestor_OnEquip(object sender, System.EventArgs e)
    {
        animator.SetFloat(animIDCombatSpeed, 1);
        animator.Play("Equip",1);
    }

    private void AnimationRequestor_OnNewActiveWeapon(object sender, System.EventArgs e)
    {
        SwitchAnimatorController();
    }

    private void LocalInstance_OnImpact(object sender, PlayerAnimationRequestor.OnImpactEventArgs e)
    {
        animator.SetFloat("ImpactX", e.impactX);
        animator.SetFloat("ImpactZ", e.impactZ);
        animator.Play("Impact", 1);
    }

    private void LocalInstance_OnBlockImpact(object sender, System.EventArgs e)
    {
        animator.Play("BlockImpact", combatLayer);
    }

    private void LocalInstance_OnResetAnimator(object sender, System.EventArgs e)
    {
        AnimationFinishedResetAll();
    }

    private void LocalInstance_OnBlock(object sender, PlayerAnimationRequestor.OnBlockEventArgs e)
    {
        animator.SetBool(animIDBlocking, e.isBlockingEventArgs);

        if (e.isBlockingEventArgs)
        {
            AnimationFinishedResetAll();
            animator.SetTrigger(animIDInteruptCombat);
            animator.SetTrigger(animIDInteruptUB);
        }
    }

    private void LocalInstance_OnClimb(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDClimb);
    }

    private void LocalInstance_OnVault(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDVault);
    }

    private void LocalInstance_OnDodge(object sender, PlayerAnimationRequestor.OnDodgeArgs e)
    {
        animator.SetFloat(animIDDodgeHorizontal, e.dodgeHorizontal);
        animator.SetFloat(animIDDodgeVertical, e.dodgeVertical);

        animator.SetTrigger(animIDInteruptCombat);
        animator.SetTrigger(animIDInteruptUB);

        animator.SetTrigger(animIDDodge);
    }

    private void LocalInstance_OnPlayerLand(object sender, PlayerAnimationRequestor.OnPlayerLandEventArgs e)
    {
        if(e.landVelocity < -25f)
        {
            animator.SetFloat(animIDLandType, 2);
        }
        else if (e.landVelocity < -15f && e.landVelocity >= -25f)
        {
            animator.SetFloat(animIDLandType, 1);
        }
        else
        {
            animator.SetFloat(animIDLandType, 0);
        }
    }


    private void LocalInstance_OnExitFreeFalling(object sender, System.EventArgs e)
    {
        animator.SetBool(animIDFreeFall, false);
    }

    private void LocalInstance_OnFreeFalling(object sender, System.EventArgs e)
    {
        animator.SetBool(animIDFreeFall, true);
    }

    private void LocalInstance_OnGrounded(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDGrounded);
    }

    private void LocalInstance_OnPlayerJump(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDJump);
        animator.SetBool(animIDGrounded, false);
    }

    private void LocalInstance_OnPlayerMove(object sender, PlayerAnimationRequestor.OnPlayerMoveEventArgs e)
    {
        float newHorizontalBlend = e.horizontalAnimationBlendSpeed;
        float newVerticalBlend = e.verticalAnimationBlendSpeed;

        animator.SetFloat(animIDHorizontal, newHorizontalBlend, .1f, Time.deltaTime);
        animator.SetFloat(animIDVertical, newVerticalBlend, .1f, Time.deltaTime);
    }


    private void LocalInstance_OnAttack(object sender, AttackAnimationData e)
    {
        animator.SetFloat(animIDCombatSpeed, e.animationSpeed);

        animator.CrossFade(e.stateName, combatCrossFadeTime, combatLayer);
    }

    private void LocalInstance_OnGroundedAttackStateEnter(object sender, System.EventArgs e)
    {
        animator.SetFloat(animIDHorizontal, 0);
        animator.SetFloat(animIDVertical, 0);
        animator.SetBool(animIDGroundedAttackState, true);
    }

    private void LocalInstance_OnGroundedAttackStateExit(object sender, System.EventArgs e)
    {
        animator.SetBool(animIDGroundedAttackState, false);
    }
    //Helper method
    private void AnimationFinishedResetAll()
    {
        DisableRootMotion();
        AnimationFinished();
        EnablePlayerMovement();
    }

    //Animator Helper Interface methods
    public void AnimationStarted()
    {
        OnAnimStarted?.Invoke(this, EventArgs.Empty);
    }

    public void AnimationFinished()
    {
        OnAnimFinished?.Invoke(this, EventArgs.Empty);
    }

    public void EnableRootMotion()
    {
        if (!animator.applyRootMotion)
        {
            animator.applyRootMotion = true;
        }
    }

    public void DisableRootMotion()
    {
        animator.applyRootMotion = false;
    }

    public void EnablePlayerMovement()
    {
        GameInput.Instance.EnablePlayerMovement();
    }

    public void DisablePlayerMovement()
    {
        GameInput.Instance.DisablePlayerMovement();
    }

    //string to cached int
    private void AssignAnimationIDs()
    {
        animIDGroundedAttackState = Animator.StringToHash("GroundedAttackState");
        animIDHorizontal = Animator.StringToHash("Horizontal");
        animIDVertical = Animator.StringToHash("Vertical");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDLandType = Animator.StringToHash("LandType");
        animIDDodge = Animator.StringToHash("Dodge");
        animIDDodgeHorizontal = Animator.StringToHash("DodgeHorizontal");
        animIDDodgeVertical = Animator.StringToHash("DodgeVertical");
        animIDClimb = Animator.StringToHash("Climb");
        animIDVault = Animator.StringToHash("Vault");
        animIDBlocking = Animator.StringToHash("Blocking");
        animIDAim = Animator.StringToHash("Aim");
        animIDCombatSpeed = Animator.StringToHash("CombatSpeed");
        animIDExitTurn = Animator.StringToHash("ExitTurn");
        animIDTurn = Animator.StringToHash("Turn");
        animIDInteruptCombat = Animator.StringToHash("InteruptCombat");
        animIDInteruptUB = Animator.StringToHash("InteruptUB");
    }
    public void SwitchAnimatorController()
    {
        WeaponHolster weaponHolster = PlayerStateMachine.LocalInstance.WeaponHolster;

        AnimatorOverrideController weaponOverride = weaponHolster.ActiveWeapon.WeaponData.animatorOverrideController;

        if (weaponHolster.HasShield)
        {
            if (weaponHolster.HasShieldEquipped)
            {
                animator.runtimeAnimatorController = MergeAnimatorOverrides(weaponOverride, weaponHolster.shieldOverrideController);
                animator.runtimeAnimatorController.name = weaponOverride.name + " + Shield";
            }
            else
            {
                animator.runtimeAnimatorController = weaponOverride;
                if (weaponHolster.ActiveWeapon == weaponHolster.unArmedWeapon)
                {
                    animator.runtimeAnimatorController = MergeAnimatorOverrides(weaponOverride, weaponHolster.combatMovementOverride);
                    animator.runtimeAnimatorController.name = weaponOverride.name + " + Shield on back";
                }
            }
        }
        else animator.runtimeAnimatorController = weaponOverride;
    }

    private AnimatorOverrideController MergeAnimatorOverrides(AnimatorOverrideController baseController, AnimatorOverrideController overrideController2)
    {
        // Create a new AnimatorOverrideController based on the weapon's overrides
        var combinedOverrideController = new AnimatorOverrideController(baseController);

        // Retrieve the shield overrides
        var shieldOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController2.GetOverrides(shieldOverrides);

        // Retrieve the weapon overrides
        var weaponOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        baseController.GetOverrides(weaponOverrides);

        // Create a dictionary to store the combined overrides
        var combinedOverrides = new Dictionary<AnimationClip, AnimationClip>();

        // Add all weapon overrides to the combined dictionary
        foreach (var weaponOverride in weaponOverrides)
        {
            combinedOverrides[weaponOverride.Key] = weaponOverride.Value;
        }

        // Overwrite with shield overrides where applicable
        foreach (var overrideState in shieldOverrides)
        {
            if (overrideState.Value != null) // Only override if the shield has a valid clip
            {
                combinedOverrides[overrideState.Key] = overrideState.Value;
            }
        }

        // Apply the combined overrides to the new controller
        combinedOverrideController.ApplyOverrides(combinedOverrides.ToList());
        return combinedOverrideController;
    }

}
