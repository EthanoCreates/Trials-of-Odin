using System.Globalization;
using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public event EventHandler<TriggeredDamageCollider> OnDamageAvalible;
    public event EventHandler<TriggeredDamageCollider> OnDamageUnAvalible;

    public event Action OnComboAvalible;
    public event Action OnComboUnAvalible;

    public event Action OnUnEquip;
    public event Action OnEquip;

    public event Action OnUnEquipShield;
    public event Action OnEquipShield;

    public event Action OnShootWeapon;
    public event Action OnReloadWeapon;

    public event Action OnApplyJumpVelocity;
    public event Action OnRollLand;

    public event Action OnRage;
    private void ApplyJumpVelocity()
    {
        OnApplyJumpVelocity?.Invoke();
    }

    private void ComboAvailabe()
    {
        OnComboAvalible?.Invoke();
    }

    public void ComboUnAvailable()
    {
        OnComboUnAvalible?.Invoke();
    }

    private void ComboAndDamageUnAvailable(int collider)
    {
        ComboUnAvailable();
        DamageUnavailable(collider);
    }

    private void DamageUnavailable(int collider)
    {
        OnDamageUnAvalible?.Invoke(this, new TriggeredDamageCollider
        {
            collider = collider
        });
    }

    private void DamageAvailible(int collider)
    {
        OnDamageAvalible?.Invoke(this, new TriggeredDamageCollider
        {
            collider = collider
        });
    }

    private void UnEquipWeapon()
    {
        OnUnEquip?.Invoke();
    }

    private void EquipWeapon()
    {
        OnEquip?.Invoke();
    }

    private void UnEquipShield()
    {
        OnUnEquipShield?.Invoke();
    }

    private void EquipShield()
    {
        OnEquipShield?.Invoke();
    }

    private void ShootWeapon()
    {
        OnShootWeapon?.Invoke();
    }

    private void ReloadWeapon()
    {
        OnReloadWeapon?.Invoke();
    }

    private void RollLand()
    {
        OnRollLand?.Invoke();
    }

    private void RageTransition()
    {
        OnRage?.Invoke();
    }

    public class TriggeredDamageCollider
    {
        public int collider;    
    }
}
