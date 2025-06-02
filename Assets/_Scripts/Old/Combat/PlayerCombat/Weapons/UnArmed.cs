using System.Collections.Generic;
using TrialsOfOdin.State;
using UnityEngine;

namespace TrialsOfOdin.Combat
{
    public class UnArmed : Weapon
    {
        [SerializeField] private float handColliderSizeMultiplier = 1;
        [SerializeField] private float footColliderSizeMultiplier = 1;
        private Animator animator;
        private readonly List<UnArmedColliders> unarmedColliders = new List<UnArmedColliders>();
        public const int WEAPON_LAYER = 11;

        float playerHandWidth;
        float playerFootWidth;

        private void Start()
        {
            Wielder = PlayerStateMachine.LocalInstance.transform;

            float playerWidth = PlayerStateMachine.LocalInstance.CharacterController.radius;
            playerHandWidth = playerWidth / 2f * handColliderSizeMultiplier;
            playerFootWidth = playerWidth / 1.5f * footColliderSizeMultiplier;
            PrepareDamageCollider();
        }

        private void PrepareDamageCollider()
        {
            animator = PlayerStateMachine.LocalInstance.transform.GetComponentInChildren<Animator>();
            CreateDamageCollider(HumanBodyBones.RightHand, playerHandWidth, new Vector3(0f, .1f, 0f));
            CreateDamageCollider(HumanBodyBones.LeftHand, playerHandWidth, new Vector3(0f, .1f, 0f));
            CreateDamageCollider(HumanBodyBones.RightFoot, playerFootWidth, new Vector3(0f, .1f, -.1f));
            CreateDamageCollider(HumanBodyBones.LeftFoot, playerFootWidth, new Vector3(0f, .1f, -.1f));
        }

        private void CreateDamageCollider(HumanBodyBones colliderBonePosition, float colliderWidth, Vector3 offset)
        {
            GameObject boneObject = animator.GetBoneTransform(colliderBonePosition).gameObject;
            BoxCollider damageCollider = boneObject.AddComponent<BoxCollider>();
            Rigidbody rb = boneObject.AddComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezeAll;

            damageCollider.gameObject.layer = WEAPON_LAYER;
            damageCollider.size = new(colliderWidth, colliderWidth, colliderWidth);

            damageCollider.center = offset;

            damageCollider.isTrigger = true;

            DamageColliders.Add(damageCollider);
            unarmedColliders.Add(boneObject.AddComponent<UnArmedColliders>());

            unarmedColliders[unarmedColliders.Count - 1].OnEnemyCollision += UnArmed_OnEnemyCollision;
            unarmedColliders[unarmedColliders.Count - 1].OnShieldCollision += UnArmed_OnShieldCollision;
            DisableDamageColliders(DamageColliders.Count - 1);
        }

        private void OnDestroy()
        {
            foreach (BoxCollider unarmedCollider in DamageColliders)
            {
                UnArmedColliders unArmedCollidersScript = unarmedCollider.gameObject.GetComponent<UnArmedColliders>();

                unArmedCollidersScript.OnEnemyCollision -= UnArmed_OnEnemyCollision;
                unArmedCollidersScript.OnShieldCollision -= UnArmed_OnShieldCollision;
            }
        }

        private void UnArmed_OnShieldCollision(object sender, UnArmedColliders.CollisionEventArgs e)
        {
            HandleShieldCollision(e.collision);
        }

        private void UnArmed_OnEnemyCollision(object sender, UnArmedColliders.CollisionEventArgs e)
        {
            HandleEnemyCollision(e.collision);
        }

        public void AttackIDIncrement()
        {
            AttackID++;
        }

        public override void WeaponPickUp()
        {
            //overriding to ensure base is not applied
            //This method should not do anything as weapon is unarmed
        }
    }
}