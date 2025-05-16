using FIMSpace.FProceduralAnimation;
using Kinemation.MotionWarping.Runtime.Examples;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DummyStateMachine : StateManager<DummyStateMachine.EDummyState>, IRagdollAnimator2Receiver, IHumanoidUtilities
{
    public event EventHandler OnScriptsInitialized;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private EnemyDataSO dummyData;
    [SerializeField] private Health health;
    [SerializeField] private EnemyUI UI;
    [SerializeField] private EnemyVFX VFX;
    [SerializeField] private AlignComponent finisherAligner;

    public enum EDummyState
    {
        Grounded,

        Falling,

        Idle,

        Land,

        Damaged,

        Stagger,
        Stunned,
        Knockback,
        Knockdown,
        Popup,
        Finisher,
    }

    private DummyUtilities utilies;

    public HumanoidUtilities Utilites { get { return utilies; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitalizeScripts();
        InitializeStates();
        currentState.EnterState();
        health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath(object sender, EventArgs e)
    {
        utilies.CombatManager.ApplyDeathForceAndSleep();
        VFX.Dissolve();
        health.OnDeath -= Health_OnDeath;
        this.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        CurrentState(currentState);
    }

    public void InitalizeScripts()
    {
        utilies = new DummyUtilities(this, enemyStats, dummyData, health, VFX, UI, finisherAligner);
        OnScriptsInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeStates()
    {
        States.Add(EDummyState.Grounded, new DummyGroundedState(utilies, EDummyState.Grounded));
        States.Add(EDummyState.Falling, new DummyFallingState(utilies, EDummyState.Falling));

        States.Add(EDummyState.Land, new DummyLandState(utilies, EDummyState.Land));
        States.Add(EDummyState.Idle, new DummyIdleState(utilies, EDummyState.Idle));

        States.Add(EDummyState.Damaged, new DummyDamagedState(utilies, EDummyState.Damaged));

        States.Add(EDummyState.Stagger, new DummyStaggerState(utilies, EDummyState.Stagger));
        States.Add(EDummyState.Stunned, new DummyStunnedState(utilies, EDummyState.Stunned));
        States.Add(EDummyState.Knockback, new DummyKnockbackState(utilies, EDummyState.Knockback));
        States.Add(EDummyState.Knockdown, new DummyKnockdownState(utilies, EDummyState.Knockdown));
        States.Add(EDummyState.Popup, new DummyPopupState(utilies, EDummyState.Popup));
        States.Add(EDummyState.Finisher, new DummyFinisherState(utilies, EDummyState.Finisher));

        currentState = States[EDummyState.Grounded];
    }

    //Utilities handling damage
    public void RagdollAnimator2_OnCollisionEnterEvent(RA2BoneCollisionHandler hitted, Collision mainCollision)
    {
        if (((1 << mainCollision.gameObject.layer) & dummyData.DamagingWeaponLayers) != 0)
        {
            GetComponent<RagdollAnimator2>().RagdollBlend = 1; utilies.CombatManager.OnRagdollCollision(hitted, mainCollision);
        }
    }
    public void OnGetUp() => utilies.CombatManager.IsFallen = false;

    public DummyAnimationRequestor GetAnimationRequestor() { return utilies.AnimationRequestor; }
}
