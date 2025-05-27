using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;


namespace TrialsOfOdin.Combat
{
    public class CombatSystemPro : MonoBehaviour
    {
        [SerializeField] private CombatProfileSO combatProfileSO;
        [SerializeField] private WeaponHolster weaponHolster;

        [SerializeField, HorizontalGroup("LightInput", DisableAutomaticLabelWidth = true)] 
        private bool useInputForLightAttack;
        [SerializeField, ShowIf(nameof(useInputForLightAttack)), HorizontalGroup("LightInput"), HideLabel] 
        private InputActionReference lightAttack;
        [SerializeField, HorizontalGroup("HeavyInput", DisableAutomaticLabelWidth = true)] 
        private bool useInputForHeavyAttack;
        [SerializeField, ShowIf(nameof(useInputForHeavyAttack)), HorizontalGroup("HeavyInput"), HideLabel] 
        private InputActionReference heavyAttack;
        [SerializeField] PlayerAnimationEvents playerAnimationEvents;

        [SerializeField] private bool canChargeFirstAttack = true;
        [SerializeField] private CombatAttackState combatAttackState;

        private Coroutine chargeAttackCoroutine;

        private Weapon weapon;
        private AttackSO executedAttack;

        private AttackType queuedAttackType;
        private AttackType executedAttackType;

        private AttackAnimationData attackAnimationData = new AttackAnimationData();
        public event EventHandler<AttackAnimationData> OnExecuteAttack;

        private bool hasChargedAttacked;
        private bool hasSprintAttacked;
        private bool hasAerialAttacked;

        private float lightChargedTimer;
        private float heavyChargedTimer;

        private int lightComboCount;
        private int heavyComboCount;

        private int comboValue;

        private bool canQueueCombo;
        private bool hasAnimFinished = true;

        private bool hasQueuedAttack;
        private bool isHoldingAttack;
        private bool exitComboUnlessRecover;

        private void OnEnable()
        {
            weaponHolster.OnNewActiveWeapon += WeaponHolster_OnNewActiveWeapon;

            if (useInputForLightAttack)
                if (lightAttack != null)
                {
                    InputAction lightAttackInput = lightAttack.ToInputAction();
                    lightAttackInput.started += (InputAction.CallbackContext obj) => AttackInputPerformed(AttackType.Light);
                    lightAttackInput.canceled += (InputAction.CallbackContext obj) => AttackInputReleased(AttackType.Light);
                }
                else LogWarning("Light attack input is null");

            if (useInputForHeavyAttack)
                if (heavyAttack != null)
                {
                    InputAction heavyAttackInput = heavyAttack.ToInputAction();
                    heavyAttackInput.started += (InputAction.CallbackContext obj) => AttackInputPerformed(AttackType.Heavy);
                    heavyAttackInput.canceled += (InputAction.CallbackContext obj) => AttackInputReleased(AttackType.Heavy);
                }
                else LogWarning("Heavy attack input is null");

            //playerAnimationEvents.OnComboAvalible += (object sender, EventArgs e) => canQueueCombo = true;
            playerAnimationEvents.OnComboUnAvalible += (object sender, EventArgs e) => canQueueCombo = false;
        }
        private void Start()
        {
            PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;

            PlayerAnimator animator = localPlayerStateMachine.gameObject.GetComponent<PlayerAnimator>();

            animator.OnAnimStarted += (object sender, EventArgs e) => hasAnimFinished = false;
            animator.OnAnimFinished += (object sender, EventArgs e) =>  hasAnimFinished = true;
        }

        private void WeaponHolster_OnNewActiveWeapon(object sender, WeaponHolster.WeaponEventArgs e)
        {
            weapon = e.Weapon;

            lightComboCount = weapon.LightCombo.Count;
            heavyComboCount = weapon.HeavyCombo.Count;

            lightChargedTimer = weapon.LightChargedAttack.chargingTimer;
            heavyChargedTimer = weapon.HeavyChargedAttack.chargingTimer;
        }

        public void AttackInputPerformed(AttackType attackType)
        {
            if (isHoldingAttack)
            {
                if (queuedAttackType != attackType) Debug.Log("Clicked other button while holding other"); 
                return;
            }

            if (combatAttackState == CombatAttackState.Aim) { AimAttack(attackType); return; }

            if (hasAnimFinished) { FirstAttack(attackType); return; }

            if (CanRecover(attackType)) { RecoveryAttack(); return; }

            if (!canQueueCombo)
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
            ResetSpecialAttackValues();

            if (combatAttackState == CombatAttackState.Aerial) { AerialAttack(attackType); return; }

            if (combatAttackState == CombatAttackState.Sprint) { SprintAttack(attackType); return; }

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
            ExecuteSpecialAttack(
                weapon.LightChargedAttack,
                weapon.HeavyChargedAttack,
                attackType,
                () => hasChargedAttacked = true
            );
        }

        public void SprintAttack(AttackType attackType)
        {
            ExecuteSpecialAttack(
                weapon.LightSprintAttack,
                weapon.HeavySprintAttack,
                attackType,
                () => hasSprintAttacked = true
            );
        }

        public void AerialAttack(AttackType attackType)
        {
            ExecuteSpecialAttack(
                weapon.LightAerialAttack,
                weapon.HeavyAerialAttack,
                attackType,
                () => hasAerialAttacked = true
            );
        }

        public void AimAttack(AttackType attackType)
        {
            AttackSO aimAttack = attackType == AttackType.Light ? weapon.LightAimAttack : weapon.HeavyAimAttack;
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
            return ((hasAerialAttacked || hasSprintAttacked || hasChargedAttacked) && executedAttack.hasRecoveryAttack
                && executedAttackType == attackType);
        }  

        public void RecoveryAttack()
        {
            if (!TryExecuteAttack(weapon.RecoveryAttacks[executedAttack.recoveryAttack])) return;

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
            if (!PlayerStateMachine.LocalInstance.Utilities.HasSufficientStamina(attack.staminaCost)) return false;

            weapon.currentAttack = attack;
            weapon.SetDamageMultiplier(attack.damageMultiplier);

            attackAnimationData.stateName = attack.animationState.ToString();
            attackAnimationData.animationSpeed = attack.animationSpeed;

            hasAnimFinished = false;
            hasQueuedAttack = false;

            executedAttack = attack;
            OnExecuteAttack?.Invoke(this, attackAnimationData);

            if(attack.canCombo)
            {
                canQueueCombo = true;
                StartCoroutine(CheckForQueuedComboAttack());
            }

            Debug.Log(attack.animationState);

            return true;
        }

        private IEnumerator CheckForQueuedComboAttack()
        {
            while(canQueueCombo) yield return null;

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
            hasChargedAttacked = false;

            float chargeTimer = (chargedAttackType == AttackType.Light) ? lightChargedTimer : heavyChargedTimer;

            float timer = -chargeTimer / 2f;

            while (isHoldingAttack)
            {
                if (timer < chargeTimer)
                {
                    if (!canQueueCombo)
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
                    comboValue = (comboValue + 1) % lightComboCount;
                    if (TryExecuteAttack(weapon.LightCombo[comboValue])) executedAttackType = attackType;
                    break;
                case AttackType.Heavy: 
                    comboValue = (comboValue + 1) % heavyComboCount;
                    if (TryExecuteAttack(weapon.HeavyCombo[comboValue])) executedAttackType = attackType;
                    break;
            }
        }

        private void ResetSpecialAttackValues()
        {
            hasChargedAttacked = false;
            hasSprintAttacked = false;
            hasAerialAttacked = false;
            comboValue = -1;
        }

        public void SetAttackState(CombatAttackState combatAttackState) => this.combatAttackState = combatAttackState;
       
        private void LogWarning(object message) => Debug.LogWarning(message);

        private void Log(object message) => Debug.Log(message);
    }
    public enum AttackType
    {
        Heavy,
        Light
    }

    public enum CombatAttackState
    {
        General,
        Aerial,
        Sprint,
        Aim,
    }
}