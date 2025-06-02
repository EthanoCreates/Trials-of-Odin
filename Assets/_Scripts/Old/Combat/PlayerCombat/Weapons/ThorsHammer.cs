using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace TrialsOfOdin.Combat
{
    public class ThorsHammer : Weapon, IThrowable, IAimAttacks
    {
        [FoldoutGroup("References")]
        [SerializeField] private BoxCollider airbourneDamageCollider;
        [FoldoutGroup("References")]
        [SerializeField] private Rigidbody rigidBody;
        [FoldoutGroup("References")]
        [SerializeField] private Transform hammerHolder;

        [FoldoutGroup("Settings")]
        [SerializeField] private float hammerSpeed = 50f;
        [FoldoutGroup("Settings")]
        [SerializeField] private float returnTime = 1.0f;
        [FoldoutGroup("Settings")]
        [SerializeField] private float maxReturnTime = 1.5f;
        [FoldoutGroup("Settings")]
        [SerializeField] private float spinSpeed = 360f;
        [FoldoutGroup("Settings")]
        [SerializeField] private LayerMask enemyLayerMask;

        public bool Recallable { get; } = true;
        public bool Released { get; private set; } = false;

        public IAimAttacks.EAimAttackTypes HeavyAimAttackType { get; } = IAimAttacks.EAimAttackTypes.Throw;
        public IAimAttacks.EAimAttackTypes LightAimAttackType { get; } = IAimAttacks.EAimAttackTypes.Throw;

        private Vector3 hitPosition;
        private Vector3 targetPoint;
        private Transform hammerArc;
        private float elapsedTime = 0f;

        public override void HandleShieldCollision(Collision collision)
        {
            if (Released)
            {
                rigidBody.linearVelocity = Vector3.zero;
                rigidBody.AddForce(collision.contacts[0].normal * 5, ForceMode.Impulse);
            }

            BlockedAttackID = AttackID;
            collision.gameObject.GetComponent<EnemyShield>().ShieldBlocked();
        }

        public override void HandleEnemyCollision(Collision collision)
        {
            hitPosition = Released ? transform.position - transform.up * 4 : Wielder.position;
            // collision.gameObject.GetComponent<HitBox>().OnWeaponCollision(WeaponData.damage * damageMultiplier, hitPosition, AttackID);
        }

        public void Throw(bool isHeavyAimAttack)
        {
            Released = true;

            EnableAirborneDamage();

            PerformAimAttack();
            if (!isHeavyAimAttack) StartCoroutine(SpinHammer());

            WeaponHolster.NewActiveWeapon(WeaponHolster.unArmedWeapon);
        }

        public void Recall()
        {
            WeaponToHand();
        }

        private void EnableAirborneDamage()
        {
            airbourneDamageCollider.enabled = true;
            foreach (BoxCollider damageCollider in DamageColliders)
            {
                damageCollider.excludeLayers = enemyLayerMask;
                damageCollider.enabled = true;
            }

            rigidBody.useGravity = true;
            AttackID++;
            SetDamageMultiplier(1.5f);
        }

        private void DisableAirBoureDamage()
        {
            foreach (BoxCollider damageCollider in DamageColliders)
            {
                damageCollider.excludeLayers = 0;
                damageCollider.enabled = false;
            }

            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            airbourneDamageCollider.enabled = false;
        }

        private void ReturnHammerToHolster()
        {
            hammerHolder.parent = Holster;
            returnTime = 0.02f;
            StartCoroutine(ReturnHammer(Holster));
            rigidBody.useGravity = false;
        }

        private IEnumerator ArcHammerToHand()
        {
            float distance;
            if (Released)
            {
                distance = Vector3.Distance(transform.position, hammerArc.position);
                returnTime = Mathf.Min(maxReturnTime, distance / hammerSpeed);

                yield return MoveHammerToPosition(hammerArc.position, returnTime * 0.9f);
            }

            distance = Vector3.Distance(hammerArc.position, Hand.position);
            returnTime = Mathf.Min(maxReturnTime, distance / hammerSpeed);

            yield return MoveHammerToPosition(Hand.position, returnTime * 0.1f);

            CompleteHammerReturn(Hand, new Vector3(0, 0, 90));
        }

        private IEnumerator ReturnHammer(Transform targetHolder)
        {
            float distance = Vector3.Distance(transform.position, targetHolder.position);
            returnTime = Mathf.Min(maxReturnTime, distance / hammerSpeed);

            yield return MoveHammerToPosition(targetHolder.position, returnTime);
            CompleteHammerReturn(targetHolder, targetHolder == Hand ? new Vector3(0, 0, 90) : Vector3.zero);
        }

        private IEnumerator MoveHammerToPosition(Vector3 endPosition, float duration)
        {
            Vector3 startPosition = transform.position;
            elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
                yield return null;
            }
        }

        private void CompleteHammerReturn(Transform targetHolder, Vector3 localEulerAngles)
        {
            hammerHolder.SetParent(targetHolder);
            hammerHolder.localPosition = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            hammerHolder.localEulerAngles = new(0f, 0f, 90f);

            DisableAirBoureDamage();

            if (Released)
            {
                WeaponHolster.FillHolsterSlot(this);
                Released = false;
            }
        }

        public void HeavyAimAttack()
        {
            Throw(true);
        }

        public void LightAimAttack()
        {
            Throw(false);
        }

        private void PerformAimAttack()
        {
            rigidBody.constraints = RigidbodyConstraints.None;
            foreach (BoxCollider damageCollider in DamageColliders)
            {
                damageCollider.enabled = true;
            }

            hammerHolder.SetParent(null);

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000);

            Vector3 directionToTarget = (targetPoint - hammerHolder.position).normalized;
            hammerHolder.rotation = Quaternion.LookRotation(directionToTarget);
            transform.localEulerAngles = new Vector3(90, 0, 0);

            rigidBody.linearVelocity = directionToTarget * hammerSpeed;
        }

        private IEnumerator SpinHammer()
        {
            while (Released)
            {
                transform.Rotate(spinSpeed * Time.deltaTime * Vector3.forward, Space.Self);
                yield return null;
            }
        }

        public override void WeaponPickUp()
        {
            SetUpHelper();

            CreateArc();

            FillHolsterslot();
        }

        private void CreateArc()
        {
            GameObject arcHelper = new GameObject();
            arcHelper.transform.parent = Holster.transform.parent.parent.parent;
            arcHelper.transform.localPosition = new Vector3(4.68f, 1.04f, 5.21f);
            arcHelper.name = "HammerArc";
            hammerArc = arcHelper.transform;
        }

        public int GetAmmoAmount()
        {
            throw new System.NotImplementedException();
        }

        public bool IsReloaded()
        {
            throw new System.NotImplementedException();
        }
    }
}