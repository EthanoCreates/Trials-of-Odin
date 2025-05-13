using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    public PlayerHealth Health { get { return health; } }
    [SerializeField] private PlayerStamina stamina;
    public PlayerStamina Stamina { get { return stamina; } }
}
