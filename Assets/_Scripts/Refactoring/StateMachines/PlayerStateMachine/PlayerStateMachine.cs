using Sirenix.OdinInspector;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using TrialsOfOdin.Combat;
using System.Collections.Generic;

namespace TrialsOfOdin.State
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerStateMachine : StateManager<ECharacterState>, IAnimationRequestor, ISoundRequestor
    {
        /*
            This script inherits state manager which inherits a netorkbehavior
            Here we have all of our player states
         */

        public static PlayerStateMachine LocalInstance { get; private set; }

        private List<string> activeStates = new();
        [ShowInInspector]
        public List<string> ActiveStates
        {
            get 
            {
                activeStates.Clear();
                BaseState<ECharacterState> state = currentState;
                while(state != null)
                {
                    activeStates.Add(state.StateKey.ToString());
                    state = state.CurrentSubState;
                }
                return activeStates;
            }
        }

        [SerializeField] private CombatSystemPro combatSystem;
        public WeaponHolster WeaponHolster { get { return combatSystem.WeaponHolster; } }
        public CharacterController CharacterController { get; private set; }
        public PlayerAnimationEvents PlayerAnimationEvents { get; private set; }

        [Button]
        private void MovePlayerTo(Vector3 pos)
        {
            transform.position = pos;
        }

        public static event EventHandler OnAnyPlayerSpawn;

        public StateUtilities Utilities { get; private set; }
        public AnimationRequestor AnimationRequestor { get; private set; }
        public SoundRequestor SoundRequestor { get; private set; }
        public CombatSystemPro CombatSystem { get { return combatSystem; } }


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

            transform.position = new Vector3(5, 5, 5);

            CombatSystem.Stats.Health.OnDeath += Health_OnDeath;
        }

        private void Health_OnDeath()
        {

        }

        public override void OnDestroy()
        {
            if (!IsOwner) return;

            CombatSystem.Stats.Health.OnDeath -= Health_OnDeath;
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
            States.Add(ECharacterState.Grounded, new GroundedState(Utilities, ECharacterState.Grounded));
            States.Add(ECharacterState.Land, new LandState(Utilities, ECharacterState.Land));
            States.Add(ECharacterState.Idle, new IdleState(Utilities, ECharacterState.Idle));
            States.Add(ECharacterState.Walk, new WalkState(Utilities, ECharacterState.Walk));
            States.Add(ECharacterState.Run, new RunState(Utilities, ECharacterState.Run));
            States.Add(ECharacterState.Dodge, new DodgeState(Utilities, ECharacterState.Dodge));
            States.Add(ECharacterState.Jump, new JumpState(Utilities, ECharacterState.Jump));
            States.Add(ECharacterState.Falling, new FallingState(Utilities, ECharacterState.Falling));
            States.Add(ECharacterState.Ascend, new AscendState(Utilities, ECharacterState.Ascend));
            States.Add(ECharacterState.Block, new BlockState(Utilities, ECharacterState.Block));
            States.Add(ECharacterState.CombatMovement, new CombatMoveState(Utilities, ECharacterState.CombatMovement));
            States.Add(ECharacterState.CombatOrient, new CombatOrientState(Utilities, ECharacterState.CombatOrient));
            States.Add(ECharacterState.Aim, new AimState(Utilities, ECharacterState.Aim));
            States.Add(ECharacterState.AerialMovement, new AerialMovementState(Utilities, ECharacterState.AerialMovement));
            States.Add(ECharacterState.Stunned, new StunnedState(Utilities, ECharacterState.Stunned));
            States.Add(ECharacterState.Attack, new AttackState(Utilities, ECharacterState.Attack));

            currentState = States[ECharacterState.Grounded];
        }

        private void InitializeScripts()
        {
            Action<BaseState<ECharacterState>> updateCurrentState = CurrentState;
            Func<ECharacterState, BaseState<ECharacterState>> getStateInstance = GetStateInstance;

            Utilities = new StateUtilities(CombatSystem, new PlayerContext(CombatSystem.Stats, CharacterController, CombatSystem), updateCurrentState, getStateInstance);

            AnimationRequestor = Utilities.AnimationRequestor;
            SoundRequestor = Utilities.SoundRequestor;
        }
    }
}