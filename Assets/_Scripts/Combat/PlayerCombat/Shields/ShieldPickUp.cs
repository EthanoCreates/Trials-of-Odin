using Unity.Netcode;
using UnityEngine;

public class ShieldPickUp : NetworkBehaviour, IInteractables
{
    [SerializeField] private PlayerShield playerShield;

    public void Interact()
    {
        WeaponHolster weaponHolster = PlayerStateMachine.LocalInstance.WeaponHolster;

        weaponHolster.shield = playerShield;
        weaponHolster.EquipShield();
    }
}
