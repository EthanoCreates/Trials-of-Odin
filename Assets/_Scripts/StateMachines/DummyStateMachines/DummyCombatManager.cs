using FIMSpace.FProceduralAnimation;
using Kinemation.MotionWarping.Runtime.Examples;
using TrialsOfOdin.Stats;
using UnityEngine;

public class DummyCombatManager : HumanoidCombatManager
{
    RagdollHandler ragdoll;
    Rigidbody lastLimbHit;
    private Vector3 impactDirection;
    public bool IsDamaged { get; set; }
    public DamageType DamageType { get; set; }
    public float StanceBreakBuildUp { get; set; }
    public bool IsFallen { get; set; }
    
    Health health;
    EnemyDataSO dummyData;
    EnemyVFX enemyVFX;
    EnemyUI enemyUI;
    AlignComponent finisherAlign;

    public DummyCombatManager(Health health, EnemyDataSO dummyData, EnemyVFX enemyVFX, EnemyUI enemyUI, AlignComponent finisherAlign)
    {
        this.health = health;
        this.dummyData = dummyData;
        this.enemyVFX = enemyVFX;
        this.enemyUI = enemyUI;
        this.finisherAlign = finisherAlign;
    }

    private int currentAttackID;
    public void OnRagdollCollision(RA2BoneCollisionHandler hit, Collision collision)
    {
        GameObject collisionObject = collision.gameObject;

        DamageSource damageData = collisionObject.GetComponent<DamageSource>();

        if (damageData == null) return;
        if (currentAttackID == damageData.AttackID) return;

        currentAttackID = damageData.AttackID;

        if(ragdoll == null) ragdoll = hit.ParentHandler;

        impactDirection = collision.relativeVelocity.normalized;

        lastLimbHit = hit.DummyBoneRigidbody;

        Damaged(damageData.DamageAmount, (hit.ParentChain.ChainType == ERagdollChainType.LeftLeg
                || hit.ParentChain.ChainType == ERagdollChainType.RightLeg), damageData.StanceBreakPower);
    }

    public void ApplyDeathForceAndSleep()
    {
        PlayerStateMachine.LocalInstance.PlayerVFX.PlayImpactFeedBack(PlayerVFX.EImpactVFXs.HeavyImpact);
        enemyVFX.PlayImpactFeedBack(EnemyVFX.EImpactVFXs.HeavyImpact);
        ApplyForceToRagdollAndLimb(dummyData.DeathImpactPower, dummyData.DeathImpactDuration);
        ragdoll.AnimatingMode = RagdollHandler.EAnimatingMode.Sleep;
    }

    public void MakeDummyFall()
    {
        ragdoll.User_SwitchFallState(RagdollHandler.EAnimatingMode.Falling);
        IsFallen = true;
    }

    public void ApplyForceToRagdollAndLimb()
    {
        ragdoll.User_AddAllBonesImpact(impactDirection * dummyData.ImpactPower, dummyData.ImpactDuration, ForceMode.Acceleration);
        ragdoll.User_AddRigidbodyImpact(lastLimbHit, impactDirection * dummyData.ImpactPower, dummyData.ImpactDuration, ForceMode.VelocityChange);
    }

    public void ApplyForceToRagdollAndLimb(float impactPower, float impactDuration)
    {
        ragdoll.User_AddAllBonesImpact(impactDirection * impactPower, impactDuration, ForceMode.Acceleration);
        ragdoll.User_AddRigidbodyImpact(lastLimbHit, impactDirection * impactPower, impactDuration, ForceMode.VelocityChange);
    }

    public void KnockDown()
    {
        ragdoll.User_AddRigidbodyImpact(ragdoll.GetChain(ERagdollChainType.LeftLeg).LastBone.GameRigidbody, 
            -health.transform.forward * dummyData.ImpactPower, dummyData.ImpactDuration, ForceMode.VelocityChange);

        ragdoll.User_AddRigidbodyImpact(ragdoll.GetChain(ERagdollChainType.RightLeg).LastBone.GameRigidbody,
            -health.transform.forward * dummyData.ImpactPower, dummyData.ImpactDuration, ForceMode.VelocityChange);
    }

    public void PopUp()
    {
        ragdoll.User_AddAllBonesImpact(health.transform.up * dummyData.ImpactPower/20, .2f, ForceMode.VelocityChange);
    }

    public void Damaged(float amount, bool isLegHit, float stanceBreakPower)
    {
        if (!beingFinishered) health.TakeDamage(amount);

        StanceBreakBuildUp += stanceBreakPower;
        DamageType = CalculateDamageState(stanceBreakPower, impactDirection, isLegHit);

        if (DamageType == DamageType.Minor || DamageType == DamageType.Stagger || DamageType == DamageType.Stun)
        {
            PlayerStateMachine.LocalInstance.PlayerVFX.PlayImpactFeedBack(PlayerVFX.EImpactVFXs.LightImpact);
            enemyVFX.PlayImpactFeedBack(EnemyVFX.EImpactVFXs.LightImpact);
        }
        else
        {
            PlayerStateMachine.LocalInstance.PlayerVFX.PlayImpactFeedBack(PlayerVFX.EImpactVFXs.HeavyImpact);
            enemyVFX.PlayImpactFeedBack(EnemyVFX.EImpactVFXs.HeavyImpact);
        }
        IsDamaged = true;
        
    }

    public void DepleteDamageBuildUp(float amountRestored)
    {
        if(StanceBreakBuildUp > 0) StanceBreakBuildUp -= amountRestored;
    }

    public DamageType CalculateDamageState(float stanceBreakPower, Vector3 impactDirection, bool isLegHit)
    {
        Debug.Log("Hello");
        float stanceStrength = dummyData.stanceStrength;

        if (DamageType == DamageType.Popup & IsDamaged)
        { 
            //gravity reduction would be nice?
            PopUp();
            return DamageType.Popup;
        }

        if (StanceBreakBuildUp > stanceStrength)
        {
            if (health.GetCurrentHealth() < dummyData.maxHealth / 3)
            {
                return DamageType.Finisher;
            }
            else
            {
                return HeavyImpactState(impactDirection, isLegHit);
            }

        }
        else if (StanceBreakBuildUp > stanceStrength / 2)
        {
            return HeavyImpactState(impactDirection, isLegHit);
        }
        else if (StanceBreakBuildUp > stanceStrength / 4)
        {

            return DamageType.Stagger;
        }
        else 
        { 
            return DamageType.Minor;
        }

    }

    public void Kill()
    {
        Time.timeScale = 1f;
        health.TakeDamage(health.GetCurrentHealth());
    }


    public void FinisherAvailable(float finisherTimer)
    {
        enemyUI.StartFinisherTimer(finisherTimer);
    }

    private DamageType HeavyImpactState(Vector3 impactDirection, bool isLegHit)
    {
        if (impactDirection.normalized.y < -0.7) return DamageType.Popup;
        if (isLegHit) return DamageType.Knockdown;
        if(StanceBreakBuildUp > dummyData.stanceStrength) return DamageType.Knockback;
        return DamageType.Stun;      
    }

    public override void Finisher(GameObject player)
    {
        finisherAlign.Interact(finisherAlign.transform.parent.gameObject);
        beingFinishered = true;
    }
}
