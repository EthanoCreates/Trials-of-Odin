using System.Collections;
using TrailsFX;
using UnityEngine;

public class Spear : Weapon, IAimAttacks, IThrowable
{
    [SerializeField] private float spearSpeed;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private GameObject pickUpObject;
    [SerializeField] private TrailEffect trail;

    [SerializeField] private float penetrationThreshold = 10f; // Minimum speed for penetration
    [SerializeField] private float maxPenetrationDepth = 0.5f; // Maximum penetration depth
    [SerializeField] private float minPenetrationDepth = 0.1f; // Minimum penetration depth

    public bool isLodged = false;
    public bool trackCollision = false;

    public IAimAttacks.EAimAttackTypes HeavyAimAttackType => IAimAttacks.EAimAttackTypes.Throw;

    public IAimAttacks.EAimAttackTypes LightAimAttackType => IAimAttacks.EAimAttackTypes.Throw;

    public bool Recallable => false;

    public bool Released => false;

    private int ammo = 0;

    
    public void Recall()
    {
        throw new System.NotImplementedException();
    }

    public override void WeaponPickUp()
    {
        base.WeaponPickUp();

        trackCollision = false;
        isLodged = false;
        pickUpObject.GetComponent<BoxCollider>().enabled = false;
        GetComponent<BoxCollider>().enabled = true;

        rigidBody.isKinematic = false;
        ammo++;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        DisableDamageColliders(0);
        pickUpObject.SetActive(false);
        PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;
        localPlayerStateMachine.AnimationRequestor.OnAim += AnimationRequestor_OnAim;
        localPlayerStateMachine.AnimationRequestor.OnAimExit += AnimationRequestor_OnAimExit;
        DisableWeaponVFX();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (!trackCollision) return;
        if (isLodged) return;

        float speed = rigidBody.linearVelocity.magnitude;
        if (speed >= penetrationThreshold)
        {
            StartCoroutine(LodgeSpear(collision, speed));
        }
        else
        {
            StopCoroutine(nameof(ArcTipCoroutine));
        }
    }

    private void AnimationRequestor_OnAimExit(object sender, System.EventArgs e)
    {
        if(ammo == 0) return;
        base.WeaponToHand();
    }

    private void AnimationRequestor_OnAim(object sender, System.EventArgs e)
    {

    }

    public void Throw(bool isHeavyAimAttack)
    {
        if (isHeavyAimAttack) HeavyAimAttack();
        else LightAimAttack();
    }

    public void HeavyAimAttack()
    {
        Throw();
    }

    public void LightAimAttack()
    {
        Throw();
    }

    public void Throw()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
        foreach (BoxCollider damageCollider in DamageColliders)
        {
            damageCollider.enabled = true;
        }

        transform.SetParent(null);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint; targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000);

        Vector3 directionToTarget = (targetPoint - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToTarget);
        //transform.eulerAngles += new Vector3(90f, 0f, 0f);

        rigidBody.linearVelocity = directionToTarget * spearSpeed;
        ammo--;
        AttackID++;
        pickUpObject.SetActive(true);
        EnableWeaponVFX();
        rigidBody.constraints = RigidbodyConstraints.None;
        StartCoroutine(nameof(ArcTipCoroutine));
        trackCollision = true;
        rigidBody.centerOfMass = new Vector3(0f, 0f, 0.8f);
        pickUpObject.GetComponent<BoxCollider>().enabled = true;
    }

    private IEnumerator ArcTipCoroutine()
    {
        while (!isLodged)
        {
            if (rigidBody.linearVelocity.sqrMagnitude > 0.1f)
            {
                transform.up = rigidBody.linearVelocity.normalized;
            }
            yield return null;
        }
    }

    private IEnumerator LodgeSpear(Collision collision, float speed)
    {
        isLodged = true;

        // Disable Rigidbody dynamics
        rigidBody.isKinematic = true;
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        // Calculate penetration depth based on speed
        float penetrationDepth = Mathf.Lerp(minPenetrationDepth, maxPenetrationDepth, speed / penetrationThreshold);
        penetrationDepth = Mathf.Clamp(penetrationDepth, minPenetrationDepth, maxPenetrationDepth);

        // Move arrow slightly forward in the direction of the spear's forward direction
        Vector3 penetrationPosition = transform.position + transform.up * penetrationDepth;
        transform.position = penetrationPosition;

        // Parent the arrow to the object it collides with, if possible
        if (collision.transform.TryGetComponent(out Rigidbody hitRigidbody))
        {
            transform.SetParent(collision.transform);
        }

        // Lock all constraints to ensure the arrow stays lodge
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;

        // Stop the arc behavior
        StopCoroutine(nameof(ArcTipCoroutine));
        GetComponent<BoxCollider>().enabled = false;
        yield return null;
    }


    public override void EnableWeaponVFX()
    {
        trail.checkWorldPosition = true;
        trail.UpdateMaterialProperties();
        trail.Restart();
    }

    public override void DisableWeaponVFX()
    {
        trail.checkWorldPosition = false;
        trail.UpdateMaterialProperties();
        trail.Restart();
    }

    public int GetAmmoAmount()
    {
        return ammo;
    }

    public bool IsReloaded()
    {
        //later upgrading needed
        return true;
    }
}
