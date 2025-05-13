using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Rigidbody> ragdollRigidbodies;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody hips;

    private void Start()
    {
        DeactivateRagdoll();
    }

    public void ActivateRagdoll()
    {
        foreach(Rigidbody rigidBody in ragdollRigidbodies)
        {
            rigidBody.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void DeactivateRagdoll()
    {
        foreach (Rigidbody rigidBody in ragdollRigidbodies)
        {
            rigidBody.isKinematic = true;
        }
        animator.enabled = true;
    }

    public void ApplyForce(Vector3 force)
    { 
        hips.AddForce(force, ForceMode.VelocityChange);
    }
}
