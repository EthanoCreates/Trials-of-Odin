using System;
using UnityEngine;

namespace TrialsOfOdin.Combat
{
    public class Shield : MonoBehaviour
    {
        public event Action<float> OnDamageBlocked;
        private CharacterProfile user;

        [SerializeField] private BoxCollider shieldCollider;
        public AnimatorOverrideController overrideController;

        public void SetUser(CharacterProfile profile) => user = profile; 

        private void Start()
        {
            DisableShield();
        }

        public void EnableShield()
        {
            shieldCollider.enabled = true;
        }

        public void DisableShield()
        {
            shieldCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out DamageSource damageSource)) return;

            OnDamageBlocked?.Invoke(damageSource.DamageDealt);

            damageSource.blockedCharactersHit.Add(user);
        }
    }
}
