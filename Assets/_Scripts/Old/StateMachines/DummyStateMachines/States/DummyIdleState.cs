using UnityEngine;

public class DummyIdleState : DummyState
{
    public DummyIdleState(DummyUtilities utilities ,DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {
        //Utilities.Agent.isStopped = true;
    }

    public override void ExitState()
    {
        //Utilities.Agent.isStopped = false;
    }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (CombatManager.IsDamaged) return DummyStateMachine.EDummyState.Damaged; 

        return StateKey;
    }

    public override void UpdateState() 
    {

    }
}
