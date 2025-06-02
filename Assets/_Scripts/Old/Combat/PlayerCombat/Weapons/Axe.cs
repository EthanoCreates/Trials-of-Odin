using TrailsFX;
using UnityEngine;

namespace TrialsOfOdin.Combat
{
    public class Axe : Weapon
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
