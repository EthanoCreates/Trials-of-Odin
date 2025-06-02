using UnityEngine;
using TrailsFX;

namespace TrialsOfOdin.Combat
{
    public class Sword : Weapon
    {
        [SerializeField] private TrailEffect trail;

        public override void EnableWeaponVFX()
        {
            trail.checkWorldPosition = true;
            trail.UpdateMaterialProperties();
            trail.Restart();
        }

        public override void DisableWeaponVFX()
        {
            trail.checkWorldPosition = false;
            trail.UpdateMaterialProperties();
            trail.Restart();
        }
    }
}
