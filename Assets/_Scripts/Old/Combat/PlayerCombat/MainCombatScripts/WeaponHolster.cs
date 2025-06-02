using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TrialsOfOdin.State;
using UnityEngine;

namespace TrialsOfOdin.Combat
{
    public class WeaponHolster : MonoBehaviour
    {
        [ReadOnly] public Weapon ActiveWeapon;
        [FoldoutGroup("Weapons")]
        [SerializeField] private Weapon primaryWeapon;
        [FoldoutGroup("Weapons")]
        [SerializeField] private Weapon secondaryWeapon;
        [FoldoutGroup("Weapons")]
        public Weapon unArmedWeapon;
        [FoldoutGroup("Shield")]
        public Shield shield;
        [FoldoutGroup("Shield")]
        [SerializeField] private Transform shieldHolster;
        [FoldoutGroup("Shield")]
        [SerializeField] private Transform shieldHolder;
        [SerializeField] private List<Transform> holster;
        [FoldoutGroup("Weapon Holders")]
        [SerializeField] private Transform rWeaponHolder;
        [FoldoutGroup("Weapon Holders")]
        [SerializeField] private Transform lWeaponHolder;
        [FoldoutGroup("Retargeters")]
        [SerializeField] private Transform rHandProp;
        [FoldoutGroup("Retargeters")]
        [SerializeField] private Transform lHandProp;

        private Dictionary<EHolsterType, int> holsterSlots;
        private Dictionary<EHolderType, Transform> holderSlots;
        private Dictionary<ERetargeterType, Transform> propSlots;

        public event Action<Weapon> OnNewActiveWeapon;

        //activated on input
        public event Action<WeaponSlotType> OnEquip;
        public event Action<Weapon> OnUnEquip;
        public event Action OnEquipShield;
        public event Action OnUnEquipShield;

        public bool HasShield { get { return shield != null; } }
        public AnimatorOverrideController shieldOverrideController { get { return shield.overrideController; } }
        public AnimatorOverrideController combatMovementOverride;

        private WeaponSlotType activeWeaponSlotType;
        public bool HasShieldEquipped { get; private set; } = false;


        private void Awake()
        {
            holsterSlots = new Dictionary<EHolsterType, int>
        {
            { EHolsterType.HipHolster, 0 },
            { EHolsterType.BackHolster, 1 },
            { EHolsterType.BowHolster, 2},
            { EHolsterType.QuiverHolster, 3},
        };

            holderSlots = new Dictionary<EHolderType, Transform>
        {
            { EHolderType.RightHandHolder, rWeaponHolder},
            { EHolderType.LeftHandHolder, lWeaponHolder},
        };
            propSlots = new Dictionary<ERetargeterType, Transform>
        {
            {ERetargeterType.rHandProp, rHandProp},
            {ERetargeterType.lHandProp, lHandProp},
        };
        }

        void Start()
        {
            //Setting up initial active weapon 
            if (primaryWeapon != null) primaryWeapon.WeaponPickUp();
            else NewActiveWeapon(primaryWeapon != null ? primaryWeapon : unArmedWeapon);

            //on inputs
            GameInput.Instance.OnEquipPrimary += () => SwitchWeapons(true);
            GameInput.Instance.OnEquipSecondary += () => SwitchWeapons(false);
            GameInput.Instance.OnEquipShield += Instance_OnEquipShield;

            PlayerAnimationEvents playerAnimationEvents = PlayerStateMachine.LocalInstance.PlayerAnimationEvents;

            playerAnimationEvents.OnEquip += AnimationEvent_OnEquip;
            playerAnimationEvents.OnUnEquip += AnimationEvent_OnUnEquip;
            playerAnimationEvents.OnReloadWeapon += PlayerState_OnEquipShield;
            playerAnimationEvents.OnUnEquipShield += PlayerState_OnUnEquipShield;
            playerAnimationEvents.OnDamageAvalible += PlayerAnimationEvents_OnDamageAvalible;
            playerAnimationEvents.OnDamageUnAvalible += PlayerAnimationEvents_OnDamageUnAvalible;
        }

        private void PlayerAnimationEvents_OnDamageAvalible(object sender, PlayerAnimationEvents.TriggeredDamageCollider e)
        {
            ActiveWeapon.EnableDamageColliders(e.collider);
            ActiveWeapon.EnableWeaponVFX();
        }
        private void PlayerAnimationEvents_OnDamageUnAvalible(object sender, PlayerAnimationEvents.TriggeredDamageCollider e)
        {
            ActiveWeapon.DisableDamageColliders(e.collider);
            ActiveWeapon.DisableWeaponVFX();
        }

        private void PlayerState_OnUnEquipShield()
        {
            if (shield == null) return;
            HasShieldEquipped = false;
            shield.transform.SetParent(shieldHolster);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localEulerAngles = Vector3.zero;

            //Just allowing animator override to refresh without shield
            OnNewActiveWeapon?.Invoke(ActiveWeapon);
        }

        private void PlayerState_OnEquipShield()
        {
            if (shield == null) return;
            EquipShield();
        }

        public void EquipShield()
        {
            HasShieldEquipped = true;
            shield.transform.SetParent(shieldHolder);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localEulerAngles = Vector3.zero;


            //Just allowing animator override to refresh with shield
            OnNewActiveWeapon?.Invoke(ActiveWeapon);
        }

        private void Instance_OnEquipShield()
        {
            if (shield == null) return;

            if (HasShieldEquipped)
            {
                OnUnEquipShield?.Invoke();
            }
            else
            {
                OnEquipShield?.Invoke();
            }
        }

        public Transform GetHolster(EHolsterType holsterType)
        {
            return holster[holsterSlots[holsterType]];
        }

        public Transform GetHolder(EHolderType holderType)
        {
            return holderSlots[holderType];
        }

        public Transform GetPropRetargeter(ERetargeterType eRetargeterType)
        {
            return propSlots[eRetargeterType];
        }

        private void AnimationEvent_OnUnEquip()
        {
            //returning weapon to holster
            ActiveWeapon.WeaponToHolster();
            //setting weapon to unarmed
            NewActiveWeapon(unArmedWeapon);
        }

        private void AnimationEvent_OnEquip()
        {
            if (activeWeaponSlotType == WeaponSlotType.primary && secondaryWeapon != null 
                && !IsThrown(secondaryWeapon)) secondaryWeapon.WeaponToHolster();
            if (activeWeaponSlotType == WeaponSlotType.secondary && primaryWeapon != null 
                && !IsThrown(primaryWeapon)) primaryWeapon.WeaponToHolster();

            //setting weapon to hand
            ActiveWeapon.WeaponToHand();
        }

        public void NewActiveWeapon(Weapon newActiveWeapon)
        {
            ActiveWeapon = newActiveWeapon;
            OnNewActiveWeapon?.Invoke(ActiveWeapon);
        }


        public void SwitchWeapons(bool switchToPrimary)
        {
            //switch toggle like system
            if (switchToPrimary)
            {
                if (primaryWeapon == null) return;

                if (ActiveWeapon == primaryWeapon)
                {
                    UnEquip();
                }
                else
                {
                    Equip(primaryWeapon);
                }
            }
            else
            {
                if (secondaryWeapon == null) return;

                if (ActiveWeapon == secondaryWeapon)
                {
                    UnEquip();
                }
                else
                {
                    Equip(secondaryWeapon);
                }
            }
        }

        public void UnEquip()
        {
            OnUnEquip?.Invoke(ActiveWeapon);
        }

        public void Equip(Weapon weapon)
        {
            if (IsThrown(weapon))
            {
                weapon.GetComponent<IThrowable>().Recall();
                return;
            }

            NewActiveWeapon(weapon);

            activeWeaponSlotType = WeaponSlotType.secondary;

            if (weapon == primaryWeapon) activeWeaponSlotType = WeaponSlotType.primary;

            OnEquip?.Invoke(activeWeaponSlotType);
        }

        private bool IsThrown(Weapon weapon)
        {
            IThrowable throwData = weapon.GetComponent<IThrowable>();
            if (throwData != null)
            {
                if (throwData.Released) return true;
            }
            return false;
        }


        public void FillHolsterSlot(Weapon weapon)
        {
            if (DropWeaponIfFull(weapon)) return;

            if (primaryWeapon == null && secondaryWeapon != weapon) primaryWeapon = weapon;
            else if (secondaryWeapon == null && primaryWeapon != weapon) secondaryWeapon = weapon;

            if (ActiveWeapon == unArmedWeapon)
            {
                weapon.WeaponToHand();
                NewActiveWeapon(weapon);
            }
            else
            {
                weapon.WeaponToHolster();
            }
        }
        public void ReleaseWeaponFromHolster()
        {
            if (ActiveWeapon == primaryWeapon) primaryWeapon = null;
            if (ActiveWeapon == secondaryWeapon) secondaryWeapon = null;
            NewActiveWeapon(unArmedWeapon);
        }

        private bool DropWeaponIfFull(Weapon weapon)
        {
            if (primaryWeapon != null && secondaryWeapon != null)
            {
                if (ActiveWeapon == unArmedWeapon)
                {
                    primaryWeapon.DropWeapon();
                    primaryWeapon = weapon;
                    weapon.WeaponToHolster();
                    return true;
                }

                if (ActiveWeapon == primaryWeapon) primaryWeapon = weapon;
                else secondaryWeapon = weapon;

                ActiveWeapon.DropWeapon();
                weapon.WeaponToHand();
                NewActiveWeapon(weapon);

                return true;
            }
            return false;
        }

        public enum EHolsterType
        {
            HipHolster,
            BackHolster,
            BowHolster,
            QuiverHolster,
        }

        public enum EHolderType
        {
            RightHandHolder,
            LeftHandHolder,
        }

        public enum ERetargeterType
        {
            rHandProp,
            lHandProp,
        }

        public enum WeaponSlotType
        {
            primary,
            secondary,
        }
    }
}