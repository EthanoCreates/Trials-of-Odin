using FIMSpace.FProceduralAnimation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class HealthForRagdolls : MonoBehaviour, IRagdollAnimator2Receiver
{
    public event EventHandler OnDeath;
    public event EventHandler <OnDamageEventArgs> OnDamage;

    public class OnDamageEventArgs
    {
        public float damageX;
        public float damageZ;
        public Vector3 suspectedPosition;
    }

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthBarEasing = 1;
    [SerializeField] private float deathImpactForce= 10;
    [SerializeField] private List<Material> dissolveMaterials;
    [SerializeField] private VisualEffect disintegrate;
    [SerializeField] private float dissolveRate = 0.0125f;
    public EnemyUI EnemyUI;
    [SerializeField] private Ragdoll ragdoll;
    [HideInInspector]
    public int currentAttackID;

    private Slider healthUI;
    private Slider healthEasingUI;

    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }

    // Start is called before the first frame update
    void Start()
    {
        if (dissolveMaterials != null)
        {
            //for(int x = 0; x < dissolveMaterials.Count; x++)
            //{
            //    dissolveMaterials[x] = new Material(dissolveMaterials[x]);
            //    dissolveMaterials[x].SetFloat("_DissolveAmount", 0);
            //}

            foreach (Material material in dissolveMaterials)
            {
                material.SetFloat("_DissolveAmount", 0);
            }
        }

        EnemyUI.gameObject.SetActive(true);
        currentHealth = maxHealth;
        healthUI = EnemyUI.HealthUI;
        healthEasingUI = EnemyUI.HealthBarEasingUI;
    }

    public void TakeDamage(float damageAmount, Vector3 collisionPosition, int attackID)
    {
        Vector3 directionToHitInWorld = Direction(transform.position, collisionPosition); // Parameter 1 = Target, Parameter 2 = Thing doing the hitting
        Vector3 forceDirection = transform.InverseTransformDirection(directionToHitInWorld);

        collisionPosition.y = transform.position.y;

        currentAttackID = attackID;
        currentHealth -= damageAmount;
        if(currentHealth <= 0)
        {
            Die(forceDirection);
        }
        else
        {
            OnDamage?.Invoke(this, new OnDamageEventArgs
            {
                damageX = forceDirection.x,
                damageZ = forceDirection.z,
                suspectedPosition = collisionPosition,
            });
        }
        DisplayHealth(currentHealth / maxHealth);
    }

    private void DisplayHealth(float newFillAmount)
    {
        if (newFillAmount < 0) newFillAmount = 0;
        healthUI.value = newFillAmount;
        StopCoroutine(nameof(HealthDisplayEasing));
        StartCoroutine(HealthDisplayEasing(newFillAmount));
    }

    private IEnumerator HealthDisplayEasing(float targetFillAmount)
    {
        while (Mathf.Abs(healthEasingUI.value - targetFillAmount) > 0.01f)
        {
            healthEasingUI.value = Mathf.Lerp(healthEasingUI.value, targetFillAmount, Time.deltaTime * healthBarEasing);
            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set precisely
        healthEasingUI.value = targetFillAmount;
        if (targetFillAmount <= 0) {

            StartCoroutine(Dissolve());
            if (EnemyUI != null)
            {
                Destroy(EnemyUI.gameObject);
            }
        }
    }

    IEnumerator Dissolve()
    {
        if (dissolveMaterials != null)
        {
            if (disintegrate != null) disintegrate.Play();
            float counter = 0;

            foreach (Material dissolveMaterial in dissolveMaterials)
            {
                while (dissolveMaterial.GetFloat("_DissolveAmount") < 1)
                {
                    counter += dissolveRate;
                    dissolveMaterial.SetFloat("_DissolveAmount", counter);
                    yield return new WaitForSeconds(dissolveRate * 2);
                }

                dissolveMaterial.SetFloat("_DissolveAmount", 0);
                Destroy(gameObject);
            }
        }
    }

    private Vector3 Direction(Vector3 fromPosition, Vector3 toPosition) => (toPosition - fromPosition).normalized;

    private void Die(Vector3 forceDirection)
    {
        ragdoll.ActivateRagdoll();
        ragdoll.ApplyForce(forceDirection * 30);
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void RagdollAnimator2_OnCollisionEnterEvent(RA2BoneCollisionHandler hitted, Collision mainCollision)
    {
        throw new NotImplementedException();
    }
}
