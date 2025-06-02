using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TrialsOfOdin.Combat
{
    [System.Serializable]
    public class CombatInputHandler
    {
        [SerializeField] private bool useInputs;
        [SerializeField, ShowIf(nameof(useInputs))] private InputActionReference lightAttack;
        [SerializeField, ShowIf(nameof(useInputs))] private InputActionReference heavyAttack;
        [SerializeField, ShowIf(nameof(useInputs))] private InputActionReference aim;
        public bool UseInputs { get { return useInputs; } }

        public event Action<AttackType> OnAttackPerformed;
        public event Action<AttackType> OnAttackReleased;
        public event Action<bool> OnAimingChanged;

        private InputAction lightInput;
        private InputAction heavyInput;
        private InputAction aimInput;

        public void Enable()
        {
            if (lightAttack != null)
            {
                lightInput = lightAttack.action;
                lightInput.started += OnLightAttackPerformed;
                lightInput.canceled += OnLightAttackReleased;
                lightInput.Enable();
            }

            if (heavyAttack != null)
            {
                heavyInput = heavyAttack.action;
                heavyInput.started += OnHeavyAttackPerformed;
                heavyInput.canceled += OnHeavyAttackReleased;
                heavyInput.Enable();
            }

            if(aimInput != null)
            {
                aimInput = aim.action;
                aimInput.started += OnAimStarted;
                aimInput.canceled += OnAimFinished;
                aimInput.Enable();
            }
        }

        public void Disable()
        {
            if (lightInput != null)
            {
                lightInput.started -= OnLightAttackPerformed;
                lightInput.canceled -= OnLightAttackReleased;
                lightInput.Disable();
            }

            if (heavyInput != null)
            {
                heavyInput.started -= OnHeavyAttackPerformed;
                heavyInput.canceled -= OnHeavyAttackReleased;
                heavyInput.Disable();
            }

            if(aimInput != null)
            {
                aimInput.started -= OnAimStarted;
                aimInput.canceled -= OnAimFinished;
                aimInput.Disable();
            }
        }

        private void OnLightAttackPerformed(InputAction.CallbackContext ctx) => OnAttackPerformed?.Invoke(AttackType.Light);
        private void OnLightAttackReleased(InputAction.CallbackContext ctx) => OnAttackReleased?.Invoke(AttackType.Light);
        private void OnHeavyAttackPerformed(InputAction.CallbackContext ctx) => OnAttackPerformed?.Invoke(AttackType.Heavy);
        private void OnHeavyAttackReleased(InputAction.CallbackContext ctx) => OnAttackReleased?.Invoke(AttackType.Heavy);
        private void OnAimStarted(InputAction.CallbackContext ctx) => OnAimingChanged?.Invoke(true);
        private void OnAimFinished(InputAction.CallbackContext ctx) => OnAimingChanged?.Invoke(false);
    }
}
