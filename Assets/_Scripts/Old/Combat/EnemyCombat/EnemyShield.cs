using System;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{


    public event EventHandler OnBlocked;

    [SerializeField] private BoxCollider shieldCollider;

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
