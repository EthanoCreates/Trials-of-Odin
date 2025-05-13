using System;
using UnityEngine;

public class DummyAnimationRequestor
{
    public event EventHandler OnAttack;
    public event EventHandler OnStun;
    public event EventHandler OnLoopingStun;
    public event EventHandler OnLoopingStunExit;
    public DummyAnimationRequestor()
    {
        GameInput.Instance.OnTesterInput += Instance_OnTesterInput;
    }

    private void Instance_OnTesterInput(object sender, System.EventArgs e) => OnAttack?.Invoke(this, EventArgs.Empty);
    public void AnimateStun() { OnStun?.Invoke(this, EventArgs.Empty); }
    public void AnimateLoopingStun() { OnLoopingStun?.Invoke(this, EventArgs.Empty); }
    public void AnimateLoopingStunExit() { OnLoopingStunExit?.Invoke(this, EventArgs.Empty); }
    
}
