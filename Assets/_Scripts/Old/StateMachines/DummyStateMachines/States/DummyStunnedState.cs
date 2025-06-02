using UnityEngine;

public class DummyStunnedState : DummyState
{
    public DummyStunnedState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {
        AnimationRequestor.AnimateStun();
        CombatManager.ApplyForceToRagdollAndLimb();
    }

    public override void ExitState() { }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if(Context.AnimFinished) CombatManager.IsDamaged = false;
        return StateKey;
    }

    public override void UpdateState() { }
}
