using UnityEngine;

public class DummyKnockdownState : DummyState
{
    public DummyKnockdownState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {
        CombatManager.MakeDummyFall();
        CombatManager.KnockDown();
    }

    public override void ExitState()
    {

    }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (!CombatManager.IsFallen) CombatManager.IsDamaged = false;
        return StateKey;
    }

    public override void UpdateState()
    {

    }
}
