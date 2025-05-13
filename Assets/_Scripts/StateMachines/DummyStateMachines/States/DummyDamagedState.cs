using UnityEngine;

public class DummyDamagedState : DummyState
{
    public DummyDamagedState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key){ }

    public override void EnterState() { InitalizeSubStates(); }

    private void InitalizeSubStates()
    {
        switch(CombatManager.DamageType)
        {
            case DamageType.Minor:
                CombatManager.IsDamaged = false;
                break;
            case DamageType.Stagger:
                SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Stagger));
                break;
            case DamageType.Stun: 
                SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Stunned));
                break;
            case DamageType.Knockback:
                SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Knockback));
                break;
            case DamageType.Knockdown:
                SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Knockdown));
                break;
            case DamageType.Popup:
                SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Popup));
                break;
            case DamageType.Finisher:
                SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Finisher));
                break;
        }
    }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (!CombatManager.IsDamaged) return DummyStateMachine.EDummyState.Idle;
        return StateKey;
    }

    public override void UpdateState() { }
    public override void ExitState() { }
}
