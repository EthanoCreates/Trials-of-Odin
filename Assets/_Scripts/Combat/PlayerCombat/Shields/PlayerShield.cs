using System;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public event EventHandler OnBlocked;

    [SerializeField] private BoxCollider shieldCollider;
    public AnimatorOverrideController overrideController;

    private void Start()
    {
        DisableShield();
    }

    public void EnableShield()
    {
        shieldCollider.enabled = true;
    }

    public void DisableShield()
    {
        shieldCollider.enabled = false;
    }

    public void ShieldBlocked()
    {
        OnBlocked?.Invoke(this, EventArgs.Empty);
    }
}
