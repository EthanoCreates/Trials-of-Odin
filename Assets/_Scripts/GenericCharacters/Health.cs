using System;
using UnityEngine;


namespace TrialsOfOdin.Stats
{
    public class Health : MonoBehaviour
    {
        public delegate void TakeDamageEvent(float currentHealth, float maxHealth);
        public event TakeDamageEvent OnTakeDamage;
        public event Action OnDeath;

        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float currentHealth = 100;

        private void Start()
        {
            currentHealth = maxHealth;
        }
        public void TakeDamage(float damageTaken)
        {
            currentHealth -= damageTaken;
            OnTakeDamage?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0) OnDeath?.Invoke();    
        }
        public float GetCurrentHealth() { return currentHealth; }
        public float GetMaxHealth() { return maxHealth; }
    }
}