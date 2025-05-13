using System;
using UnityEngine;


public class Health : MonoBehaviour
{
    public event EventHandler OnTakeDamage;
    public event EventHandler OnDeath;

    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth = 100;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damageTaken)
    {
        currentHealth -= damageTaken;
        OnTakeDamage?.Invoke(this, EventArgs.Empty);

        if(currentHealth <= 0)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
    public float GetCurrentHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }
}