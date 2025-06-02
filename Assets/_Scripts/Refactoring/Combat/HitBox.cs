using System;
using TrialsOfOdin;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] protected float damageMultiplier = 1f;
    [SerializeField] protected DamageHandler damageHandler;

    public DamageHandler DamageHandler { get { return damageHandler; } set { damageHandler = value; } }
    protected void InitHandler()
    {
        if (damageHandler == null)
        {
            damageHandler = GetComponentInParent<DamageHandler>();

            if (damageHandler == null) Debug.LogWarning("HitBox cannot Find damage handler");
        }
    }

    private void Start()
    {
        InitHandler();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out DamageSource damageSource)) return;

        damageHandler.ApplyDamage(damageSource.DamageDealt * damageMultiplier);
    }
}

