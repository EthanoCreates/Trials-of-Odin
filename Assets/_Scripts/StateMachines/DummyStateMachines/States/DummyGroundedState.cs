using UnityEngine;

public class DummyGroundedState : DummyState
{
    private float timeTillFall;
    private float timerTillFall;

    private Vector3 fallingVectors = new();
    private float fallSpeed;
    private float gravity;
    private bool isFalling;

    public DummyGroundedState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) 
    { 
        IsRootState = true; 
    }

    public override void EnterState()
    {
        GroundedSetUp();
        InitializeSubState();
    }

    private void InitializeSubState()
    {
        SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Land));
    }

    public override void ExitState(){ }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (isFalling) return DummyStateMachine.EDummyState.Falling;
        return StateKey;
    }

    public override void UpdateState()
    {
        if (CurrentSubState != null) { if (CurrentSubState.StateKey != DummyStateMachine.EDummyState.Damaged) CombatManager.DepleteDamageBuildUp(Time.deltaTime * 8); }
        //Debug.Log(CombatManager.DamageBuildup + " is depleting: " + (CurrentSubState.StateKey != DummyStateMachine.EDummyState.Damaged));
        if (Context.IsGrounded) { timerTillFall = timeTillFall; return; }

        if (timerTillFall >= 0.0f)
        {
            timerTillFall -= Time.deltaTime;

            fallSpeed = Context.VerticalVelocity += gravity * Time.deltaTime;
            fallingVectors.y = fallSpeed;
            Utilities.Agent.Move(fallingVectors);
        }
        else
        {
            isFalling = true;
        }
    }

    private void GroundedSetUp()
    {
        gravity = Context.Gravity;
        timerTillFall = Context.FallTimeout;
        timeTillFall = timerTillFall;
        isFalling = false;
    }
}
