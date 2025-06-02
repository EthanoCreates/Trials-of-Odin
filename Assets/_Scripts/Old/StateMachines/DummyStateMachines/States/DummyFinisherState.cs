using UnityEngine;

public class DummyFinisherState : DummyState
{
    private float finisherAvailibleTimer;
    private float finisherAvailibleTime = 10f;

    public DummyFinisherState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {
        finisherAvailibleTimer = finisherAvailibleTime;
        CombatManager.isFinisherable = true;
        CombatManager.FinisherAvailable(finisherAvailibleTimer);
        Utilities.AnimationRequestor.AnimateLoopingStun();
    }

    public override void ExitState()
    {
        CombatManager.IsDamaged = false;
        Utilities.AnimationRequestor.AnimateLoopingStunExit();
        CombatManager.isFinisherable = false;
    }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (!CombatManager.isFinisherable) return DummyStateMachine.EDummyState.Idle;

        return StateKey;
    }

    public override void UpdateState()
    {
        finisherAvailibleTimer -= Time.deltaTime;
        if (finisherAvailibleTimer < 0)
        {
            CombatManager.isFinisherable = false;
        }

        if (CombatManager.beingFinishered)
        {
            if (Context.AnimFinished)
            {
                CombatManager.Kill();
                CombatManager.isFinisherable = false;
            }
        }
    }
}
