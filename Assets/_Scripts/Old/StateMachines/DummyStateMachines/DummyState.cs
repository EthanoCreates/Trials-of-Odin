using UnityEngine;

public abstract class DummyState : BaseState<DummyStateMachine.EDummyState>
{
    protected readonly DummyUtilities Utilities;
    protected DummyContext Context => Utilities.Context;
    protected DummyCombatManager CombatManager => Utilities.CombatManager;
    protected DummyAnimationRequestor AnimationRequestor => Utilities.AnimationRequestor;

    protected DummyState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(key) 
    {
        this.Utilities = utilities; 
    }

    public override void UpdateSubState()
    {
        Utilities.StateMachine.CurrentState(this);
    }
}
