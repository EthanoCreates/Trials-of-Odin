using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public float DamageAmount { get; protected set; } = 1;
    public float StanceBreakPower { get; protected set; } = 1;
    public int AttackID { get; protected set; } = 0;
}

public enum DamageType
{
    Minor,
    Stagger,
    Stun,
    Knockback,
    Knockdown,
    Popup,
    Juggle,
    Finisher,
}