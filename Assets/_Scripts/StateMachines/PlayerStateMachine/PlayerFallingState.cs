using UnityEngine;

public class PlayerFallingState : PlayerState
{
    private LayerMask groundLayers;
    private Transform transform;

    public PlayerFallingState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate)
    {
        IsRootState = true;
    }

    private float fallGravityMultiplier;
    private float terminalvelocity;

    public override void EnterState()
    {
        FallingSetUp();
        InitializeSubState();
    }

    private void InitializeSubState()
    {
        SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.AerialMovement));
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if(Context.IsGrounded) return PlayerStateMachine.EPlayerState.Grounded;
        return StateKey;
    }

    public override void UpdateState()
    {
        //falling logic
        if (MovementUtility.VerticalVelocity < terminalvelocity)
        {
            // add to vertical velocity
            MovementUtility.ApplyGravity(fallGravityMultiplier);
        }
        MovementUtility.MovePlayer();
    }

    public override void ExitState()
    {
        AnimationRequestor.AnimateExitFalling();
        if(MovementUtility.VerticalVelocity < -15f) Context.Landing = true;
    }

    private bool ValidAerialAttackHeight()
    {
        if (Physics.Raycast(transform.position, -transform.up, 10f, groundLayers))
        {
            return true;
        }
        return false;
    }

    private void FallingSetUp()
    {
        AnimationRequestor.AnimateFalling();
        fallGravityMultiplier = Context.FallGravityMultiplier;
        terminalvelocity = Context.TerminalVelocity;

        transform = MovementUtility.Transform;

        groundLayers = Context.GroundLayers;
    }
}
