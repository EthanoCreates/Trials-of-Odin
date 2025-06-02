using Sirenix.OdinInspector;
using UnityEngine;

namespace TrialsOfOdin
{
    public enum RagdollBoneGroups { Core, Leg, Arm, Head, Neck }
    public class RagdollDamageHandler : DamageHandler
    {
        [Button]
        private void GenerateHitBoxes()
        {
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>(true);

            foreach (Rigidbody rb in rigidbodies)
            {
                RagdollHitBox hitBox = rb.GetComponent<RagdollHitBox>();

                if (hitBox == null)
                    hitBox = rb.gameObject.AddComponent<RagdollHitBox>();

                string nameOfHitBox = rb.gameObject.name;

                hitBox.DamageHandler = this;

                if (nameOfHitBox.Contains("Leg") || nameOfHitBox.Contains("Foot")) hitBox.boneGroup = RagdollBoneGroups.Leg;
                else if (nameOfHitBox.Contains("Arm") || nameOfHitBox.Contains("Hand")) hitBox.boneGroup = RagdollBoneGroups.Arm;
                else if (nameOfHitBox.Contains("Head")) hitBox.boneGroup = RagdollBoneGroups.Head;
                else if (nameOfHitBox.Contains("Neck")) hitBox.boneGroup = RagdollBoneGroups.Neck;
                else hitBox.boneGroup = RagdollBoneGroups.Core;
            }
        }
    }
}
