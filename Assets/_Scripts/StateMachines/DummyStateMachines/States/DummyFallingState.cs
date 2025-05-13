using System;
using UnityEngine;

public class DummyFallingState : DummyState
{
    private float gravity;
    private float gravityMultiplier;
    private Vector3 fallingVectors = new();
    private float fallSpeed;

    public DummyFallingState(DummyUtilities utilities, DummyStateMachine.EDummyState key) : base(utilities, key) { }

    public override void EnterState()
    {
        FallingSetUp();
        InitializeSubState();
    }

    private void InitializeSubState()
    {
        SetSubState(Utilities.StateMachine.GetStateInstance(DummyStateMachine.EDummyState.Falling));
    }

    public override void ExitState(){ }

    public override DummyStateMachine.EDummyState GetNextState()
    {
        if (Context.IsGrounded) return DummyStateMachine.EDummyState.Grounded;

        return StateKey;
    }

    public override void UpdateState()
    {
        Debug.Log("GHeeke");
        fallSpeed = Context.VerticalVelocity += gravity * Time.deltaTime * gravityMultiplier;
        fallingVectors.y = fallSpeed;
        Utilities.Agent.Move(fallingVectors);
    }

    private void FallingSetUp()
    {
        gravity = Context.Gravity;
        gravityMultiplier = Context.FallGravityMultiplier;
        //enemyAnimatorRequestor.AnimateFalling();
    }
}
