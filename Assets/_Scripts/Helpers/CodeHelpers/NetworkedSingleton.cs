using Unity.Netcode;
using UnityEngine;

public class NetworkedSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance { get; private set; }
    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }
}
