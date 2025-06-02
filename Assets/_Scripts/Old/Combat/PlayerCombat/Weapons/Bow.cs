using Sirenix.OdinInspector;
using System.Collections;
using TrialsOfOdin.State;
using UnityEngine;

namespace TrialsOfOdin.Combat
{
    public class Bow : Weapon, IShootable, IAimAttacks
    {
        [FoldoutGroup("Bow Reference")]
        [SerializeField] private SkinnedMeshRenderer bowSkinnedMeshRenderer;
        [FoldoutGroup("Bow Reference")]
        [SerializeField] private GameObject Quiver;
        [FoldoutGroup("Bow Reference")]
        [SerializeField] private GameObject Arrow;

        [SerializeField] private float bowShootingSpeed = 30f;
        [SerializeField] private WeaponHolster.ERetargeterType lHandTargeterType; //The hand used to determine the bow bending

        private UnArmed unArmed; //The bow also has unarmed combat for when not aiming
        private Transform arrowHolder;
        private Transform bowHandRetargeter; //The object used to determine bow bending
        private Transform currentHeldArrow;
        private GameObject currentQuiver;
        private bool loaded;
        private bool reloading;
        private Coroutine aimCoroutine;
        private bool aiming;
        private PlayerStateMachine playerState; //caching class here for reload and state checking
        private Animator bowString;
        public IAimAttacks.EAimAttackTypes HeavyAimAttackType => IAimAttacks.EAimAttackTypes.Shoot; //Heavy aim attacks are of the shooting type

        public IAimAttacks.EAimAttackTypes LightAimAttackType => IAimAttacks.EAimAttackTypes.Shoot; //Light aim attacks are of the shooting type

        private void Start()
        {
            bowString = GetComponent<Animator>();
        }

        public void HeavyAimAttack()
        {
            Shoot();
        }

        public void LightAimAttack()
        {
            Shoot();
        }

        /// <summary>
        /// These are unarmed collider for bow non shooting combat
        /// </summary>
        /// <param name="collider">This will be hands or feet</param>
        public override void EnableDamageColliders(int collider)
        {
            DamageColliders[collider].enabled = true;
            unArmed.AttackIDIncrement();
        }

        /// <summary>
        /// Attempting a reload, if the player is in a viable state
        /// like Idle or walk.
        /// </summary>
        public void Reload()
        {
            StartCoroutine(TryReload());
        }

        /// <summary>
        /// This is fired during the reload animation when the
        /// players hand is next to the quiver, ready to pick up the arrow
        /// We also parent the arrow to the hand during instantiation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerState_OnReloadWeapon()
        {
            loaded = true;
            SpawnArrow();
        }


        /// <summary>
        /// This is an automatic reload when the player does not currently
        /// have an arrow, to make sure reloading and other states like 
        /// dodge dont play at the same time we have a state check
        /// </summary>
        private IEnumerator TryReload()
        {
            // Initialize `loaded` to false before starting the reload process
            loaded = false;
            reloading = false;

            while (!loaded)
            {
                ECharacterState currentState = playerState.currentState.StateKey;
                ECharacterState subState = playerState.currentState.CurrentSubState.StateKey;

                // Check if the player is in a valid state to reload
                if (currentState == ECharacterState.Grounded &&
                    (subState == ECharacterState.Idle ||
                     subState == ECharacterState.Walk ||
                     subState == ECharacterState.Run ||
                     subState == ECharacterState.Aim))
                {
                    if (!reloading)
                    {
                        //leaving aim anim to allow for reloading
                        playerState.AnimationRequestor.AnimateAimExit();
                        // Trigger reload animation if not already reloading
                        playerState.AnimationRequestor.AnimateReload();

                        reloading = true;
                    }
                }
                else
                {
                    // If the state is not valid and we are reloading, cancel the reload
                    if (reloading)
                    {
                        playerState.AnimationRequestor.AnimateReloadCancel();
                        reloading = false;
                    }
                }

                // Wait for the next frame before checking again
                yield return null;
            }

            if (playerState.CombatSystem.IsAiming) playerState.AnimationRequestor.AnimateAim();
        }

        /// <summary>
        /// Instantiating arrow and parenting it to hand
        /// </summary>
        private void SpawnArrow()
        {
            currentHeldArrow = Instantiate(Arrow, arrowHolder).transform;
        }

        /// <summary>
        /// Getting arrow script and shooting it with bow shooting speed + arrow shooting speed
        /// Also subscribing to some events for collision and deletion for that arrow.
        /// </summary>
        public void Shoot()
        {
            AttackID++;

            loaded = false;

            Arrow arrowScript = currentHeldArrow.GetComponentInChildren<Arrow>();

            arrowScript.Shoot(bowShootingSpeed);

            arrowScript.OnCollision += ArrowScript_OnCollision;
            arrowScript.OnDelete += ArrowScript_OnDelete;

            currentHeldArrow = null;

            bowString.Play("Fire");

            Invoke(nameof(Reload), 0.1f);
        }

        /// <summary>
        /// using base weapon collision detection 
        /// checking if arrow collision was block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArrowScript_OnCollision(object sender, Arrow.CollisionEventArgs e)
        {
            if (AttackID == BlockedAttackID) return;

            OnCollisionEnter(e.collision);
        }

        /// <summary>
        /// These is an event called from the arrow that wants to delete
        /// </summary>
        /// <param name="sender">Arrow script</param>
        /// <param name="e">Arrow object</param>
        private void ArrowScript_OnDelete(object sender, Arrow.OnDeleteEventArgs e)
        {
            e.arrowToUnsubscribeFrom.OnCollision -= ArrowScript_OnCollision;
            e.arrowToUnsubscribeFrom.OnDelete -= ArrowScript_OnDelete;
        }

        public override void WeaponToHand()
        {
            base.WeaponToHand();
            playerState.PlayerAnimationEvents.OnReloadWeapon += PlayerState_OnReloadWeapon;
            Reload();
        }
        public override void WeaponToHolster()
        {
            base.WeaponToHolster();
            if (currentHeldArrow != null) Destroy(currentHeldArrow.gameObject);
            playerState.PlayerAnimationEvents.OnReloadWeapon -= PlayerState_OnReloadWeapon;
        }

        public override void WeaponPickUp()
        {
            playerState = PlayerStateMachine.LocalInstance;

            base.WeaponPickUp();

            currentQuiver = Instantiate(Quiver, WeaponHolster.GetHolster(WeaponHolster.EHolsterType.QuiverHolster));

            arrowHolder = WeaponHolster.GetHolder(WeaponHolster.EHolderType.RightHandHolder);
            bowHandRetargeter = WeaponHolster.GetPropRetargeter(lHandTargeterType);

            playerState.AnimationRequestor.OnAim += AnimationRequestor_OnAim;
            playerState.AnimationRequestor.OnAimExit += AnimationRequestor_OnAimExit;

            AssignDamageColliders();

            StartCoroutine(LerpBowWeight());
        }

        private void AssignDamageColliders()
        {
            unArmed = WeaponHolster.unArmedWeapon.GetComponent<UnArmed>();
            DamageColliders = unArmed.DamageColliders;
        }

        /// <summary>
        /// Setting aiming to true and fixing arrow position in hand
        /// </summary>

        private void AnimationRequestor_OnAim()
        {
            aiming = true;
            currentHeldArrow.localPosition = new Vector3(-.065f, .433f, .07f);
            currentHeldArrow.localEulerAngles = new Vector3(-80f, -69f, 69f);
            Invoke(nameof(DrawAnimation), 1f);
        }

        /// <summary>
        /// Setting aiming to false and fixing arrow position in hand
        /// </summary>

        private void AnimationRequestor_OnAimExit()
        {
            aiming = false;
            if (currentHeldArrow == null) return;
            currentHeldArrow.localPosition = new Vector3(-.06f, .068f, .06f);
            currentHeldArrow.localEulerAngles = new Vector3(-50f, -64f, -104f);
            CancelInvoke(nameof(DrawAnimation));
            bowString.Play("Fire");
        }

        private void DrawAnimation() => bowString.Play("Draw");

        /// <summary>
        /// This makes the bow bend when the arrow is pulled back
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpBowWeight()
        {
            while (true)
            {
                float bowWeight = Mathf.InverseLerp(0, -1f, bowHandRetargeter.localPosition.z);
                //bowSkinnedMeshRenderer.SetBlendShapeWeight(0, bowWeight * 100);
                yield return null;
            }
        }

        /// <summary>
        /// Cleaning up bow data and destroying bow objects
        /// </summary>
        public override void DropWeapon()
        {
            playerState.PlayerAnimationEvents.OnReloadWeapon -= PlayerState_OnReloadWeapon;
            Destroy(currentQuiver);
            if (currentHeldArrow != null) Destroy(currentHeldArrow.gameObject);
            Destroy(gameObject);
        }

        public int GetAmmoAmount()
        {
            //infinite, may change later
            return -1;
        }

        public bool IsReloaded()
        {
            return loaded;
        }
    }
}