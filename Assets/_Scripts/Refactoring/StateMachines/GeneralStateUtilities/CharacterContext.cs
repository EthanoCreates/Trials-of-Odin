using TrialsOfOdin.Stats;
using UnityEngine;

public abstract class CharacterContext
{
    public CharacterContext(CharacterStats characterStats) 
    {
        WalkSpeed = characterStats.WalkSpeed;
        RunSpeed = characterStats.RunSpeed;
        SprintSpeed = characterStats.SprintSpeed;
        SprintTime = characterStats.SprintTime;
        JumpHeight = characterStats.JumpHeight;
    }

    protected CharacterStats humanoidStats;

    // Common References
    public float Gravity { get; private set; } = -9.81f;
    public Transform Transform { get; protected set; }
    public float FallGravityMultiplier { get; protected set; } = 2f;
    public float FallTimeout { get; protected set; } = 0.5f;
    public bool AnimFinished { get; protected set; } = true;
    public bool IsDamaged { get; set; }

    //Movement
    public float WalkSpeed { get; }
    public float RunSpeed { get; }
    public float SprintSpeed { get ;}
    public float SprintTimer { get; set; }
    public float SprintTime { get ;}
    public bool IsJumping { get; set;}
    public bool IsDodging { get; set; }
    public float JumpHeight { get ;}
    public bool Landing { get; set; } = false;
    public float TerminalVelocity { get; private set; } = 53.0f;
    public float JumpTimeout { get; private set; } = .5f;

    // Grounded
    public bool IsGrounded { get { return CheckIfGrounded(); } }
    public LayerMask GroundLayers { get => humanoidStats.GroundLayers;}
    public abstract bool CheckIfGrounded();

    //Combat
    public bool IsAttacking { get; set;}
    public abstract bool IsCharging { get; }
     
    public void AnimationFinished() => AnimFinished = true;

    public void AnimationStarted() => AnimFinished = false;
}
