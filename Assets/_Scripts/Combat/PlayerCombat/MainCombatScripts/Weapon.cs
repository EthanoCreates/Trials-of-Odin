using Ami.BroAudio;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Weapon : DamageSource
{
    [SerializeField] private WeaponSO weaponData;
    [SerializeField] private List<BoxCollider> damageColliders;


    [FoldoutGroup("Position & Rotation")]
    public bool overrideHandPosition;

    [FoldoutGroup("Position & Rotation")]
    [ShowIf(nameof(overrideHandPosition))]
    public Vector3 handPosition;

    [FoldoutGroup("Position & Rotation")]
    public bool overrideHandRotation;

    [FoldoutGroup("Position & Rotation")]
    [ShowIf(nameof(overrideHandRotation))]
    public Vector3 handRotation;

    [FoldoutGroup("Position & Rotation")]
    public bool overrideHolsterPosition;

    [FoldoutGroup("Position & Rotation")]
    [ShowIf(nameof(overrideHolsterPosition))]
    public Vector3 holsterPosition;

    [FoldoutGroup("Position & Rotation")]
    public bool overrideHolsterRotation;

    [FoldoutGroup("Position & Rotation")]
    [ShowIf(nameof(overrideHolsterRotation))]
    public Vector3 holsterRotation;

    public WeaponSO WeaponData { get { return weaponData; } }

    #region Attack_Getters
    public List<AttackSO> LightCombo{ get { return weaponData.LightCombos; } }
    public List<AttackSO> HeavyCombo { get {return weaponData.HeavyCombos; } }
    public AttackSO LightChargedAttack { get { return weaponData.LightChargedAttack; } }
    public AttackSO HeavyChargedAttack {get { return weaponData.HeavyChargedAttack; } }
    public AttackSO LightSprintAttack { get {return weaponData.LightSprintAttack; } }
    public AttackSO HeavySprintAttack { get {return weaponData.HeavySprintAttack; } }
    public AttackSO LightAerialAttack { get {return weaponData.LightAerialAttack; } }
    public AttackSO HeavyAerialAttack { get {return weaponData.HeavyAerialAttack; } }
    public AttackSO LightAimAttack { get {return weaponData.LightAimAttack; } }
    public AttackSO HeavyAimAttack { get  {return weaponData.HeavyAimAttack; } }
    public List<AttackSO> RecoveryAttacks { get {return weaponData.RecoveryAttacks; } }
    public AttackSO FinisherAttack { get { return weaponData.FinisherAttack; } }
    #endregion
    public List<BoxCollider> DamageColliders { get { return damageColliders; } protected set { damageColliders = value; } }

    protected Transform Wielder;
    public Transform Hand { get; set; }
    public Transform Holster { get; set; }
    public WeaponHolster WeaponHolster { get; private set; }

    [ReadOnly] public AttackSO currentAttack;
    private int EffectID; 
    public int BlockedAttackID { get; protected set; } = -1;
    protected float damageMultiplier = 1;

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }

    /// <summary>
    /// This is where the weapon is set up and everything is initialized.
    /// </summary>
    public virtual void WeaponPickUp()
    {
        SetUpHelper();
        FillHolsterslot();
    }
    public virtual void WeaponToHolster()
    {
        transform.parent = Holster;
        transform.localPosition = overrideHolsterPosition ? holsterPosition : Vector3.zero;
        transform.localEulerAngles = overrideHolsterRotation ? holsterRotation : Vector3.zero;
    }

    public virtual void WeaponToHand()
    {
        transform.parent = Hand;
        transform.localPosition = overrideHandPosition ? handPosition : Vector3.zero;
        transform.localEulerAngles = overrideHandRotation ? handRotation : Vector3.zero;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (BlockedAttackID == AttackID) return;

        switch (collision.gameObject.layer)
        {
            case 10:
                HandleShieldCollision(collision);
                if(EffectID != AttackID)
                {
                    PlayerStateMachine.LocalInstance.PlayerVFX.PlayImpactFeedBack(PlayerVFX.EImpactVFXs.LightImpact);
                    PlayHitSound(collision.relativeVelocity.magnitude);
                    EffectID = AttackID;
                }
                break;
            case 8:
                HandleEnemyCollision(collision);
                if (EffectID != AttackID)
                {
                    PlayerUI.Instance.IncreaseComboCount();
                    if (weaponData.hitVFX != null) Instantiate(weaponData.hitVFX[Random.Range(0, weaponData.hitVFX.Count - 1)], collision.contacts[0].point, Quaternion.identity);
                    PlayHitSound(collision.relativeVelocity.magnitude);
                    EffectID = AttackID;
                }
                break;
            case 16:
                if (EffectID != AttackID)
                {
                    PlayerStateMachine.LocalInstance.PlayerVFX.PlayImpactFeedBack(PlayerVFX.EImpactVFXs.LightImpact);
                    if (weaponData.hitVFX != null) Instantiate(weaponData.hitVFX[Random.Range(0, weaponData.hitVFX.Count - 1)], collision.contacts[0].point, Quaternion.identity);
                    PlayHitSound(collision.relativeVelocity.magnitude);
                    EffectID = AttackID;
                }
                break;  
        }
    }

    private void PlayHitSound(float collisionMagnitude)
    {
        if(currentAttack.overrideHitSound)
        {
            //picking specific one
            weaponData.hitAsset.MulticlipsPlayMode = Ami.BroAudio.Data.MulticlipsPlayMode.Velocity;
            weaponData.hitSounds.Play().SetVelocity(currentAttack.hitVelocity);
        }
        else
        {
            //potentially make algorithm here for collision magnitude to velocity
            //picking random one
            weaponData.hitAsset.MulticlipsPlayMode = Ami.BroAudio.Data.MulticlipsPlayMode.Random;
            weaponData.hitSounds.Play();
        }
    }

    public virtual void HandleEnemyCollision(Collision collision)
    {
        
    }
    public virtual void HandleShieldCollision(Collision collision)
    {
        BlockedAttackID = AttackID;
        collision.gameObject.GetComponent<EnemyShield>().ShieldBlocked();
    }
    public virtual void EnableDamageColliders(int collider)
    {
        BoxCollider enabledCollider = DamageColliders[collider];

        enabledCollider.enabled = true;

        PlaySwingSound();

        weaponData.effortAsset.MulticlipsPlayMode = Ami.BroAudio.Data.MulticlipsPlayMode.Random;
        weaponData.effortSounds.Play();

        SetDamageData();
    }

    private void SetDamageData()
    {
        AttackID++;
        DamageAmount = damageMultiplier * weaponData.damage;
        StanceBreakPower = weaponData.StanceBreakPower * damageMultiplier;
    }

    private void PlaySwingSound()
    {
        if (currentAttack.overrideSwingSound) //specific one chosen
        {
            float pitch = Random.Range(currentAttack.swingSoundPitch.x, currentAttack.swingSoundPitch.y);
            weaponData.swingSounds.Play().SetPitch(pitch).SetVelocity(currentAttack.swingVelocity);
        }
        else // algorithm one
        {
            float enableTime = GetAnimationEventTime(currentAttack.animation, "DamageAvailible");
            float disableTime = GetAnimationEventTime(currentAttack.animation, "ComboAndDamageUnAvailable");

            float animationDuration = disableTime - enableTime;

            int velocity = Mathf.FloorToInt(animationDuration * 10f);

            float playbackSpeed = currentAttack.animation.length / animationDuration;
            float speedFactor = 1 / (animationDuration * playbackSpeed) * 1.5f;

            float pitch = Mathf.Clamp(Random.Range(speedFactor - .1f, speedFactor + .1f), .85f, 1.15f);
            weaponData.swingSounds.Play().SetPitch(pitch).SetVelocity(velocity);
        }
    }

    private float GetAnimationEventTime(AnimationClip clip, string eventName)
    {
        foreach (var animEvent in AnimationUtility.GetAnimationEvents(clip))
        {
            if (animEvent.functionName == eventName)
                return animEvent.time;
        }
        return 0f;
    }

    public void DisableDamageColliders(int collider)
    {
        DamageColliders[collider].enabled = false;
    }

    public virtual void DropWeapon() 
    {
        Destroy(gameObject);
    }

    public void SetUpHelper()
    {
        PlayerStateMachine playerState = PlayerStateMachine.LocalInstance;

        WeaponHolster = playerState.WeaponHolster;

        Holster = WeaponHolster.GetHolster(weaponData.holsterSlot);

        Hand = WeaponHolster.GetHolder(weaponData.holderSlot);

        Wielder = playerState.transform;
    }
    public void FillHolsterslot()
    {
        WeaponHolster.FillHolsterSlot(this);
    }

    public virtual void EnableWeaponVFX()
    {

    }

    public virtual void DisableWeaponVFX()
    {

    }
}
