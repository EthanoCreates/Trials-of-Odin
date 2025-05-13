using FIMSpace.FProceduralAnimation;
using UnityEngine;

public class HitBox : RA2BoneCollisionHandler
{
    [SerializeField] private float damageMultiplier = 1f;

    public void OnWeaponCollision(float damageAmount, Vector3 collisionPosition, int attackID)
    {
        //    //avoids multiple damage hits registering

        //    if(health.currentAttackID == attackID) return;
        //    health.TakeDamage(damageAmount * damageMultiplier, collisionPosition, attackID);

        //    ParentHandler.OnCollisionEnterEvent(this, collision);
    }


}
