using Ami.BroAudio;
using UnityEngine;

public class PlayerDodgeState : PlayerState
{
    public PlayerDodgeState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate) { }

    public override void EnterState()
    {
        //refreshing anim flag as this state is an interupt
        Context.AnimationStarted();
        DodgeSetUp();
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (Context.IsDamaged) return PlayerStateMachine.EPlayerState.Stunned;
        if (Context.AnimFinished)
        {
            return CheckForStateChange();
        }
        return StateKey;
    }

    public override void UpdateState() {  }
    public override void ExitState() { }

    private void DodgeSetUp()
    {
        AnimationRequestor.ResetLocomotion();

        float dodgeX = GameInput.Instance.GetMovementInput().ReadValue<Vector2>().x;
        float dodgeY = GameInput.Instance.GetMovementInput().ReadValue<Vector2>().y;

        if (Mathf.Abs(dodgeX) < .3f) dodgeX = 0;
        else if(dodgeX > 0) { dodgeX = 1; } else {  dodgeX = -1; }
        if (Mathf.Abs(dodgeY) < .3f) dodgeY = 0;
        else if (dodgeY > 0) { dodgeY = 1; } else {  dodgeY = -1; }

        CombatManager.Weapon.WeaponData.effortSounds.Play().SetVolume(.3f);

        AnimationRequestor.AnimateDodge(dodgeX, dodgeY);
        AudioRequestor.PlayJumpSound();
        Context.IsDodging = false;
    }
}

