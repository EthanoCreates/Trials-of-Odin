using UnityEngine;

namespace TrialsOfOdin.Stats
{
    public abstract class CharacterStats : MonoBehaviour
    {
        [SerializeField] private CharacterProfile character = new();
        public CharacterProfile Character { get { return character; } }

        [SerializeField] private Health health;
        public Health Health { get { return health; } }

        [SerializeField] private Stamina stamina;
        public Stamina Stamina { get { return stamina; } }

        public float WalkSpeed { get; set; }
        public float RunSpeed { get; set; }
        public float SprintSpeed { get; set; }
        public float SprintTime { get; set; }
        public float JumpHeight { get; set; }
        public LayerMask GroundLayers { get; set; }
    }
}