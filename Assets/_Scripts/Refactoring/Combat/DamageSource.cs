using System.Collections.Generic;
using TrialsOfOdin;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public float DamageDealt { get; protected set; } = 1;
    public float StanceBreakPower { get; protected set; } = 1;
    public int AttackID { get; protected set; } = 0;
    public HashSet<CharacterProfile> charactersHit = new();
    public HashSet<CharacterProfile> blockedCharactersHit = new();
    public virtual void HitCharcter(Collision collision) { }
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