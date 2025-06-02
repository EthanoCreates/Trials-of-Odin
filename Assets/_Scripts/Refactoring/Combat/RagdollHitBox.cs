using UnityEngine;

namespace TrialsOfOdin
{
    public class RagdollHitBox : HitBox
    {
        public RagdollBoneGroups boneGroup;

        public override void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out DamageSource damageSource)) return;

            if (damageSource.charactersHit.Contains(damageHandler.GetCharacter) || 
                damageSource.blockedCharactersHit.Contains(damageHandler.GetCharacter)) 
                return;

            damageSource.charactersHit.Add(damageHandler.GetCharacter);

            damageHandler.ApplyDamage(damageSource.DamageDealt * damageMultiplier);
        }
    }
}
