using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    //Ground check variables
    private float timeTillFall;
    private float timerTillFall;
    private float jumpTimeout;
    private bool isFalling;

    public PlayerGroundedState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GroundedSetup();
        InitializeSubState();
    }

    private void InitializeSubState()
    {
        if(Context.Landing) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Land));
        else if (CombatManager.IsBlocking) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Block));
        else if (CanTransitionToDodge()) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Dodge));
        else if (CanTransitionToHeavyAttack()) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.HeavyAttack));
        else if (CanTransitionToLightAttack()) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.LightAttack));
        else if (GameInput.Instance.IsSprinting() && MovementUtility.Speed > 0) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Run));
        else if (MovementUtility.Speed > 0) SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Walk));
        else SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.Idle));
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (isFalling) return PlayerStateMachine.EPlayerState.Falling;
        if (Context.IsJumping && jumpTimeout < 0 && Context.AnimFinished)  return PlayerStateMachine.EPlayerState.Ascend;

        return StateKey;
    }

    public override void UpdateState()
    {
        HandleStamina();

        if (jumpTimeout >= 0.0f)
        {
            jumpTimeout -= Time.deltaTime;
        }

        if (Context.IsGrounded)
        {
            timerTillFall = timeTillFall;
            return;
        }

        if (timerTillFall >= 0.0f)
        {
            timerTillFall -= Time.deltaTime;
            MovementUtility.ApplyGravity(1f);
        }
        else
        {
            isFalling = true;
        }
    }

    private void HandleStamina()
    {
        if(CurrentSubState.StateKey != PlayerStateMachine.EPlayerState.Run)
        {
            Utilities.RecoverStamina();
        }
    }

    public override void ExitState() { }

    private void GroundedSetup()
    {
        AnimationRequestor.AnimateGrounded();

        AnimationRequestor.AnimateLand(MovementUtility.VerticalVelocity);
        AudioRequestor.PlayLandSound();

        jumpTimeout = Context.JumpTimeout;
        timeTillFall = Context.FallTimeout;
        timerTillFall = timeTillFall;

        isFalling = false;
        MovementUtility.VerticalVelocity = -2f; //Ensures player does not clip through ground by lowering downward force
    }
}
