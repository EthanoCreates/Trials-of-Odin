using System;
using UnityEngine;
using TrialsOfOdin.Combat;
using RootMotion.FinalIK;
public class PlayerUtilities
{
    public event EventHandler<OnDamageBlockedEventArgs> OnDamageBlocked;

    private readonly PlayerStats stats;
    public PlayerContext Context { get; private set; }
    public PlayerMovementUtility MovementUtility { get; private set; }
    public PlayerCombatManager CombatManager { get; private set; }
    public PlayerAnimationRequestor AnimationRequestor { get; private set; }
    public PlayerSoundRequestor AudioRequestor { get; private set; }

    public PlayerUtilities(PlayerStats stats, ArmIK armIK)
    {
        Context = new PlayerContext(stats);
        CombatManager = new PlayerCombatManager(stats, armIK);
        MovementUtility = new PlayerMovementUtility();
        AnimationRequestor = new PlayerAnimationRequestor(CombatManager);
        AudioRequestor = new PlayerSoundRequestor();
        this.stats = stats;

    }

    public void TakeDamage(float damageAmount, Vector3 collisionPosition, int attackID, HealthForRagdolls enemyHealth)
    {
        if (attackID == GetAttackID()) return; //ensuring only 1 hit gets registered on attack
        Transform transform = MovementUtility.Transform;

        if (CombatManager.IsBlocking)
        {
            Vector3 directionToCollision = (collisionPosition - transform.position).normalized;
            directionToCollision.y = 0;

            if (Vector3.Angle(transform.forward, directionToCollision) <= 45)
            {
                OnDamageBlocked?.Invoke(this, new OnDamageBlockedEventArgs { enemyHealth = enemyHealth, collisionPosition = collisionPosition });
                return;
            }
        }

        Vector3 directionToHitInWorld = Direction(transform.position, collisionPosition); // Parameter 1 = Target, Parameter 2 = Thing doing the hitting
        Vector3 forceDirection = transform.InverseTransformDirection(directionToHitInWorld);

        stats.Health.TakeDamage(damageAmount, attackID, forceDirection);

        Context.IsDamaged = true;
        AnimationRequestor.AnimateImpact(forceDirection.x, forceDirection.z);
        AudioRequestor.PlayHurtSound();
    }
    private Vector3 Direction(Vector3 fromPosition, Vector3 toPosition) => (toPosition - fromPosition).normalized;

    public int GetAttackID () => stats.Health.currentAttackID;
    public void RecoverStamina()
    {
        stats.Stamina.RecoverStamina();
    }
    public bool TrySprintWithStamina()
    {
        return stats.Stamina.CanSprint();
    }

    public bool HasSufficientStamina(float staminaCost)
    {
        return stats.Stamina.HasSufficientStamina(staminaCost);
    }

    public bool CouldPerformActionWithStamina(float staminaCost)
    {
        return stats.Stamina.CouldPerformActionWithStamina(staminaCost);
    }

    public class OnDamageBlockedEventArgs
    {
        public HealthForRagdolls enemyHealth;
        public Vector3 collisionPosition;
    }
}
