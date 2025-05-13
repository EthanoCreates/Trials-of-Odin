using UnityEngine;

[CreateAssetMenu(menuName = "EnemySOs/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public float maxHealth = 100;
    public float stanceStrength = 100;

    public float walkSpeed = 2;
    public float runSpeed = 4;
    public LayerMask groundLayers;
    public LayerMask DamagingWeaponLayers;

    public float ImpactPower = 1f;
    public float ImpactDuration = 0.1f;

    public float DeathImpactPower = 1f;
    public float DeathImpactDuration = 0.1f;
}
