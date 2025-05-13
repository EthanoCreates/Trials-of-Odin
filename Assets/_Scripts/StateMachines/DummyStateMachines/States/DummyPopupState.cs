using UnityEngine;

public class DummyPopupState : DummyState
{
    public DummyPopupState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key){ }

    public override void EnterState()
    {
        CombatManager.MakeDummyFall();
        CombatManager.PopUp();
    }


    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (!CombatManager.IsFallen) CombatManager.IsDamaged = false;
        return StateKey;
    }

    public override void UpdateState() { }
    public override void ExitState() { }
}
