using UnityEngine;

public class DummyLandState : DummyState
{
    public DummyLandState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        return DummyStateMachine.EDummyState.Idle;
        return StateKey;
    }

    public override void UpdateState()
    {

    }
}
