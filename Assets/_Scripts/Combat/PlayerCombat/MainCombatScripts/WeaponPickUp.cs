using Unity.Netcode;
using UnityEngine;

public class WeaponPickUp : NetworkBehaviour, IInteractables
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private bool createNewWeapon;

    public void Interact()
    {
        if (createNewWeapon) Instantiate(weapon).WeaponPickUp();
        else weapon.WeaponPickUp();
    }
}
