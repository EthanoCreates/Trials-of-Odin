using UnityEngine;

public abstract class HumanoidSMContext
{
    // Common References
    public float Gravity { get; private set; } = -9.81f;
    public Transform Transform { get; protected set; }
    public float FallGravityMultiplier { get; protected set; } = 2f;
    public float FallTimeout { get; protected set; } = 0.5f;
    public bool AnimFinished { get; protected set; } = true;

    //Movement
    public float WalkSpeed { get; protected set; }
    public float RunSpeed { get; protected set; }
    public bool IsJumping { get; set; }
    public bool IsDodging { get; set; }
    public float JumpHeight { get; protected set; }
    public bool Landing { get; set; } = false;
    public float TerminalVelocity { get; private set; } = 53.0f;
    public float JumpTimeout { get; private set; } = .5f;

    // Grounded
    public bool IsGrounded { get { return CheckIfGrounded(); } }
    public LayerMask GroundLayers { get; protected set; }
    public abstract bool CheckIfGrounded();
     
    public void AnimationFinished()
    {
        AnimFinished = true;
    }

    public void AnimationStarted()
    {
        AnimFinished = false;
    }
}
