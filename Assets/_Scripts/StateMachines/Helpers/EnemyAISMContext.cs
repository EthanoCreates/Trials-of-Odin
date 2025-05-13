using UnityEngine;

public class EnemyAISMContext : HumanoidSMContext
{
    public float StoppingDistance { get; protected set; }
    public float GroundedOffset { get; protected set; } = -0.14f;
    public float GroundCheckRadius { get; protected set; } = 1f;

    public override bool CheckIfGrounded()
    {
        Vector3 spherePosition = Transform.position;
        spherePosition.y -= GroundedOffset;
        return Physics.CheckSphere(spherePosition, GroundCheckRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
}
