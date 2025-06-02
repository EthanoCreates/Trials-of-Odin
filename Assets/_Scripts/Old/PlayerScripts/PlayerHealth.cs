using FIMSpace.FProceduralAnimation;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public event EventHandler OnDeath;

    [SerializeField] private float maxHealth;
    [ShowInInspector]
    [ReadOnly] public float CurrentHealth { get; private set; }
    [SerializeField] private float deathImpactForce = 10;
    [SerializeField] private Ragdoll ragdoll;
    private RagdollAnimator2 ragdollAnimator;
    [HideInInspector] public int currentAttackID;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount, int attackID, Vector3 forceDirection)
    { 

        currentAttackID = attackID;
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Die(forceDirection);
        }

    }  

    private void Die(Vector3 forceDirection)
    {
        //ragdoll.ActivateRagdoll();
        //ragdoll.ApplyForce(forceDirection * deathImpactForce);
        OnDeath?.Invoke(this, EventArgs.Empty);
    }
}
