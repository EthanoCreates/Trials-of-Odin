using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private EnemyWeaponSO enemyWeaponData;
    [SerializeField] private List<EnemyAttackSO> enemyAttacks;
    [SerializeField] private List<EnemyAttackSO> specialEnemyAttacks;
    [SerializeField] private List<EnemyAttackSO> enemyCombos;

    public EnemyWeaponSO EnemyWeaponData { get { return enemyWeaponData; } }
    public List<EnemyAttackSO> EnemyAttacks { get { return enemyAttacks; } }
    public List<EnemyAttackSO> SpecialEnemyAttacks { get { return specialEnemyAttacks; } }
    public List<EnemyAttackSO> EnemyCombos { get { return enemyCombos; } }


    public abstract void SetDamageMultiplier(float multiplier);
    public abstract void EnableDamageColliders();
    public abstract void DisableDamageColliders();
}
