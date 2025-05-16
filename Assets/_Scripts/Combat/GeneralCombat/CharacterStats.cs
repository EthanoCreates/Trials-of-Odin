using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    public PlayerHealth Health { get { return health; } }

    [SerializeField] private PlayerStamina stamina;
    public PlayerStamina Stamina { get { return stamina; } }

    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float SprintSpeed { get; set; }
    public float SprintTime { get; set; }
    public float JumpHeight { get; set; }
    public LayerMask GroundLayers { get; set; }
}