using System;
using UnityEngine;

public class EnemyAnimatorRequestor
{
    public event EventHandler<AttackEventArgs> OnAttack;
    public event EventHandler<AttackEventArgs> OnSpecialAttack;
    public event EventHandler<AttackEventArgs> OnAttackCombo;
    public event EventHandler OnIdle;
    public event EventHandler OnReEngage;
    public event EventHandler OnIdleBreak;
    public event EventHandler<OnMoveEventArgs> OnMove;
    public event EventHandler OnCharge;
    public event EventHandler OnBlock;
    public event EventHandler OnSuccessfulBlock;
    public event EventHandler<OnImpactEventArgs> OnImpact;
    public event EventHandler OnExitBlock;
    public event EventHandler OnFalling;
    public event EventHandler OnLanding;
    public event EventHandler OnWakeUp;

    private EnemyContext enemyContext;

    private EnemyWeapon PrimaryWeapon;

    public float ImpactPosX { get; set; }
    public float ImpactPosZ { get; set; }

    public EnemyAnimatorRequestor(EnemyContext enemyContext)
    {
        this.enemyContext = enemyContext;
        enemyContext.Shield.OnBlocked += Shield_OnBlocked;
        PrimaryWeapon = enemyContext.PrimaryWeapon;
    }

    private void Shield_OnBlocked(object sender, EventArgs e)
    {
        AnimateBlocking();
    }



    public void AnimateBlocking()
    {
        OnSuccessfulBlock?.Invoke(this, EventArgs.Empty);
    }

    public void ReEngage()
    {
        OnReEngage?.Invoke(this, EventArgs.Empty);
    }

   

    public void AnimateIdle()
    {
        OnIdle?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateIdleBreak()
    {
        OnIdleBreak?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateFalling()
    {
        OnFalling?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateLanding()
    {
        OnLanding?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateMovement(float speed)
    {
        OnMove?.Invoke(this, new OnMoveEventArgs
        {
            speed = speed
        });
    }

    public void AnimateBlock()
    {
        OnBlock?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateAttack(int attackNum)
    {
        EnemyAttackSO attack = PrimaryWeapon.EnemyAttacks[attackNum];
        PrimaryWeapon.SetDamageMultiplier(attack.damageMultiplier);
        OnAttack?.Invoke(this, new AttackEventArgs { attackType = attack });
    }

    public void AnimateSpecialAttack(int attackNum)
    {
        EnemyAttackSO attack = PrimaryWeapon.SpecialEnemyAttacks[attackNum];
        PrimaryWeapon.SetDamageMultiplier(attack.damageMultiplier);
        OnSpecialAttack?.Invoke(this, new AttackEventArgs { attackType = attack });
    }

    public void AnimateAttackCombo(int attackNum)
    {
        EnemyAttackSO attack = PrimaryWeapon.EnemyCombos[attackNum];
        PrimaryWeapon.SetDamageMultiplier(attack.damageMultiplier);
        OnAttackCombo?.Invoke(this, new AttackEventArgs { attackType = attack });
    }

    public void AnimateImpact(float xForce, float zForce, Vector3 suspectedPosition)
    {
        ImpactPosX = xForce;
        ImpactPosZ = zForce;
        enemyContext.suspectedPos = suspectedPosition;

        OnImpact?.Invoke(this, new OnImpactEventArgs
        {
            impactX = xForce,
            impactZ = zForce
        });
        enemyContext.IsDamaged = true;
    }
    public void AnimateExitBlock()
    {
        OnExitBlock?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateWakeUp()
    {
        OnWakeUp?.Invoke(this, EventArgs.Empty);
    }

    public void AnimateCharge()
    {
        OnCharge?.Invoke(this, EventArgs.Empty);
    }

    public class AttackEventArgs
    {
        public EnemyAttackSO attackType;
    }

    public class OnMoveEventArgs
    {
        public float speed;
    }

    public class OnImpactEventArgs
    {
        public float impactX;
        public float impactZ;
    }
}
