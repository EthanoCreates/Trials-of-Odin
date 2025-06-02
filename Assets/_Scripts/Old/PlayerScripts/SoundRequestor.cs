using System;
using TrialsOfOdin.State;
using UnityEngine;

public class SoundRequestor
{
    public event Action OnJumpSound;
    public event Action OnLandSound;
    public event Action OnRollLand;
    public event Action OnHurt;

    public SoundRequestor()
    {
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnRollLand += () => OnRollLand?.Invoke();
    }

    public void PlayJumpSound() => OnJumpSound?.Invoke();
    public void PlayLandSound() => OnLandSound?.Invoke();
    public void PlayHurtSound() => OnHurt?.Invoke();
}
