using UnityEngine;
using System;
using System.Collections;
using TrialsOfOdin.Stats;
using TrialsOfOdin.State;


namespace TrialsOfOdin.Combat
{
    public class CombatSystemPro : MonoBehaviour
    {
        [SerializeField] private CombatProfileSO combatProfileSO;
        [SerializeField] private WeaponHolster weaponHolster;
        [SerializeField] private CharacterStats stats;
        public WeaponHolster WeaponHolster { get { return weaponHolster; } }
        public CharacterStats Stats { get { return stats; } }

        [SerializeField] private CombatInputHandler inputHandler = new();
        [HideInInspector] public bool DisableInputs;

        [SerializeField] PlayerAnimationEvents playerAnimationEvents;

        [SerializeField] private bool canChargeFirstAttack = true;

        private Coroutine chargeAttackCoroutine;

        public Weapon Weapon { get; private set; }
        private Shield shield;
        private AttackSO executedAttack;

        private AttackType queuedAttackType;
        private AttackType executedAttackType;

        private AttackAnimationData attackAnimationData = new AttackAnimationData();
        public event EventHandler<AttackAnimationData> OnExecuteAttackAnimation;

        public event Action OnExecuteAttack;

        private bool hasChargedAttacked;
        private bool hasSprintAttacked;
        private bool hasAerialAttacked;

        private int comboValue;

        public bool CanQueueCombo { get; private set; }
        private bool hasAnimFinished = true;

        private bool hasQueuedAttack;
        public bool isHoldingAttack { get; private set; }
        private bool exitComboUnlessRecover;

        public bool IsBlocking { get; set; }
        public bool IsAiming { get; set; }
        public bool IsSprinting { get; set; }
        public bool IsAerialing {  get; set; }
        public bool IsDodging { get; set; }

        private void OnEnable()
        {
            weaponHolster.OnNewActiveWeapon += WeaponHolster_OnNewActiveWeapon;
            weaponHolster.OnEquipShield += WeaponHolster_OnEquipShield;

            if (inputHandler.UseInputs)
            {
                inputHandler.Enable();
                inputHandler.OnAttackPerformed += ProcessAttackInput;
                inputHandler.OnAttackReleased += AttackInputReleased;
                inputHandler.OnAimingChanged += (bool isAiming) => IsAiming = isAiming;
            }

            playerAnimationEvents.OnComboAvalible += () => CanQueueCombo = true;
            playerAnimationEvents.OnComboUnAvalible += () => CanQueueCombo = false;
        }

        private void WeaponHolster_OnEquipShield()
        {
            shield.SetUser(stats.Character);
            this.shield = weaponHolster.shield;
        }

        private void WeaponHolster_OnNewActiveWeapon(Weapon weapon) => this.Weapon = weapon;

        private void Start()
        {
            PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;

            PlayerAnimator animator = localPlayerStateMachine.gameObject.GetComponent<PlayerAnimator>();

            animator.OnAnimStarted += () => hasAnimFinished = false;
            animator.OnAnimFinished += () =>  hasAnimFinished = true;
        }



        public void ProcessAttackInput(AttackType attackType)
        {
            if (DisableInputs) return;

            if (IsDodging) return;

            if (isHoldingAttack)
            {
                if (queuedAttackType != attackType) Debug.Log("Clicked other button while holding other"); 
                return;
            }

            if (IsAiming && Weapon.WeaponData.hasAimAttacks) { AimAttack(attackType); return; }

            if (hasAnimFinished) { FirstAttack(attackType); return; }

            if (CanRecover(attackType)) { RecoveryAttack(); return; }

            if (!CanQueueCombo)
            {
                ComboAttack(attackType);
                CheckForChargingAttack(attackType, true);
                return;
            }

            if (exitComboUnlessRecover) return;

            hasQueuedAttack = true;
            queuedAttackType = attackType;
        }

        private void FirstAttack(AttackType attackType)
        {
            comboValue = -1; //will get iterated to 0

            if (IsAerialing) { AerialAttack(attackType); return; }

            if (IsSprinting) { SprintAttack(attackType); return; }

            if (canChargeFirstAttack)
            {
                queuedAttackType = attackType;
                CheckForChargingAttack(attackType, false);
            }
            else
            {
                ComboAttack(attackType);
                CheckForChargingAttack(attackType, true);
            }
        }

        public void ChargedAttack(AttackType attackType)
        {
            hasChargedAttacked = false;
            ExecuteSpecialAttack(
                Weapon.LightChargedAttack,
                Weapon.HeavyChargedAttack,
                attackType,
                () => hasChargedAttacked = true
            );
        }

        public void SprintAttack(AttackType attackType)
        {
            hasSprintAttacked = false;
            ExecuteSpecialAttack(
                Weapon.LightSprintAttack,
                Weapon.HeavySprintAttack,
                attackType,
                () => hasSprintAttacked = true
            );
        }

        public void AerialAttack(AttackType attackType)
        {
            hasAerialAttacked = false;
            ExecuteSpecialAttack(
                Weapon.LightAerialAttack,
                Weapon.HeavyAerialAttack,
                attackType,
                () => hasAerialAttacked = true
            );
        }

        public void AimAttack(AttackType attackType)
        {
            AttackSO aimAttack = attackType == AttackType.Light ? Weapon.LightAimAttack : Weapon.HeavyAimAttack;
            TryExecuteAttack(aimAttack);
        }

        public void ExecuteSpecialAttack(AttackSO lightAttack, AttackSO heavyAttack, AttackType attackType, Action onSuccess)
        {
            AttackSO attack = attackType == AttackType.Light ? lightAttack : heavyAttack;

            if (!TryExecuteAttack(attack)) return;

            executedAttackType = attackType;
            onSuccess?.Invoke();

            if (executedAttack.canCombo) ComboHandling(attackType);
            else exitComboUnlessRecover = true;
        }

        private bool CanRecover(AttackType attackType)
        {
            return (((hasAerialAttacked && !IsAerialing) || hasSprintAttacked || hasChargedAttacked) 
                && executedAttack.hasRecoveryAttack && executedAttackType == attackType);
        }  

        public void RecoveryAttack()
        {
            if (!TryExecuteAttack(Weapon.RecoveryAttacks[executedAttack.recoveryAttack])) return;

            hasSprintAttacked = false;
            hasChargedAttacked = false;
            hasAerialAttacked = false;
            exitComboUnlessRecover = false;

            //for transition sake
            executedAttackType = AttackType.Heavy;
            ComboHandling(AttackType.Heavy);
        }

        public void FinisherAttack()
        {
            //checking attack is possible
            //perform
        }

        public void AttackInputReleased(AttackType attackType)
        {
            if(queuedAttackType == attackType) isHoldingAttack = false;
        }

        public bool TryExecuteAttack(AttackSO attack)
        {
            if (!stats.Stamina.TryUseStamina(attack.staminaCost)) return false;

            Weapon.currentAttack = attack;
            Weapon.SetDamageMultiplier(attack.damageMultiplier);

            hasAnimFinished = false;
            hasQueuedAttack = false;

            attackAnimationData.stateName = attack.animationState.ToString();
            attackAnimationData.animationSpeed = attack.animationSpeed;

            OnExecuteAttackAnimation?.Invoke(this, attackAnimationData);
            OnExecuteAttack?.Invoke();
            executedAttack = attack;

            if(attack.canCombo)
            {
                CanQueueCombo = true;
                StartCoroutine(CheckForQueuedComboAttack());
            }

            Debug.Log(attack.animationState);
            return true;
        }

        private IEnumerator CheckForQueuedComboAttack()
        {
            while(CanQueueCombo) yield return null;

            if (hasQueuedAttack)
            {
                if (queuedAttackType != executedAttackType) ComboHandling(queuedAttackType);
                ComboAttack(queuedAttackType);
            }
        }

        private void ComboHandling(AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.Light:
                    comboValue = executedAttack.lightComboTransition - 1;
                    break;

                case AttackType.Heavy:
                    comboValue = executedAttack.heavyComboTransition - 1;
                    break;
            }
        }

        private void CheckForChargingAttack(AttackType chargedAttackType, bool attackWhileCharging)
        {
            if(chargeAttackCoroutine != null) StopCoroutine(chargeAttackCoroutine);
            isHoldingAttack = true;

            Action onChargedRelease = attackWhileCharging ? () => { } : () => ComboAttack(chargedAttackType);
            chargeAttackCoroutine = StartCoroutine(ChargingAttack(chargedAttackType, onChargedRelease));
        }

        private IEnumerator ChargingAttack(AttackType chargedAttackType, Action onEarlyRelease)
        {
            float chargeTimer = (chargedAttackType == AttackType.Light) 
                ? Weapon.LightChargedAttack.chargingTimer 
                : Weapon.HeavyChargedAttack.chargingTimer;

            float timer = -chargeTimer / 2f;

            while (isHoldingAttack)
            {
                if (timer < chargeTimer)
                {
                    if (!CanQueueCombo)
                    {
                        timer += Time.deltaTime;
                        //if (queuedAttackType != chargedAttackType)
                            //finisher attack
                    }
                }
                else
                {
                    timer = -chargeTimer;
                    ChargedAttack(chargedAttackType);
                }

                yield return null;
            }
            if(!hasChargedAttacked) onEarlyRelease?.Invoke();
        }

        private void ComboAttack(AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.Light:
                    comboValue = (comboValue + 1) % Weapon.LightCombo.Count;
                    if (TryExecuteAttack(Weapon.LightCombo[comboValue])) executedAttackType = attackType;
                    break;
                case AttackType.Heavy: 
                    comboValue = (comboValue + 1) % Weapon.HeavyCombo.Count;
                    if (TryExecuteAttack(Weapon.HeavyCombo[comboValue])) executedAttackType = attackType;
                    break;
            }
        }
       
        private void LogWarning(object message) => Debug.LogWarning(message);
        private void Log(object message) => Debug.Log(message);
    }
    public enum AttackType
    {
        Heavy,
        Light
    }
}