using System;
using System.Collections;
using TrialsOfOdin.Stats;
using UnityEngine;

public class PlayerContext : HumanoidSMContext
{
    public event EventHandler OnResetCamera;
    public event EventHandler OnResetAnimator;

    public PlayerContext(PlayerStats stats)
    {
        PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;

        CharacterController = localPlayerStateMachine.CharacterController;
        humanoidStats = stats;

        SprintTimer = humanoidStats.SprintTime;

        GameInput gameInput = GameInput.Instance;
        gameInput.OnJump += (object sender, EventArgs e) => { IsJumping = true; CoroutineHelper.Instance.StartCoroutine(TurnOffJumpAfterDelay(JumpTimeout + .1f)); };
        gameInput.OnDodge += (object sender, EventArgs e) => { IsDodging = true; CoroutineHelper.Instance.StartCoroutine(TurnOffDodgeAfterDelay(.6f)); };

        PlayerAnimator animator = localPlayerStateMachine.gameObject.GetComponent<PlayerAnimator>();

        animator.OnAnimStarted += (object sender, EventArgs e) => AnimFinished = false;
        animator.OnAnimFinished += (object sender, EventArgs e) => AnimFinished = true;
    }


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