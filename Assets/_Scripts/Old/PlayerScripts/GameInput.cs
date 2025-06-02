using System;
using TrialsOfOdin.State;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    //triggered events
    public event Action OnJump;
    public event Action OnHeavyAttackStarted;
    public event Action OnHeavyAttackFinished;
    public event Action OnLightAttackStarted;
    public event Action OnLightAttackFinished;
    public event Action OnLockOn;
    public event Action OnBlockStarted;
    public event Action OnBlockFinished;
    public event Action OnAimStarted;
    public event Action OnAimFinished;
    public event Action OnInteract;
    public event Action OnDodge;
    public event Action OnEquipPrimary;
    public event Action OnEquipSecondary;
    public event Action OnEquipShield;
    public event Action OnTesterInput;
    public event Action OnRage;

    //caching the move action as it is used very often, this allows us to just read it
    private InputAction move;
    private InputAction look;
    private InputAction sprint;

    public bool InputEnabled { get; private set; }

    //Input Asset
    private PlayerInputActions playerInputActions;

    //current inputDevice
    InputDevice currentInputDevice;

    public override void Awake()
    {
        base.Awake();
        playerInputActions = new PlayerInputActions();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //contiuous movement actions
        move = playerInputActions.Player.Move;
        look = playerInputActions.Player.Look;
        sprint = playerInputActions.Player.Sprint;

        //triggered movement actions
        playerInputActions.Player.Jump.started += Jump_started;
        //triggered combat actions
        playerInputActions.Player.Aim.started += Aim_started;
        playerInputActions.Player.Aim.canceled += Aim_canceled;
        playerInputActions.Player.HeavyAttack.started += HeavyAttack_started;
        playerInputActions.Player.HeavyAttack.canceled += HeavyAttack_canceled;

        //LockOn
        playerInputActions.Player.LockOn.performed += LockOn_performed;

        playerInputActions.Player.LightAttack.started += LightAttack_started;
        playerInputActions.Player.LightAttack.canceled += LightAttack_canceled;
        //Interact actions
        playerInputActions.Player.Interact.started += Interact_started;

        //Dodge actions
        playerInputActions.Player.Dodge.started += Dodge_started;

        //Block actions
        playerInputActions.Player.Block.started += Block_started;
        playerInputActions.Player.Block.canceled += Block_canceled;

        //Equip actions
        playerInputActions.Player.EquipPrimary.started += EquipPrimary_started;
        playerInputActions.Player.EquipSecondary.started += EquipSecondary_started;
        playerInputActions.Player.EquipShield.started += EquipShield_started;

        //Rage action
        playerInputActions.Player.Rage.started += Rage_started;

        //helper actions 
        playerInputActions.Player.TesterInput.started += TesterInput_started;

        if (PlayerStateMachine.LocalInstance == null)
        {
            PlayerStateMachine.OnAnyPlayerSpawn += PlayerStateMachine_OnAnyPlayerSpawn;
        }

        //input device checkers 
        InputSystem.onActionChange += InputSystem_onActionChange;
        InputSystem.onDeviceChange += InputSystem_onDeviceChange;

        //enabling player actions
        playerInputActions.Player.Enable();
    }

    private void Rage_started(InputAction.CallbackContext obj)
    {
        OnRage?.Invoke();
    }

    private void LockOn_performed(InputAction.CallbackContext obj)
    {
        OnLockOn?.Invoke();
    }

    private void TesterInput_started(InputAction.CallbackContext obj)
    {
        OnTesterInput?.Invoke();
    }

    private void EquipShield_started(InputAction.CallbackContext obj)
    {
        OnEquipShield?.Invoke();
    }

    public bool StillHoldingHeavy()
    {
        return playerInputActions.Player.HeavyAttack.IsPressed();
    }

    public bool StillHoldingLight()
    {
        return playerInputActions.Player.LightAttack.IsPressed();
    }

    private void EquipSecondary_started(InputAction.CallbackContext obj)
    {
        OnEquipSecondary?.Invoke();
    }

    private void EquipPrimary_started(InputAction.CallbackContext obj)
    {
        OnEquipPrimary?.Invoke();
    }

    private void Block_canceled(InputAction.CallbackContext obj)
    {
        OnBlockFinished?.Invoke();
    }

    private void Block_started(InputAction.CallbackContext obj)
    {
        OnBlockStarted?.Invoke();
    }

    private void LightAttack_canceled(InputAction.CallbackContext obj)
    {
        OnLightAttackFinished?.Invoke();
    }

    private void LightAttack_started(InputAction.CallbackContext obj)
    {
        OnLightAttackStarted?.Invoke();
    }

    private void Animator_OnEnableMovement(object sender, EventArgs e)
    {
        EnablePlayerMovement();
    }

    private void Animator_OnDisableMovement(object sender, EventArgs e)
    {
        DisablePlayerMovement();
    }

    private void PlayerStateMachine_OnAnyPlayerSpawn(object sender, EventArgs e)
    {
        if(PlayerStateMachine.LocalInstance != null)
        {

        }
    }

    private void Dodge_started(InputAction.CallbackContext obj)
    {
        OnDodge?.Invoke();
    }

    private void Interact_started(InputAction.CallbackContext obj)
    {
        //potentially a valid interact checker
        OnInteract?.Invoke();
    }


    private void OnDestroy()
    {
        //triggered movement actions
        playerInputActions.Player.Jump.started -= Jump_started;
        //triggered combat actions
        playerInputActions.Player.Aim.started -= Aim_started;
        playerInputActions.Player.Aim.canceled -= Aim_canceled;

        playerInputActions.Player.HeavyAttack.started -= HeavyAttack_started;
        playerInputActions.Player.HeavyAttack.canceled -= HeavyAttack_canceled;

        playerInputActions.Player.LightAttack.started -= LightAttack_started;
        playerInputActions.Player.LightAttack.canceled -= LightAttack_canceled;

        //Lock on action
        playerInputActions.Player.LockOn.performed += LockOn_performed;

        //Interact actions
        playerInputActions.Player.Interact.started -= Interact_started;

        //Dodge actions
        playerInputActions.Player.Dodge.started -= Dodge_started;

        //Block actions
        playerInputActions.Player.Block.started -= Block_started;
        playerInputActions.Player.Block.canceled -= Block_canceled;

        //Equip actions
        playerInputActions.Player.EquipPrimary.started -= EquipPrimary_started;
        playerInputActions.Player.EquipSecondary.started -= EquipSecondary_started;
        playerInputActions.Player.EquipShield.started -= EquipShield_started;

        //Rage action
        playerInputActions.Player.Rage.started += Rage_started;

        //Static event
        PlayerStateMachine.OnAnyPlayerSpawn -= PlayerStateMachine_OnAnyPlayerSpawn;

        //helper actions 
        playerInputActions.Player.TesterInput.started -= TesterInput_started;

        //input device checkers 
        InputSystem.onActionChange -= InputSystem_onActionChange;
        InputSystem.onDeviceChange -= InputSystem_onDeviceChange;

        playerInputActions.Dispose();
    }

    private void Jump_started(InputAction.CallbackContext obj)
    {
        OnJump?.Invoke();
    }

    private void HeavyAttack_started(InputAction.CallbackContext obj)
    {
        OnHeavyAttackStarted?.Invoke();
    }

    private void HeavyAttack_canceled(InputAction.CallbackContext obj)
    {
        OnHeavyAttackFinished?.Invoke();
    }


    private void Aim_canceled(InputAction.CallbackContext obj)
    {
        OnAimFinished?.Invoke();
    }

    private void Aim_started(InputAction.CallbackContext obj)
    {
        OnAimStarted?.Invoke();
    }

    private void InputSystem_onDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                currentInputDevice = device;
                break;
            case InputDeviceChange.Removed:
                currentInputDevice = InputSystem.devices[0];
                break;
            case InputDeviceChange.Disconnected:
                currentInputDevice = InputSystem.devices[0];
                break;
        }
    }

    private void InputSystem_onActionChange(object inputAction, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            var action = inputAction as InputAction;
            var inputDevice = action.activeControl?.device;
            if (inputDevice != currentInputDevice)
            {
                currentInputDevice = action.activeControl?.device;
            }
        }
    }

    public bool IsSprinting()
    {
        //if (ThirdPersonCombat.LocalInstance.IsAttacking()) return false;
        if (currentInputDevice is Gamepad)
        {
            // Check if the left thumbstick is tilted forward and has input
            Vector2 leftStickInput = move.ReadValue<Vector2>();
            if (leftStickInput.magnitude > 0.7f)
            {
                return true;
            }
        }
        if (sprint.ReadValue<float>() == 1)
        {
            return true;
        }
        return false;
    }

    public bool IsTacticalSprinting()
    {
        if (sprint.ReadValue<float>() == 1)
            return true;
        if (currentInputDevice is Gamepad)
        {
            if (sprint.ReadValue<float>() > .5f)
                return true;
        }
        return false;
    }

    public bool controllerTacticalSprintCheck()
    {
        if (currentInputDevice is Gamepad)
        {
            if (sprint.ReadValue<float>() > .5f)
                return true;
        }
        return false;
    }

    public void DisablePlayerMovement()
    {
        playerInputActions.Player.Move.Disable();
        playerInputActions.Player.Jump.Disable();
        InputEnabled = false;
    }

    public void EnablePlayerMovement()
    {
        playerInputActions.Player.Move.Enable();
        playerInputActions.Player.Jump.Enable();
        InputEnabled = true;
    }

    public InputDevice GetCurrentInputDevice()
    {
        return currentInputDevice;
    }

    public InputAction GetMovementInput()
    {
        return move;
    }

    public InputAction GetLookInput()
    {
        return look;
    }

    public InputAction GetSprintInput()
    {
        return sprint;
    }
}
