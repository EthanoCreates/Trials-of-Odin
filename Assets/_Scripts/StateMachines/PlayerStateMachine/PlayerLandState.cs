using System;
using UnityEngine;

public class PlayerLandState : PlayerState
{
    public PlayerLandState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState stateKey) : base(utilities, stateKey) { }

    public override void EnterState()
    {
        CombatManager.OnHeavyAttackStarted += PlayerContext_OnHeavyAttack;
        CombatManager.OnLightAttackStarted += CombatHelper_OnLightAttackStarted;
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        if (Context.IsDamaged) return PlayerStateMachine.EPlayerState.Stunned;

        if (CombatManager.IsRecoveringFromAerialAttack && !CombatManager.IsComboAvailable)
        {
            CombatManager.IsComboTransition = true;
            if (CombatManager.IsHeavyAttackAfterAerial) return PlayerStateMachine.EPlayerState.HeavyAttack;
            else return PlayerStateMachine.EPlayerState.LightAttack;
        }
        if(CombatManager.IsComboingFromAerialAttack && !CombatManager.IsComboAvailable)
        {
            CombatManager.IsComboTransition = true;
            if (CombatManager.IsHeavyAttackAfterAerial) return PlayerStateMachine.EPlayerState.HeavyAttack;
            else return PlayerStateMachine.EPlayerState.LightAttack;
        }

        if (Context.AnimFinished)
        {
            Context.Landing = false;
            return CheckForStateChange();
        }
        return StateKey;
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        CombatManager.OnHeavyAttackStarted -= PlayerContext_OnHeavyAttack;
        //if player doesn't initiate a recovery this will remain true and
        //if the player falls instead of jumping it won't be reset so reset on landing
        CombatManager.IsAerialAttack = false;
    }

    private void PlayerContext_OnHeavyAttack(object sender, EventArgs e)
    {
        if (!CombatManager.IsAerialAttack) return;

        CombatManager.AerialTransitionAttack(true);
    }

    private void CombatHelper_OnLightAttackStarted(object sender, EventArgs e)
    {
        if (!CombatManager.IsAerialAttack) return;

        CombatManager.AerialTransitionAttack(false);
    }
}
