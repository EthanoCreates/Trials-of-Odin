using FIMSpace.FProceduralAnimation;
using UnityEngine;

namespace TrialsOfOdin
{
    public class RagdollAnimatorDamageHandler : DamageHandler, IRagdollAnimator2Receiver
    {
        public void RagdollAnimator2_OnCollisionEnterEvent(RA2BoneCollisionHandler hitted, Collision mainCollision)
        {
            if (!mainCollision.gameObject.TryGetComponent(out DamageSource damageData)) return;

            //if (damageData.charactersHit.Contains(this)) return; //If hit already registered

            //rest of logic
        }
    }
}
