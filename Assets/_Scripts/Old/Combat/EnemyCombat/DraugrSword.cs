using TrialsOfOdin.State;
using UnityEngine;

public class DraugrSword : EnemyWeapon
{
    [SerializeField] private BoxCollider damageCollider;
    [SerializeField] private HealthForRagdolls enemyHealth;
    private float damageMultiplier = 1;
    int attackID = 0;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //other.gameObject.GetComponent<PlayerStateMachine>().Utilities.TakeDamage(EnemyWeaponData.damage * damageMultiplier, this.transform.position, attackID, enemyHealth);
        }
    }

    public override void DisableDamageColliders()
    {
        damageCollider.enabled = false;
    }

    public override void EnableDamageColliders()
    {

        damageCollider.enabled = true;
        attackID++;
    }

    public override void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }
}
