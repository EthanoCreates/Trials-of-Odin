using FIMSpace.FProceduralAnimation;
using Kinemation.MotionWarping.Runtime.Examples;
using UnityEngine;
using UnityEngine.AI;

public class DummyUtilities : HumanoidUtilities
{
    public DummyStateMachine StateMachine { get; private set; }
    public DummyContext Context { get; private set; }
    public DummyCombatManager CombatManager { get; private set; }
    public DummyAnimationRequestor AnimationRequestor { get; private set; }
    public NavMeshAgent Agent { get; private set; }

    HumanoidSMContext HumanoidUtilities.Context => throw new System.NotImplementedException();

    HumanoidCombatManager HumanoidUtilities.CombatManager => CombatManager;

    public EnemyDataSO dummyData;
    private Health health;

    public DummyUtilities(DummyStateMachine stateMachine, EnemyDataSO data, Health health, EnemyVFX enemyVFX, EnemyUI UI, AlignComponent finisherAlign)
    {
        StateMachine = stateMachine;

        Context = new DummyContext(stateMachine.transform, data);
        CombatManager = new DummyCombatManager(health, data, enemyVFX, UI, finisherAlign);
        AnimationRequestor = new DummyAnimationRequestor();

        AnimationRequestor.OnAttack += (object sender, System.EventArgs e) => stateMachine.GetComponent<RagdollAnimator2>().RagdollBlend = 0;

        Agent = stateMachine.GetComponent<NavMeshAgent>();
        this.dummyData = data;
        this.health = health;
    }
}
