using UnityEngine;

public class DummyKnockbackState : DummyState
{
    public DummyKnockbackState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {
        CombatManager.MakeDummyFall();
        EnemyDataSO data = Utilities.dummyData;
        CombatManager.ApplyForceToRagdollAndLimb(data.ImpactPower/2, .1f);
    }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (!CombatManager.IsFallen) CombatManager.IsDamaged = false;

        return StateKey;
    }

    public override void UpdateState() { }
    public override void ExitState() { }
}
