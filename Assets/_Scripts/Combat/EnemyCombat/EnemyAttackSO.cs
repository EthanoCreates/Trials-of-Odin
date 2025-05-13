using UnityEngine;

[CreateAssetMenu(menuName = "EnemySOs/EnemyAttackSO")]
public class EnemyAttackSO : ScriptableObject
{
    public enum AttackClipType
    { 
        Attack1,
        Attack2,
        Attack3,
        Attack4,
        SpecialAttack1,
        SpecialAttack2,
        SpecialAttack3,
        AttackCombo
    }

    public AttackClipType attackClipType;
    public float damageMultiplier;
}
