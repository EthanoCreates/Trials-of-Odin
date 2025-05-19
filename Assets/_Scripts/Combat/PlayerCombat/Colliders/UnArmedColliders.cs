using System;
using UnityEngine;

namespace TrialsOfOdin.Combat
{
    public class UnArmedColliders : DamageSource
    {
        public event EventHandler<CollisionEventArgs> OnEnemyCollision;
        public event EventHandler<CollisionEventArgs> OnShieldCollision;

        private UnArmed unArmed;


        public class CollisionEventArgs
        {
            public Collision collision;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (unArmed == null) unArmed = PlayerStateMachine.LocalInstance.WeaponHolster.unArmedWeapon.gameObject.GetComponent<UnArmed>();

            if (unArmed.BlockedAttackID == unArmed.AttackID) return;

            DamageAmount = unArmed.DamageAmount;
            AttackID = unArmed.AttackID;
            StanceBreakPower = unArmed.StanceBreakPower;

            unArmed.OnCollisionEnter(collision);
        }
    }
}
