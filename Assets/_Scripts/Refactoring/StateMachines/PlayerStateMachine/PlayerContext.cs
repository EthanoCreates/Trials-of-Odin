using System;
using System.Collections;
using TrialsOfOdin.Combat;
using TrialsOfOdin.State;
using TrialsOfOdin.Stats;
using UnityEngine;

public class PlayerContext : CharacterContext
{
    public event EventHandler OnResetCamera;
    public event EventHandler OnResetAnimator;
    private CombatSystemPro combatSystemPro;

    public PlayerContext(CharacterStats stats, CharacterController characterController, CombatSystemPro combatSystemPro) : base (stats)
    {
        humanoidStats = stats;

        CharacterController = characterController;

        SprintTimer = humanoidStats.SprintTime;

        GameInput gameInput = GameInput.Instance;
        gameInput.OnJump  += () => { IsJumping = true; CoroutineHelper.Instance.StartCoroutine(TurnOffJumpAfterDelay(JumpTimeout + .1f)); };
        gameInput.OnDodge += () => { IsDodging = true; CoroutineHelper.Instance.StartCoroutine(TurnOffDodgeAfterDelay(.6f)); };

        PlayerAnimator playerAnimator = PlayerStateMachine.LocalInstance.GetComponent<PlayerAnimator>();
        playerAnimator.OnAnimStarted += () => AnimationStarted();
        playerAnimator.OnAnimFinished += () => AnimationFinished();

        this.combatSystemPro = combatSystemPro;

        combatSystemPro.OnExecuteAttack += () => IsAttacking = true;
    }

    public override bool CheckIfGrounded()
    {
        return CharacterController.isGrounded;
    }

    //Referenced Fields
    public CharacterController CharacterController { get; private set; }
    public LayerMask VaultLayer { get; private set; }
    public LayerMask ClimbLayer { get; private set; }

    public override bool IsCharging => combatSystemPro.isHoldingAttack;

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