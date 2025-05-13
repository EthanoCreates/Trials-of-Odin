using UnityEngine;

public class PlayerStunnedState : PlayerState
{
    public PlayerStunnedState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState stateKey) : base(utilities, stateKey) { }

    public override void EnterState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if(Context.AnimFinished)
        {
            Context.IsDamaged = false;
            if (CombatManager.IsBlocking) return PlayerStateMachine.EPlayerState.Block;
            if (CanTransitionToDodge()) return PlayerStateMachine.EPlayerState.Dodge;
            if (CanTransitionToHeavyAttack()) return PlayerStateMachine.EPlayerState.HeavyAttack;
            if (CanTransitionToLightAttack()) return PlayerStateMachine.EPlayerState.LightAttack;
            if (GameInput.Instance.GetMovementInput().ReadValue<Vector2>().magnitude > 0 && GameInput.Instance.IsSprinting()) return PlayerStateMachine.EPlayerState.Run;
            if (GameInput.Instance.GetMovementInput().ReadValue<Vector2>().magnitude > 0) return PlayerStateMachine.EPlayerState.Walk;
            return PlayerStateMachine.EPlayerState.Idle;
        }
        return StateKey;
    }

    public override void UpdateState() { }

    public override void ExitState() { }
}
