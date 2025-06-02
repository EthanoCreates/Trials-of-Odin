using TrialsOfOdin.Combat;
using UnityEngine;
namespace TrialsOfOdin.State
{ 
public class PlayerAimAttackState : PlayerState
{
    IAimAttacks weaponAimSystem;
    IShootable iShootable;

    public PlayerAimAttackState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }

    public override void EnterState()
    {
        AimSetUp();
        InitializeSubState();
    }

    public override void ExitState()
    {
        UnsubscribeFromEvents();
    }

    public override ECharacterState GetNextState()
    {
        return StateKey;
    }

    private void InitializeSubState()
    {
        SetSubState(Utilities.GetStateInstance(ECharacterState.CombatMovement));
    }

    public override void UpdateState() { }

    private void AimSetUp()
    {
        Weapon weapon = PlayerStateMachine.LocalInstance.WeaponHolster.ActiveWeapon;
        weaponAimSystem = weapon.GetComponent<IAimAttacks>();
        iShootable = weapon.GetComponent<IShootable>();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        //CombatManager.OnHeavyAttackStarted += OnShootHeavy;
        //CombatManager.OnLightAttackStarted += OnShootLight;
    }

    private void OnShootLight()
    {
        if (!weaponAimSystem.IsReloaded()) return;
        //CombatManager.ExecuteAttack(CombatManager.GetLightAimAttack());
        //CombatManager.IsComboAvailable = false;
        if (iShootable != null) weaponAimSystem.LightAimAttack();
        else { PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon += LocalInstance_OnShootLightWeapon; }
    }

    private void OnShootHeavy()
    {
        if (!weaponAimSystem.IsReloaded()) return;
        //CombatManager.ExecuteAttack(CombatManager.GetHeavyAimAttack());
        //CombatManager.IsComboAvailable = false;
        if (iShootable != null) weaponAimSystem.HeavyAimAttack();
        else { PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon += LocalInstance_OnShootHeavyWeapon; }
    }

    private void LocalInstance_OnShootHeavyWeapon()
    {
        weaponAimSystem.HeavyAimAttack();
        //if (weaponAimSystem.GetAmmoAmount() == 0) CombatManager.SwitchFromAimWeapon = true;
    }

    private void LocalInstance_OnShootLightWeapon()
    {
        weaponAimSystem.LightAimAttack();
        //if (weaponAimSystem.GetAmmoAmount() == 0) CombatManager.SwitchFromAimWeapon = true;
    }

    private void UnsubscribeFromEvents()
    {
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon -= LocalInstance_OnShootLightWeapon;
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon -= LocalInstance_OnShootHeavyWeapon;
        //CombatManager.OnHeavyAttackStarted -= OnShootHeavy;
        //CombatManager.OnLightAttackStarted -= OnShootLight;
    }
}
}