using System.Globalization;
using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public event EventHandler<TriggeredDamageCollider> OnDamageAvalible;
    public event EventHandler<TriggeredDamageCollider> OnDamageUnAvalible;

    public event EventHandler OnComboAvalible;
    public event EventHandler OnComboUnAvalible;

    public event EventHandler OnUnEquip;
    public event EventHandler OnEquip;

    public event EventHandler OnUnEquipShield;
    public event EventHandler OnEquipShield;

    public event EventHandler OnShootWeapon;
    public event EventHandler OnReloadWeapon;

    public event EventHandler OnApplyJumpVelocity;
    public event EventHandler OnRollLand;

    public event EventHandler OnRage;
    private void ApplyJumpVelocity()
    {
        OnApplyJumpVelocity?.Invoke(this, EventArgs.Empty);
    }

    private void ComboAvailabe()
    {
        OnComboAvalible?.Invoke(this, EventArgs.Empty);
    }

    private void ComboUnAvailable()
    {
        OnComboUnAvalible?.Invoke(this, EventArgs.Empty);
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
        OnUnEquip?.Invoke(this, EventArgs.Empty);
    }

    private void EquipWeapon()
    {
        OnEquip?.Invoke(this, EventArgs.Empty);
    }

    private void UnEquipShield()
    {
        OnUnEquipShield?.Invoke(this, EventArgs.Empty);
    }

    private void EquipShield()
    {
        OnEquipShield?.Invoke(this, EventArgs.Empty);
    }

    private void ShootWeapon()
    {
        OnShootWeapon?.Invoke(this, EventArgs.Empty);
    }

    private void ReloadWeapon()
    {
        OnReloadWeapon?.Invoke(this, EventArgs.Empty);
    }

    private void RollLand()
    {
        OnRollLand?.Invoke(this, EventArgs.Empty);
    }

    private void RageTransition()
    {
        OnRage?.Invoke(this, EventArgs.Empty);
    }

    public class TriggeredDamageCollider
    {
        public int collider;    
    }
}
