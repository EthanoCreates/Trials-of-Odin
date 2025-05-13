using System;
using System.Collections;
using UnityEngine;

public class PlayerContext : HumanoidSMContext
{
    public event EventHandler OnResetCamera;
    public event EventHandler OnResetAnimator;

    public PlayerContext(PlayerDataSO playerData)
    {
        PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;

        CharacterController = localPlayerStateMachine.CharacterController;
        GroundLayers = playerData.groundLayers;
        VaultLayer = playerData.vaultLayers;
        ClimbLayer = playerData.climbLayers;
        WalkSpeed = playerData.walkSpeed;
        RunSpeed = playerData.runSpeed;
        SprintSpeed = playerData.sprintSpeed;
        SprintTime = playerData.sprintTime;
        JumpHeight = playerData.jumpHeight;

        SprintTimer = playerData.sprintTime;
        SprintTime = playerData.sprintTime;

        GameInput gameInput = GameInput.Instance;
        gameInput.OnJump += (object sender, EventArgs e) => { IsJumping = true; CoroutineHelper.Instance.StartCoroutine(TurnOffJumpAfterDelay(JumpTimeout + .1f)); };
        gameInput.OnDodge += (object sender, EventArgs e) => { IsDodging = true; CoroutineHelper.Instance.StartCoroutine(TurnOffDodgeAfterDelay(.6f)); };

        PlayerAnimator animator = localPlayerStateMachine.gameObject.GetComponent<PlayerAnimator>();

        animator.OnAnimStarted += (object sender, EventArgs e) => AnimFinished = false;
        animator.OnAnimFinished += (object sender, EventArgs e) => AnimFinished = true;
    }

    //Player specific Movement fields 
    public float SprintSpeed { get; private set; }
    public float SprintTime { get; private set; }
    public float SprintTimer { get; set; }
    public bool IsDamaged { get; set; }

    public override bool CheckIfGrounded()
    {
        return CharacterController.isGrounded;
    }

    //Referenced Fields
    public CharacterController CharacterController { get; private set; }
    public LayerMask VaultLayer { get; private set; }
    public LayerMask ClimbLayer { get; private set; }

    public void ResetCamera()
    {
        OnResetCamera?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator TurnOffJumpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsJumping = false;
    }

    private IEnumerator TurnOffDodgeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsDodging = false;
    }
}