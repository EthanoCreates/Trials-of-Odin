using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private bool temporary;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerUtilities health = other.GetComponent<PlayerStateMachine>().Utilities;
        int attackID = health.GetAttackID();

        if (health != null) health.TakeDamage(damage, this.transform.position, attackID++, null);
    }

    private void Start()
    {
        if (temporary)
        {
            Invoke(nameof(DeactivateCollider), .1f);
            Invoke(nameof(Destroy), GetComponentInChildren<ParticleSystem>().main.duration);
        }
    }

    private void DeactivateCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    private void Destroy()
    {
       Destroy(gameObject);
    }
}
