using TrialsOfOdin.Combat;
using TrialsOfOdin.State;
using Unity.Netcode;
using UnityEngine;

public class ShieldPickUp : NetworkBehaviour, IInteractables
{
    [SerializeField] private Shield shield;

    public void Interact()
    {
        WeaponHolster weaponHolster = PlayerStateMachine.LocalInstance.WeaponHolster;

        weaponHolster.shield = shield;
        weaponHolster.EquipShield();
    }
}
