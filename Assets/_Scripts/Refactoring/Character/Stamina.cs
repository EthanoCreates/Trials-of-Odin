using Sirenix.OdinInspector;
using UnityEngine;

namespace TrialsOfOdin.Stats
{
    public class Stamina : MonoBehaviour
    {
        public float maxStamina = 100f;
        [SerializeField] private float recoveryRate = 10f;
        [SerializeField] private float recoveryDelay = 1f;
        [SerializeField] private bool canSprint;
        [SerializeField, ShowIf(nameof(canSprint))] private float sprintStaminaCost = 3f;
        [SerializeField, ReadOnly] private float currentStamina;
        private bool isRecovering;
        public float CurrentStamina
        {
            get => currentStamina;
            private set => currentStamina = Mathf.Clamp(value, 0, maxStamina);      
        }

        public delegate void StaminaChangedEvent(float currentStamina, float maxStamina, bool shouldLerpUI);
        public event StaminaChangedEvent OnStaminaChanged;

        private void Start()
        {
            CurrentStamina = maxStamina;
            UpdateStaminaUI(false);
        }

        public bool HasStamina(float cost) => CurrentStamina >= cost;

        public bool TryUseStamina(float cost)
        {
            if (!HasStamina(cost)) return false;

            CurrentStamina -= cost;
            UpdateStaminaUI(true);

            isRecovering = false;
            CancelInvoke(nameof(StartRecovering));
            Invoke(nameof(StartRecovering), recoveryDelay);

            return true;
        }
        public bool TrySprint()
        {
            if (!canSprint || CurrentStamina <= 0) return false;

            CurrentStamina -= sprintStaminaCost * Time.deltaTime;
            UpdateStaminaUI(false);

            return true;
        }
        public void RecoverStamina()
        {
            if (!isRecovering || CurrentStamina >= maxStamina) return;

            CurrentStamina += recoveryRate * Time.deltaTime;
            UpdateStaminaUI(false);
        }

        private void StartRecovering() => isRecovering = true;
        private void UpdateStaminaUI(bool shouldLerpUI) => OnStaminaChanged?.Invoke(CurrentStamina, maxStamina, shouldLerpUI);
    }
}
