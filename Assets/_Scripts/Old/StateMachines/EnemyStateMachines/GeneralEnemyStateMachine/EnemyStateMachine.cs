using UnityEngine;
using UnityEngine.AI;
public class EnemyStateMachine : StateManager<EnemyStateMachine.EEnemyState>, IEnemies
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private EnemyDataSO enemyData;
    [SerializeField] private HealthForRagdolls enemyHealth;
    [SerializeField] private EnemyWeapon primaryWeapon;
    [SerializeField] private EnemyShield shield;
    [SerializeField] private EnemyStatesSO enemyStates;

    public EnemyContext EnemyContext { get; private set; }
    public EnemyAnimatorRequestor EnemyAnimatorRequestor { get; private set; }
    public EnemyMovementUtility EnemyMovementUtility { get; private set; }

    public bool SpawningForBossArena { get; private set; }

    public enum EEnemyState
    {
        Grounded, //root state

        Land,
        Idle,
        Patrol,
        Chase,
        Search,
        Attack,
        Block,
        Stunned,

        Falling, //root state
    }

    private void Awake()
    {
        EnemyContext = new EnemyContext(this, navMeshAgent, this.transform, enemyData.groundLayers, enemyData.walkSpeed, enemyData.runSpeed, enemyHealth, primaryWeapon, shield);
        EnemyAnimatorRequestor = new EnemyAnimatorRequestor(EnemyContext);
        EnemyMovementUtility = new EnemyMovementUtility(EnemyContext, EnemyAnimatorRequestor);

        InitializeStates();
    }

    private void Start()
    {
        if(!SpawningForBossArena)currentState.EnterState();
        enemyHealth.OnDeath += EnemyHealth_OnDeath;
        enemyHealth.OnDamage += EnemyHealth_OnDamage;
    }

    private void EnemyHealth_OnDamage(object sender, HealthForRagdolls.OnDamageEventArgs e)
    {
        EnemyAnimatorRequestor.AnimateImpact(e.damageX, e.damageZ, e.suspectedPosition);
    }

    private void EnemyHealth_OnDeath(object sender, System.EventArgs e)
    {
        navMeshAgent.enabled = false;
        primaryWeapon.DisableDamageColliders();
        this.enabled = false;
    }

    private void Update()
    {
        if (SpawningForBossArena) return;
        CurrentState(currentState);
    }

    private void InitializeStates()
    {
        //Adding states to state manager dictionary
        EnemyStateDictionary enemyStateDictionary = new EnemyStateDictionary(EnemyContext, EnemyMovementUtility, EnemyAnimatorRequestor);

        States.Add(EEnemyState.Grounded, enemyStateDictionary.GetGroundedState(enemyStates.groundedState));
        States.Add(EEnemyState.Idle, enemyStateDictionary.GetIdleState(enemyStates.idleState));
        States.Add(EEnemyState.Land, enemyStateDictionary.GetLandState(enemyStates.landState));
        States.Add(EEnemyState.Patrol, enemyStateDictionary.GetPatrolState(enemyStates.patrolState));
        States.Add(EEnemyState.Chase, enemyStateDictionary.GetChaseState(enemyStates.chaseState));
        States.Add(EEnemyState.Search, enemyStateDictionary.GetSearchState(enemyStates.searchState));
        States.Add(EEnemyState.Attack, enemyStateDictionary.GetAttackState(enemyStates.attackState));
        States.Add(EEnemyState.Block, enemyStateDictionary.GetBlockState(enemyStates.blockState));
        States.Add(EEnemyState.Falling, enemyStateDictionary.GetFallingState(enemyStates.fallState));
        States.Add(EEnemyState.Stunned, enemyStateDictionary.GetStunnedState(enemyStates.stunnedState));

        currentState = States[EEnemyState.Grounded];
    }

    public void DamageUnavailable()
    {
        primaryWeapon.DisableDamageColliders();
    }

    public void DamageAvailible()
    {
        primaryWeapon.EnableDamageColliders();
    }

    public void AnimFinished()
    {
        EnemyContext.AnimationFinished();
    }

    public void AnimStarted()
    {
        EnemyContext.AnimationStarted();
    }

    public void IsSpawning()
    {
        SpawningForBossArena = true;
        navMeshAgent.enabled = false;
    }

    public void spawned()
    {
        SpawningForBossArena = false;
        navMeshAgent.enabled = true;
        currentState.EnterState();
    }

    private void OnDrawGizmos()
    {
        // Set the Gizmo color (optional)
        Gizmos.color = Color.green;

        // Calculate the sphere position for the ground check
        Vector3 spherePosition = transform.position;
        spherePosition.y -= -.14f;

        // Draw the sphere representing the ground check area
        Gizmos.DrawWireSphere(spherePosition, .5f);
    }
}
