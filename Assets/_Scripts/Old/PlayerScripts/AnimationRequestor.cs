using System;
using UnityEngine;
using TrialsOfOdin.Combat;
using TrialsOfOdin.State;
public class AnimationRequestor
{
    //Movement Events
    public event EventHandler<OnPlayerMoveEventArgs> OnPlayerMove;
    public event Action OnResetPlayerMovement;
    public event Action OnGrounded;
    public event EventHandler<OnPlayerLandEventArgs> OnPlayerLand;
    public event Action OnPlayerJump;
    public event Action OnFreeFalling;
    public event Action OnExitFreeFalling;
    public event Action OnVault;
    public event Action OnClimb;
    public event Action OnEquip;
    public event Action OnUnEquip;
    public event Action OnEquipShield;
    public event Action OnUnEquipShield;
    public event EventHandler<OnTurnEventArgs> OnTurn;
    public event Action OnTurnStart;


    //Combat Events
    public event EventHandler<AttackAnimationData> OnAttackAnim;
    public event Action OnGroundedAttackStateEnter;
    public event Action OnGroundedAttackStateExit;
    public event EventHandler <OnDodgeArgs> OnDodge;
    public event EventHandler<OnBlockEventArgs> OnBlock;
    public event Action OnParry;
    public event Action OnBlockImpact;
    public event EventHandler<OnImpactEventArgs> OnImpact;
    public event Action OnAim;
    public event Action OnAimExit;
    public event Action OnReload;
    public event Action OnReloadCancel;
    public event Action OnRage;

    public event Action OnNewActiveWeapon;

    //Extra events
    public event Action OnResetCamera;
    public event Action OnResetAnimator;
    public event Action OnAnimExit;

    public AnimationRequestor(CombatSystemPro combatSystemPro)
    {
        WeaponHolster weaponHolster = combatSystemPro.WeaponHolster;

        weaponHolster.OnEquip += WeaponHolster_OnEquip;
        weaponHolster.OnUnEquip += WeaponHolster_OnUnEquip;
        weaponHolster.OnEquipShield += WeaponHolster_OnEquipShield;
        weaponHolster.OnUnEquipShield += WeaponHolster_OnUnEquipShield;
        weaponHolster.OnNewActiveWeapon += WeaponHolster_OnNewActiveWeapon;

        combatSystemPro.OnExecuteAttackAnimation += AnimateAttack;
        //playerCombatManager.OnExecuteAttack += AnimateAttack;
        //playerCombatManager.OnRageAnimation += PlayerCombatManager_OnRage;
    }

    private void PlayerCombatManager_OnRage()
    {
        OnRage?.Invoke();
    }

    private void WeaponHolster_OnNewActiveWeapon(Weapon weapon)
    {
        OnNewActiveWeapon?.Invoke();
    }

    public void AnimateParry()
    {
        OnParry?.Invoke();
    }

    private void WeaponHolster_OnUnEquipShield()
    {
        OnUnEquipShield?.Invoke();
    }

    private void WeaponHolster_OnEquipShield()
    {
        OnEquipShield?.Invoke();
    }

    private void WeaponHolster_OnUnEquip(Weapon weapon)
    {
        OnUnEquip?.Invoke();
    }

    private void WeaponHolster_OnEquip(WeaponHolster.WeaponSlotType weaponSlotType)
    {
        OnEquip?.Invoke();
    }

    public void ResetCamera()
    {
        OnResetCamera?.Invoke();
    }

    public void ResetAnimator()
    {
        OnResetAnimator?.Invoke();
    }

    public void AnimateJump()
    {
        OnPlayerJump?.Invoke();
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
        OnTurnStart?.Invoke();
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
        OnResetPlayerMovement?.Invoke();
    }

    public void AnimateVault()
    {
        OnVault?.Invoke();
    }

    public void AnimateClimb()
    {
        OnClimb?.Invoke();
    }

    public void AnimateFalling()
    {
        OnFreeFalling?.Invoke();
    }
    public void AnimateExitFalling()
    {
        OnExitFreeFalling?.Invoke();
    }

    public void AnimateGrounded()
    {
        OnGrounded?.Invoke();
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
        OnGroundedAttackStateEnter?.Invoke();
    }

    public void AnimateGroundedAttackStateExit()
    {
        OnGroundedAttackStateExit?.Invoke();
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
        OnBlockImpact?.Invoke();
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
        OnAim?.Invoke();
    }

    public void AnimateAimExit()
    {
        OnAimExit?.Invoke();
    }

    public void AnimateReload()
    {
        OnReload?.Invoke();
    }

    public void AnimateReloadCancel()
    {
        OnReloadCancel?.Invoke();
    }

    public void AnimationExit()
    {
        OnAnimExit?.Invoke();
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
