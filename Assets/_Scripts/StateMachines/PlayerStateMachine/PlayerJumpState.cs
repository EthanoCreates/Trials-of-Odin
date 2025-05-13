using System.Collections;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private LayerMask groundLayers;
    private Transform transform;
    private float gravity;

    public PlayerJumpState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate) { }

    public override void EnterState()
    {
        JumpSetUp();
        InitializeSubState();
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return StateKey;
    }

    public override void UpdateState()
    {
        MovementUtility.ApplyGravity(1f);

        MovementUtility.MovePlayer();
    }
    private void InitializeSubState()
    {
        SetSubState(PlayerStateMachine.LocalInstance.GetStateInstance(PlayerStateMachine.EPlayerState.AerialMovement));
    }

    public override void ExitState() 
    {
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnApplyJumpVelocity -= Player_OnApplyJumpVelocity;
    }

    private void JumpSetUp()
    {
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnApplyJumpVelocity += Player_OnApplyJumpVelocity;

        //playing jump anim turning off grounded
        AnimationRequestor.AnimateJump();

        AudioRequestor.PlayJumpSound();
        MovementUtility.VerticalVelocity = 0f;

        transform = MovementUtility.Transform;
        gravity = MovementUtility.Gravity;

        groundLayers = Context.GroundLayers;
        CombatManager.IsAerialAttack = false;
    }

    private void Player_OnApplyJumpVelocity(object sender, System.EventArgs e)
    {
        ApplyJumpVelocity();
    }


    private void ApplyJumpVelocity()
    {
        MovementUtility.VerticalVelocity = Mathf.Sqrt(Context.JumpHeight * -2f * gravity);
    }
}
