using System;
using System.Collections;
using TrailsFX;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    //assigned box collider and altered rigidbody
    [SerializeField] private BoxCollider arrowCollider;
    [SerializeField] private Rigidbody arrowRigidbody;
    [SerializeField] private TrailEffect trail;

    [SerializeField] private float penetrationThreshold = 10f; // Minimum speed for penetration
    [SerializeField] private float maxPenetrationDepth = 0.5f; // Maximum penetration depth
    [SerializeField] private float minPenetrationDepth = 0.1f; // Minimum penetration depth

    public event EventHandler<CollisionEventArgs> OnCollision;
    public event EventHandler<OnDeleteEventArgs> OnDelete;

    private bool isLodged = false;
    bool trackCollision = false;

    //passing collision to allow bow script to handle it
    public class CollisionEventArgs
    {
        public Collision collision;
    }

    private void Start()
    {
        DisableArrow();
    }

    private void DisableArrow()
    {
        arrowCollider.enabled = false;
        arrowRigidbody.useGravity = false;
        arrowRigidbody.isKinematic = true;

        trail.checkWorldPosition = false;
        trail.UpdateMaterialProperties();
        trail.Restart();
    }

    private void EnableArrow()
    {
        arrowCollider.enabled = true;
        arrowRigidbody.useGravity = true;
        arrowRigidbody.isKinematic = false;

        trail.checkWorldPosition = true;
        trail.UpdateMaterialProperties();
        trail.Restart();
    }

    /// <summary>
    /// Allowing the bow to handle collision through event
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(this, new CollisionEventArgs
        {
            collision = collision,
        });

        if (!trackCollision) return;
        if (isLodged) return;

        float speed = arrowRigidbody.linearVelocity.magnitude;
        if (speed >= penetrationThreshold)
        {
            StartCoroutine(LodgeArrow(collision, speed));
        }
        else
        {
            StopCoroutine(nameof(ArcTipCoroutine));
        }
    }

    private IEnumerator ArcTipCoroutine()
    {
        while (!isLodged)
        {
            if (arrowRigidbody.linearVelocity.sqrMagnitude > 0.1f)
            {
                transform.forward = arrowRigidbody.linearVelocity.normalized;
            }
            yield return null;
        }
    }

    private IEnumerator LodgeArrow(Collision collision, float speed)
    {
        isLodged = true;

        // Disable Rigidbody dynamics
        arrowRigidbody.isKinematic = true;
        arrowRigidbody.linearVelocity = Vector3.zero;
        arrowRigidbody.angularVelocity = Vector3.zero;

        // Calculate penetration depth based on speed
        float penetrationDepth = Mathf.Lerp(minPenetrationDepth, maxPenetrationDepth, speed / penetrationThreshold);
        penetrationDepth = Mathf.Clamp(penetrationDepth, minPenetrationDepth, maxPenetrationDepth);

        // Move arrow slightly forward in the direction of the arrow's "up" direction
        Vector3 penetrationPosition = transform.position + transform.forward * penetrationDepth;
        transform.position = penetrationPosition;

        // Parent the arrow to the object it collides with, if possible
        if (collision.transform.TryGetComponent(out Rigidbody hitRigidbody))
        {
            transform.SetParent(collision.transform);
        }

        // Lock all constraints to ensure the arrow stays lodged
        arrowRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // Stop the arc behavior
        StopCoroutine(nameof(ArcTipCoroutine));
        GetComponent<BoxCollider>().enabled = false;
        yield return null;
    }

    public void Shoot(float bowShootingSpeed)
    {
        EnableArrow();

        //arrowmesh in arrow, so we are setting arrows parent to null
        transform.SetParent(null);

        ApplyVelocityAndOrientArrow(bowShootingSpeed);

        EnableRotation();
        //Invoke(nameof(EnableRotation), .0002f * bowShootingSpeed);
        //cleaning up arrow by destroying after 10 seconds
        Invoke(nameof(DestroyArrow), 10f);
    }

    private void EnableRotation()
    {
        arrowRigidbody.constraints = RigidbodyConstraints.None;
        StartCoroutine(nameof(ArcTipCoroutine));
        trackCollision = true;
        arrowRigidbody.centerOfMass = new Vector3(0f, .5f, 0f);
        //creating arced arrow
    }

    private void ApplyVelocityAndOrientArrow(float bowShootingSpeed)
    {
        //this can be broken if another camera is set to main causing arrow to go off in the wrong direction
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000);

        Vector3 directionToTarget = (targetPoint - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        arrowRigidbody.linearVelocity = directionToTarget * bowShootingSpeed;
    }

    private void DestroyArrow()
    {
        OnDelete?.Invoke(this, new OnDeleteEventArgs
        {
            arrowToUnsubscribeFrom = this
        });
        Destroy(transform.gameObject);
    }

    public class OnDeleteEventArgs
    {
        public Arrow arrowToUnsubscribeFrom;
    }
}
