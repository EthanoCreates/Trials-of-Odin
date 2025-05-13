using UnityEngine;

public class DummyStaggerState : DummyState
{
    private float staggerTime = .5f;
    private float staggerTimer;

    public DummyStaggerState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {
        staggerTimer = staggerTime;
        CombatManager.ApplyForceToRagdollAndLimb();
    }

    public override void ExitState() { }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (staggerTimer < 0) CombatManager.IsDamaged = false;
        return StateKey;
    }

    public override void UpdateState()
    {
        staggerTimer -= Time.deltaTime;
    }
}
