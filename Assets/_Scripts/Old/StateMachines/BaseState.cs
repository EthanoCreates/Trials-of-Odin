using System;
public abstract class BaseState<EState> where EState : Enum
{
    public BaseState<EState> CurrentSuperState { get; private set; }
    public BaseState<EState> CurrentSubState { get; private set; }
    public bool IsRootState { get; protected set; } = false;
    public BaseState (EState key)
    {
        StateKey = key;
    }
    public EState StateKey { get; private set; }

    public abstract void EnterState();
    public abstract EState GetNextState();
    public abstract void UpdateState();
    public abstract void UpdateSubState();
    public abstract void ExitState();

    /// <summary>
    /// Updating state, this is the update method that each state has. 
    /// If state has a substate method we pass it in to the state machine manager.
    /// This is done by e.g. PlayerStateMachine.LocalInstance.CurrentState(this);
    /// </summary>
    public void UpdateStates()
    {
        UpdateState();
        //null check with ?
        CurrentSubState?.UpdateSubState();
    }

    /// <summary>
    /// Exiting current state and all deeper substates through recursion
    /// </summary>
    public void ExitStates()
    {
        ExitState();
        CurrentSubState?.ExitStates();
        CurrentSubState = null;
    }

    protected void RemoveSubState()
    {
        if (CurrentSubState == null) return;

        CurrentSubState.CurrentSuperState = null;
        CurrentSubState = null;
    }

    protected void SetSuperState(BaseState<EState> newSuperState)
    {
        CurrentSuperState = newSuperState;
    }

    public void SetSubState(BaseState<EState> newSubState)
    {
        CurrentSubState?.ExitStates();
        CurrentSubState = newSubState;
        //child to parent link
        CurrentSubState.SetSuperState(this);
        CurrentSubState.EnterState();
    }
}
