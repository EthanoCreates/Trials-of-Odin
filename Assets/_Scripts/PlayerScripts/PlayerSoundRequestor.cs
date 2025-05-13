using System;
using UnityEngine;

public class PlayerSoundRequestor
{
    public event EventHandler OnJumpSound;
    public event EventHandler OnLandSound;
    public event EventHandler OnRollLand;
    public event EventHandler OnHurt;

    public PlayerSoundRequestor()
    {
        PlayerStateMachine.LocalInstance.PlayerAnimationEvents.OnRollLand += (object sender, EventArgs e) 
            => OnRollLand?.Invoke(this, EventArgs.Empty);
    }

    public void PlayJumpSound() => OnJumpSound?.Invoke(this, EventArgs.Empty);
    public void PlayLandSound() => OnLandSound?.Invoke(this, EventArgs.Empty);
    public void PlayHurtSound() => OnHurt?.Invoke(this, EventArgs.Empty);
}
