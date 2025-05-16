using System;
using UnityEngine;
using TrialsOfOdin.Combat;
public class PlayerAnimationRequestor
{
    //Movement Events
    public event EventHandler<OnPlayerMoveEventArgs> OnPlayerMove;
    public event EventHandler OnResetPlayerMovement;
    public event EventHandler OnGrounded;
    public event EventHandler<OnPlayerLandEventArgs> OnPlayerLand;
    public event EventHandler OnPlayerJump;
    public event EventHandler OnFreeFalling;
    public event EventHandler OnExitFreeFalling;
    public event EventHandler OnVault;
    public event EventHandler OnClimb;
    public event EventHandler OnEquip;
    public event EventHandler OnUnEquip;
    public event EventHandler OnEquipShield;
    public event EventHandler OnUnEquipShield;
    public event EventHandler<OnTurnEventArgs> OnTurn;
    public event EventHandler OnTurnStart;


    //Combat Events
    public event EventHandler<AttackAnimationData> OnAttackAnim;
    public event EventHandler OnGroundedAttackStateEnter;
    public event EventHandler OnGroundedAttackStateExit;
    public event EventHandler <OnDodgeArgs> OnDodge;
    public event EventHandler<OnBlockEventArgs> OnBlock;
    public event EventHandler OnParry;
    public event EventHandler OnBlockImpact;
    public event EventHandler<OnImpactEventArgs> OnImpact;
    public event EventHandler OnAim;
    public event EventHandler OnAimExit;
    public event EventHandler OnReload;
    public event EventHandler OnReloadCancel;
    public event EventHandler OnRage;

    public event EventHandler OnNewActiveWeapon;

    //Extra events
    public event EventHandler OnResetCamera;
    public event EventHandler OnResetAnimator;
    public event EventHandler OnAnimExit;

    public PlayerAnimationRequestor(PlayerCombatManager playerCombatManager)
    {
        WeaponHolster weaponHolster = PlayerStateMachine.LocalInstance.WeaponHolster;

        weaponHolster.OnEquip += WeaponHolster_OnEquip;
        weaponHolster.OnUnEquip += WeaponHolster_OnUnEquip;
        weaponHolster.OnEquipShield += WeaponHolster_OnEquipShield;
        weaponHolster.OnUnEquipShield += WeaponHolster_OnUnEquipShield;
        weaponHolster.OnNewActiveWeapon += WeaponHolster_OnNewActiveWeapon;

        playerCombatManager.OnExecuteAttack += AnimateAttack;
        playerCombatManager.OnRageAnimation += PlayerCombatManager_OnRage;
    }

    private void PlayerCombatManager_OnRage(object sender, EventArgs e)
    {
        OnRage?.Invoke(this, EventArgs.Empty);
    }

    private void WeaponHolster_OnNewActiveWeapon(object sender, WeaponHolster.WeaponEventArgs e)
    {
        OnNewActiveWeapon?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateParry()
    {
        OnParry?.Invoke(this, EventArgs.Empty);
    }

    private void WeaponHolster_OnUnEquipShield(object sender, EventArgs e)
    {
        OnUnEquipShield?.Invoke(this, EventArgs.Empty);
    }

    private void WeaponHolster_OnEquipShield(object sender, EventArgs e)
    {
        OnEquipShield?.Invoke(this, EventArgs.Empty);
    }

    private void WeaponHolster_OnUnEquip(object sender, WeaponHolster.WeaponEventArgs e)
    {
        OnUnEquip?.Invoke(this, EventArgs.Empty);
    }

    private void WeaponHolster_OnEquip(object sender, WeaponHolster.EquippedWeaponType e)
    {
        OnEquip?.Invoke(this, EventArgs.Empty);
    }

    public void ResetCamera()
    {
        OnResetCamera?.Invoke(this, EventArgs.Empty);
    }

    public void ResetAnimator()
    {
        OnResetAnimator?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateJump()
    {
        OnPlayerJump?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateTurn(float turnBlend)
    {
        OnTurn?.Invoke(this, new OnTurnEventArgs
        {
            turnBlend = turnBlend,
        });
    }

    public void AnimateTurnStart()
    {
        OnTurnStart?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateLocomotion(Vector3 inputDirection, float playerSpeed)
    {
        OnPlayerMove?.Invoke(this, new OnPlayerMoveEventArgs
        {
            horizontalAnimationBlendSpeed = inputDirection.x * playerSpeed,
            verticalAnimationBlendSpeed = inputDirection.z * playerSpeed
        });
    }

    public void ResetLocomotion()
    {
        OnResetPlayerMovement?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateVault()
    {
        OnVault?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateClimb()
    {
        OnClimb?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateFalling()
    {
        OnFreeFalling?.Invoke(this, EventArgs.Empty);
    }
    public void AnimateExitFalling()
    {
        OnExitFreeFalling?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateGrounded()
    {
        OnGrounded?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateLand(float verticalVelocity)
    {
        OnPlayerLand?.Invoke(this, new OnPlayerLandEventArgs
        {
            landVelocity = verticalVelocity
        });
    }

    public void AnimateGroundedAttackStateEnter()
    {
        OnGroundedAttackStateEnter?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateGroundedAttackStateExit()
    {
        OnGroundedAttackStateExit?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateAttack(object sender, AttackAnimationData e)
    {
        OnAttackAnim?.Invoke(this, e);
    }
    
    public void AnimateBlock(bool isBlocking)
    {
        OnBlock?.Invoke(this, new OnBlockEventArgs
        {
            isBlockingEventArgs = isBlocking
        });
    }

    public void AnimateBlockImpact()
    {
        OnBlockImpact?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateImpact(float impactX, float impactZ)
    {
        OnImpact?.Invoke(this, new OnImpactEventArgs
        {
            impactX = impactX,
            impactZ = impactZ
        });
    }

    public void AnimateDodge(float dodgeX, float dodgeY)
    {
        OnDodge?.Invoke(this, new OnDodgeArgs
        {
            dodgeHorizontal = dodgeX,
            dodgeVertical = dodgeY
        });
    }

    public void AnimateAim()
    {
        OnAim?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateAimExit()
    {
        OnAimExit?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateReload()
    {
        OnReload?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateReloadCancel()
    {
        OnReloadCancel?.Invoke(this, EventArgs.Empty);
    }

    public void AnimationExit()
    {
        OnAnimExit?.Invoke(this, EventArgs.Empty);
    }

    //Movement EventArgs
    public class OnPlayerMoveEventArgs
    {
        public float horizontalAnimationBlendSpeed;
        public float verticalAnimationBlendSpeed;
    }
    public class OnDodgeArgs
    {
        public float dodgeHorizontal;
        public float dodgeVertical;
    }

    public class OnImpactEventArgs
    {
        public float impactX;
        public float impactZ;
    }

    public class OnPlayerLandEventArgs
    {
        public float landVelocity;
    }

    public class OnTurnEventArgs
    {
        public float turnBlend;
    }

    //CombatEventArgs
    public class OnBlockEventArgs
    {
        public bool isBlockingEventArgs;
    }
}

public class AttackAnimationData
{
    public string stateName;
    public float animationSpeed;
}
