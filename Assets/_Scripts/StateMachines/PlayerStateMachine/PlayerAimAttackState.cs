using TrialsOfOdin.Combat;
using UnityEngine;
public class PlayerAimAttackState : PlayerState
{
    IAimAttacks weaponAimSystem;
    IShootable iShootable;

    public PlayerAimAttackState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState stateKey) : base(utilities, stateKey) { }

    public override void EnterState()
    {
        AimSetUp();
        InitializeSubState();
    }

    public override void ExitState()
    {
        UnsubscribeFromEvents();
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return StateKey;
    }

    private void InitializeSubState()
    {
        SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.CombatMovement));
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
        CombatManager.OnHeavyAttackStarted += OnShootHeavy;
        CombatManager.OnLightAttackStarted += OnShootLight;
    }

    private void OnShootLight(object sender, System.EventArgs e)
    {
        if (!weaponAimSystem.IsReloaded()) return;
        CombatManager.ExecuteAttack(CombatManager.GetLightAimAttack());
        CombatManager.IsComboAvailable = false;
        if (iShootable != null) weaponAimSystem.LightAimAttack();
        else { PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon += LocalInstance_OnShootLightWeapon; }
    }

    private void OnShootHeavy(object sender, System.EventArgs e)
    {
        if (!weaponAimSystem.IsReloaded()) return;
        CombatManager.ExecuteAttack(CombatManager.GetHeavyAimAttack());
        CombatManager.IsComboAvailable = false;
        if (iShootable != null) weaponAimSystem.HeavyAimAttack();
        else { PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon += LocalInstance_OnShootHeavyWeapon; }
    }

    private void LocalInstance_OnShootHeavyWeapon(object sender, System.EventArgs e)
    {
        weaponAimSystem.HeavyAimAttack();
        if (weaponAimSystem.GetAmmoAmount() == 0) CombatManager.SwitchFromAimWeapon = true;
    }

    private void LocalInstance_OnShootLightWeapon(object sender, System.EventArgs e)
    {
        weaponAimSystem.LightAimAttack();
        if (weaponAimSystem.GetAmmoAmount() == 0) CombatManager.SwitchFromAimWeapon = true;
    }

    private void UnsubscribeFromEvents()
    {
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon -= LocalInstance_OnShootLightWeapon;
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnShootWeapon -= LocalInstance_OnShootHeavyWeapon;
        CombatManager.OnHeavyAttackStarted -= OnShootHeavy;
        CombatManager.OnLightAttackStarted -= OnShootLight;
    }
}
