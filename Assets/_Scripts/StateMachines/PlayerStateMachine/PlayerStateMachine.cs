using Kinemation.MotionWarping.Runtime.Core;
using Sirenix.OdinInspector;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using TrialsOfOdin.Combat;
using RootMotion.FinalIK;
using TrialsOfOdin.Stats;

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : StateManager<PlayerStateMachine.EPlayerState>
{
    /*
        This script inherits state manager which inherits a netorkbehavior
        Here we have all of our player states
     */

    public static PlayerStateMachine LocalInstance { get; private set; }
    public CharacterController CharacterController { get; private set; }

    [FoldoutGroup("Player References")]
    [SerializeField] private PlayerStats playerStats;
    [FoldoutGroup("Player References")]
    [SerializeField] private PlayerVFX playerVFX;
    public PlayerVFX PlayerVFX { get { return playerVFX; } }

    [FoldoutGroup("Player References")]
    [SerializeField] private WeaponHolster weaponHolster;

    [FoldoutGroup("Player References")]
    public MotionWarpingAsset playerWarpingAsset;
    public WeaponHolster WeaponHolster {get { return weaponHolster; }}

    public PlayerAnimationEvents PlayerAnimationEvents { get; private set; }

    [Button]
    private void MovePlayerTo(Vector3 pos)
    {
        transform.position = pos;
    }

    public static event EventHandler OnAnyPlayerSpawn;

    public PlayerUtilities Utilities { get; private set; }
    public PlayerContext Context { get; private set; }
    public PlayerAnimationRequestor AnimationRequestor { get; private set; }
    public PlayerSoundRequestor AudioRequestor { get; private set; }
    public PlayerMovementUtility MovementUtility { get; private set; }
    public PlayerCombatManager CombatManager { get; private set; }
    public ArmIK armIK;
    public enum EPlayerState
    {
        Grounded, //Root State

        Block, //walk sub state
        HeavyAttack,
        LightAttack,
        Aim,
        AimAttack,
        Idle,
        Walk,
        Run,
        Dodge,
        Land,
        Stunned,

        CombatMovement, // sub state for heavy attack, light attack & block 
        CombatOrient, // sub state for heavy attack and light attack

        Ascend, //RootState

        //Sub states for ascend
        //Climb,
        //Vault,
        Jump,

        Falling, //Root State

        AerialAttack, //Sub state for falling and Jump aswell
        AerialMovement,
    }
    

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;     

        LocalInstance = this;

        GetComponents();

        InitializeScripts();

        InitializeStates();

        OnAnyPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        if (!IsOwner) return;

        PlayableDirector director = GetComponentInChildren<PlayableDirector>();

        CinemachineBrain cinemachineBrain = MainCamera.Instance.GetComponent<CinemachineBrain>();

        //potentially finisher
        foreach (var output in director.playableAsset.outputs)
        {
            if (output.streamName == "Cinemachine Track") // Make sure this matches the track name in Timeline
            {
                director.SetGenericBinding(output.sourceObject, cinemachineBrain);
                break;
            }
        }

        currentState.EnterState();

        transform.position = new Vector3(5,5, 5);

        playerStats.Health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath()
    {

    }

    public override void OnDestroy()
    {
        if (!IsOwner) return;

        playerStats.Health.OnDeath -= Health_OnDeath;
    }



    private void Update()
    {
        if (!IsOwner) return;
        CurrentState(currentState);
    }

    private void GetComponents()
    {
        CharacterController = GetComponent<CharacterController>();
        PlayerAnimationEvents = GetComponent<PlayerAnimationEvents>();
    }

    private void InitializeStates()
    {
        //Adding states to state manager dictionary
        States.Add(EPlayerState.Grounded, new PlayerGroundedState(Utilities, EPlayerState.Grounded));
        States.Add(EPlayerState.Land, new PlayerLandState(Utilities, EPlayerState.Land));
        States.Add(EPlayerState.Idle, new PlayerIdleState(Utilities, EPlayerState.Idle));
        States.Add(EPlayerState.Walk, new PlayerWalkState(Utilities, EPlayerState.Walk));
        States.Add(EPlayerState.Run, new PlayerRunState(Utilities, EPlayerState.Run));
        States.Add(EPlayerState.Dodge, new PlayerDodgeState(Utilities, EPlayerState.Dodge));
        States.Add(EPlayerState.HeavyAttack, new PlayerHeavyAttackState(Utilities, EPlayerState.HeavyAttack));
        States.Add(EPlayerState.Jump, new PlayerJumpState(Utilities, EPlayerState.Jump));
        States.Add(EPlayerState.Falling, new PlayerFallingState(Utilities, EPlayerState.Falling));
        //States.Add(EPlayerState.Climb, new PlayerClimbState(Utilities, EPlayerState.Climb));
        //States.Add(EPlayerState.Vault, new PlayerVaultState(Utilities, EPlayerState.Vault));
        States.Add(EPlayerState.Ascend, new PlayerAscendState(Utilities, EPlayerState.Ascend));
        States.Add(EPlayerState.Block, new PlayerBlockState(Utilities, EPlayerState.Block));
        States.Add(EPlayerState.LightAttack, new PlayerLightAttackState(Utilities, EPlayerState.LightAttack));
        States.Add(EPlayerState.AerialAttack, new PlayerAerialAttackState(Utilities, EPlayerState.AerialAttack));
        States.Add(EPlayerState.CombatMovement, new PlayerCombatMoveState(Utilities, EPlayerState.CombatMovement));
        States.Add(EPlayerState.CombatOrient, new PlayerCombatOrientState(Utilities, EPlayerState.CombatOrient));
        States.Add(EPlayerState.Aim, new PlayerAimState(Utilities, EPlayerState.Aim));
        States.Add(EPlayerState.AerialMovement, new PlayerAerialMovementState(Utilities, EPlayerState.AerialMovement));
        States.Add(EPlayerState.AimAttack, new PlayerAimAttackState(Utilities, EPlayerState.AimAttack));
        States.Add(EPlayerState.Stunned, new PlayerStunnedState(Utilities, EPlayerState.Stunned));
        currentState = States[EPlayerState.Grounded];
    }

    private void InitializeScripts()
    {
        Utilities = new PlayerUtilities(playerStats, armIK);

        Context = Utilities.Context;
        CombatManager = Utilities.CombatManager;
        MovementUtility = Utilities.MovementUtility;
        AnimationRequestor = Utilities.AnimationRequestor;
        AudioRequestor = Utilities.AudioRequestor;
    }

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawn = null;
    }
}
