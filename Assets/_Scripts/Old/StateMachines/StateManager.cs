using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using Unity.Netcode;

public abstract class StateManager<EState> : NetworkBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    public BaseState<EState> currentState { get; protected set; }

    protected bool isTransitioningState = false;

    public void CurrentState(BaseState<EState> state)
    {
        EState nextStateKey = state.GetNextState();
        BaseState<EState> nextState = States[nextStateKey];

        if (isTransitioningState) return;
        
        if (nextStateKey.Equals(state.StateKey))
        {
            state.UpdateStates();
        }
        else
        {
            if (nextState.IsRootState)
            {
                TransitionToState(nextStateKey);
            }
            else if (state.CurrentSuperState != null)
            {
                state.CurrentSuperState.SetSubState(nextState);
            }
        }
    }

    public BaseState<EState> GetStateInstance(EState state)
    {
        return States[state];
    }

    private void TransitionToState(EState stateKey)
    {
        isTransitioningState = true;
        currentState.ExitStates();
        currentState = States[stateKey];
        currentState.EnterState();
        isTransitioningState = false;
    }
}
