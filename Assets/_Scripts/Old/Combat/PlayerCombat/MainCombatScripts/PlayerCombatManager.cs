using UnityEngine;
using System;
using System.Collections.Generic;
using Kinemation.MotionWarping.Runtime.Core;
using Kinemation.MotionWarping.Runtime.Utility;
using KINEMATION.MotionWarping.Editor.Core;
using UnityEditor;
using System.Collections;
using UnityEngine.Playables;
using RootMotion.FinalIK;
using TrialsOfOdin.Stats;
using TrialsOfOdin.State;

namespace TrialsOfOdin.Combat
{
    public class PlayerCombatManager
    {
        public Weapon Weapon { get; private set; }
        readonly PlayerStats stats;

        public event EventHandler OnHeavyAttackStarted;
        public event EventHandler OnHeavyAttackFinished;

        public event EventHandler OnLightAttackStarted;
        public event EventHandler OnLightAttackFinished;

        public event EventHandler<AttackAnimationData> OnExecuteAttack;

        public event EventHandler OnRageAnimation;

        private AttackSO currentAttack;
        private AttackAnimationData attackAnimationData = new AttackAnimationData();
        public int ComboTransitionValue { get; set; }
        public bool exitComboUnlessRecover = false;
        public bool CanAimAttack { get; set; }
        public bool IsAiming { get; set; }
        public bool IsHeavyAttacking { get; set; }
        public bool IsLightAttacking { get; set; }
        public bool IsComboAvailable { get; set; }
        public bool IsComboTransition { get; set; }
        public bool SwitchFromAimWeapon { get; set; }
        public bool IsBlocking { get; set; }
        public bool ChargedAttackAfterParry { get; set; }
        public bool IsAerialAttack { get; set; }
        public bool IsHeavyAerialAttack { get; set; }
        public bool IsHeavyAttackAfterAerial { get; set; }
        public bool IsRecoveringFromAerialAttack { get; set; }
        public bool IsComboingFromAerialAttack { get; set; }
        public bool CanMotionWarp { get; set; } = true;
        private bool onlyWarpInY = false;
        public float MotionWarpingMagnitude { get; set; } = 2f;
        public float TargetAssistDistance { get; set; } = 5f;
        public int ComboCountForRage = 3;
        private float minimumViewableAngle = -20;
        private float maximumViewableAngle = 20;
        private List<Transform> availibleTargets = new List<Transform>();
        private Transform target;
        private bool targetted;
        private float motionWarpCancelDistance = 2f;
        private ArmIK armIK;
        private float rageTime = 10f;
        public int DamagedID { get; set; }


        public PlayerCombatManager(PlayerStats stats, ArmIK armIK)
        {
            this.armIK = armIK;
            GameInput gameInput = GameInput.Instance;

            //gameInput.OnHeavyAttackStarted += (object sender, EventArgs e) => 
            //{ OnHeavyAttackStarted?.Invoke(this, EventArgs.Empty); IsHeavyAttacking = true; };
            //gameInput.OnHeavyAttackFinished += (object sender, EventArgs e) =>
            //{ OnHeavyAttackFinished?.Invoke(this, EventArgs.Empty); IsHeavyAttacking = false; };

            //gameInput.OnLightAttackStarted += (object sender, EventArgs e) =>
            //{ OnLightAttackStarted?.Invoke(this, EventArgs.Empty); IsLightAttacking = true; };
            //gameInput.OnLightAttackFinished += (object sender, EventArgs e) =>
            //{ OnLightAttackFinished?.Invoke(this, EventArgs.Empty); IsLightAttacking = false; };

            //gameInput.OnBlockStarted += (object sender, EventArgs e) => IsBlocking = true;
            //gameInput.OnBlockFinished += (object sender, EventArgs e) => IsBlocking = false;

            //gameInput.OnAimStarted += (object sender, EventArgs e) => IsAiming = true;
            //gameInput.OnAimFinished += (object sender, EventArgs e) => IsAiming = false;

            //gameInput.OnRage += GameInput_OnRequestRage;

            PlayerStateMachine playerState = PlayerStateMachine.LocalInstance;

            playerState.WeaponHolster.OnNewActiveWeapon += PlayerContext_OnNewActiveWeapon;

            playerState.PlayerAnimationEvents.OnComboAvalible += () => IsComboAvailable = true;
            playerState.PlayerAnimationEvents.OnComboUnAvalible += () => IsComboAvailable = false;
            playerState.PlayerAnimationEvents.OnRage += PlayerAnimationEvents_OnRage;
            this.stats = stats;
        }

        private void PlayerAnimationEvents_OnRage()
        {
            PlayerUI.Instance.RageUI(rageTime);
            CoroutineHelper.Instance.StartCoroutine(TurnOffRageBuffAfterTime(rageTime));
            //PlayerStateMachine.LocalInstance.PlayerVFX.EnableRageVFX();
            stats.BuffStats();
        }

        private void GameInput_OnRequestRage(object sender, EventArgs e)
        {
            if (PlayerUI.Instance.CanRage())
            {
                OnRageAnimation?.Invoke(this, EventArgs.Empty);
            }
        }

        private IEnumerator TurnOffRageBuffAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);

            DeactivateRageBuffs();
        }

        public void DeactivateRageBuffs()
        {
            //PlayerStateMachine.LocalInstance.PlayerVFX.DisableRageVFX();
            stats.RevertStats();
            PlayerUI.Instance.ResetAll();
        }

        private void PlayerContext_OnNewActiveWeapon(Weapon weapon)
        {
            Weapon = weapon;
            CanAimAttack = Weapon.GetComponent<IAimAttacks>() != null;
        }

        public AttackSO GetLightCombo(int comboValue) { return Weapon.LightCombo[comboValue]; }
        public AttackSO GetHeavyCombo(int comboValue) { return Weapon.HeavyCombo[comboValue]; }
        public AttackSO GetLightChargedAttack() { return Weapon.LightChargedAttack; }
        public AttackSO GetHeavyChargedAttack() { return Weapon.HeavyChargedAttack; }
        public AttackSO GetLightSprintAttack() { return Weapon.LightSprintAttack; }
        public AttackSO GetHeavySprintAttack() { return Weapon.HeavySprintAttack; }
        public AttackSO GetLightAerialAttack() { return Weapon.LightAerialAttack; }
        public AttackSO GetHeavyAerialAttack() { return Weapon.HeavyAerialAttack; }
        public AttackSO GetLightAimAttack() { return Weapon.LightAimAttack; }
        public AttackSO GetHeavyAimAttack() { return Weapon.HeavyAimAttack; }
        public AttackSO GetRecoveryAttack() { exitComboUnlessRecover = false; return Weapon.RecoveryAttacks[currentAttack.recoveryAttack]; }
        public bool HasRecoveryAttack() { return currentAttack.hasRecoveryAttack; }
        public AttackSO GetFinisherAttack() { return Weapon.FinisherAttack; }
        public bool canCombo() { return currentAttack.canCombo; }
        public void ActivateArmIK(Vector3 collisionPoint, Vector3 normal)
        {
            Transform targetIK = armIK.solver.arm.target;
            Transform hand = armIK.solver.hand.transform;

            targetIK.position = collisionPoint - normal * 0.05f;

            targetIK.rotation = hand.rotation;

            armIK.enabled = true;

            CoroutineHelper.Instance.StartCoroutine(TurnOffArmIK());
        }

        private IEnumerator TurnOffArmIK()
        {
            yield return new WaitForSeconds(.2f);

            armIK.enabled = false;
        }

        public bool ExecuteAttack(AttackSO attack)
        {
            if (!PlayerStateMachine.LocalInstance.Utilities.HasSufficientStamina(attack.staminaCost)) return false;

            currentAttack = attack;

            Weapon.currentAttack = attack;
            Weapon.SetDamageMultiplier(attack.damageMultiplier);


            attackAnimationData.stateName = attack.animationState.ToString();
            attackAnimationData.animationSpeed = attack.animationSpeed;

            IsComboAvailable = true;
            OnExecuteAttack?.Invoke(this, attackAnimationData);

            GetClosestTarget();


            if (target != null)
            {
                float distanceSq = (PlayerStateMachine.LocalInstance.transform.position - target.transform.position).sqrMagnitude;

                if (distanceSq > (motionWarpCancelDistance * motionWarpCancelDistance))
                {
                    if (CanMotionWarp) WarpMotion(attack.animation, attack.animationSpeed, true);
                }
                else
                {
                    if (CanMotionWarp) WarpMotion(attack.animation, attack.animationSpeed, false);
                }
            }

            return true;
        }

        private void WarpMotion(AnimationClip attackAnimation, float animationSpeed, bool positionalWarping)
        {
            //MotionWarpingAsset motionWarpingAsset = PlayerStateMachine.LocalInstance.playerWarpingAsset;
            //motionWarpingAsset.animation = attackAnimation;

            //ExtractCurves(ref motionWarpingAsset);
            //GeneratePhases(ref motionWarpingAsset, attackAnimation.length);


            //// Modify Warp Phase
            //WarpPhase warpPhase = motionWarpingAsset.warpPhases[0];

            //// Stop slightly in front of the target
            //warpPhase.tOffset = target.GetComponent<EnemyTarget>().targetParent.transform.position - PlayerStateMachine.LocalInstance.transform.forward.Flatten() * .5f;

            //if (onlyWarpInY)
            //{
            //    motionWarpingAsset.useWarping.x = false;
            //    motionWarpingAsset.useWarping.z = false;
            //}
            //else
            //{
            //    motionWarpingAsset.useWarping.x = true;
            //    motionWarpingAsset.useWarping.z = true;
            //}

            //if (positionalWarping)
            //{
            //    motionWarpingAsset.useWarping.x = true;
            //    //motionWarpingAsset.useWarping.y = true;
            //    motionWarpingAsset.useWarping.z = true;
            //}
            //else
            //{
            //    motionWarpingAsset.useWarping.x = false;
            //    motionWarpingAsset.useWarping.y = false;
            //    motionWarpingAsset.useWarping.z = false;
            //}

            //warpPhase.rOffset = Quaternion.LookRotation(target.position - PlayerStateMachine.LocalInstance.transform.position).eulerAngles;

            //warpPhase.minRate = 1f;
            //warpPhase.maxRate = 1f;

            //// Update phase instead of clearing
            //motionWarpingAsset.warpPhases[0] = warpPhase;

            //motionWarpingAsset.playRateBasis = (MotionWarpingMagnitude * animationSpeed);

            //ComputeTotalRootMotion(ref motionWarpingAsset);

            //// Setup warp interaction result
            //WarpInteractionResult result = new()
            //{
            //    success = true,
            //    points = new[] { warpPhase.Target },
            //    asset = motionWarpingAsset
            //};

            //PlayerStateMachine.LocalInstance.GetComponent<MotionWarping>().Play(result.asset, result.points);
            //CoroutineHelper.Instance.StartCoroutine(TrackPlayerPosition(attackAnimation.length));
            //target = null;
        }

        private IEnumerator TrackPlayerPosition(float animationLength)
        {
            float elapsedTime = 0f;
            MotionWarping motionWarping = PlayerStateMachine.LocalInstance.GetComponent<MotionWarping>();

            while (elapsedTime < animationLength)
            {
                yield return null;  // Wait one frame

                if (target == null) break;

                float distanceSq = (PlayerStateMachine.LocalInstance.transform.position - target.transform.position).sqrMagnitude;
                if (distanceSq < motionWarpCancelDistance * motionWarpCancelDistance)
                {
                    Debug.Log("Stopping motion warping due to proximity.");
                    motionWarping.Stop();  // Assuming MotionWarping has a Stop() method
                    yield break;  // Exit coroutine early
                }

                elapsedTime += Time.deltaTime;
            }
        }
        public int GetHeavyComboValue() { return currentAttack.heavyComboTransition; }
        public int GetLightComboValue() { return currentAttack.lightComboTransition; }
        public bool AimAttack()
        {
            if (IsAiming && CanAimAttack) return true;
            return false;
        }
        public void AerialTransitionAttack(bool isHeavy)
        {
            bool recovered = false;
            BaseAttackTypes aerialAttack = IsHeavyAerialAttack ? BaseAttackTypes.heavy : BaseAttackTypes.light;

            //if transition attack is heavy
            if (isHeavy)
            {
                //if aerial attack was of heavy type, checking to match with recovery
                if (IsHeavyAerialAttack)
                {
                    if (currentAttack.hasRecoveryAttack)
                    {
                        IsComboingFromAerialAttack = false;
                        IsRecoveringFromAerialAttack = true;
                        recovered = true;
                    }
                }
                //this means aerial attack was light so we are combing into heavy
                if (!recovered && currentAttack.canCombo)
                {
                    ComboTransitionValue = currentAttack.heavyComboTransition;

                    IsRecoveringFromAerialAttack = false;
                    IsComboingFromAerialAttack = true;
                }
            }
            else
            {
                if (!IsHeavyAerialAttack)
                {
                    if (currentAttack.hasRecoveryAttack)
                    {
                        IsComboingFromAerialAttack = false;
                        IsRecoveringFromAerialAttack = true;
                        recovered = true;
                    }
                }
                if (!recovered && currentAttack.canCombo)
                {
                    ComboTransitionValue = currentAttack.lightComboTransition;

                    IsRecoveringFromAerialAttack = false;
                    IsComboingFromAerialAttack = true;
                }
            }
        }
        private void GetClosestTarget()
        {
            availibleTargets.Clear();
            float shortestDistance = Mathf.Infinity;

            Transform playerTransform = PlayerStateMachine.LocalInstance.transform;
            Transform camTransform = MainCamera.Instance.transform;

            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, TargetAssistDistance, WorldUtilityManager.Instance.TargetLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                Transform lockOnTransform = colliders[i].transform;
                Vector3 lockOnPosition = lockOnTransform.position;

                Vector3 lockOnTargetsDirection = lockOnPosition - camTransform.position;
                float viewableAngle = Vector3.Angle(lockOnTargetsDirection, camTransform.forward);

                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;
                    //Environment check
                    if (Physics.Linecast(camTransform.position, lockOnPosition, out hit, WorldUtilityManager.Instance.EnvironmentLayer))
                        continue;
                    else
                        availibleTargets.Add(lockOnTransform);
                }

            }

            Transform closestEnemy = null;

            for (int j = 0; j < availibleTargets.Count; j++)
            {
                float distanceFromTarget = Vector3.Distance(playerTransform.position, availibleTargets[j].position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    closestEnemy = availibleTargets[j];
                }
            }

            if (closestEnemy != null)
            {
                target = closestEnemy;

                targetted = true;
            }
            else
            {
                target = null;

                targetted = false;
            }
        }
        public bool TryFinisherAttack()
        {

            GetClosestTarget();
            if (target == null) return false;

            if (target.GetComponent<EnemyTarget>().targetParent.TryGetComponent(out IHumanoidUtilities humanoidUtilities))
            {
                if (humanoidUtilities.Utilites.CombatManager.isFinisherable)
                {
                    humanoidUtilities.Utilites.CombatManager.Finisher(PlayerStateMachine.LocalInstance.gameObject);
                    CoroutineHelper.Instance.StartCoroutine(SlowDownTime(.2f));
                    PlayerStateMachine.LocalInstance.gameObject.GetComponentInChildren<PlayableDirector>().Play();
                    return true;
                }
            }

            return false;
        }
        private IEnumerator SlowDownTime(float delay)
        {
            yield return new WaitForSeconds(delay);
            Time.timeScale = .2f;
        }
        public enum BaseAttackTypes
        {
            heavy,
            light,
        }
        private void ExtractCurves(ref MotionWarpingAsset _asset)
        {
            if (_asset == null || _asset.animation == null)
            {
                Debug.LogError("WarpingAsset or AnimationClip is null!");
                return;
            }

            EditorCurveBinding[] tBindings = new EditorCurveBinding[3];

            var curveBindings = AnimationUtility.GetCurveBindings(_asset.animation);
            foreach (var binding in curveBindings)
            {
                if (_asset.animation.isHumanMotion)
                {
                    if (binding.propertyName.ToLower().Contains("roott.x"))
                    {
                        tBindings[0] = binding;
                    }
                    else if (binding.propertyName.ToLower().Contains("roott.y"))
                    {
                        tBindings[1] = binding;
                    }
                    else if (binding.propertyName.ToLower().Contains("roott.z"))
                    {
                        tBindings[2] = binding;
                    }
                }
                else
                {
                    if (!binding.path.ToLower().EndsWith("root")) continue;

                    if (binding.propertyName.ToLower().Contains("localposition.x"))
                    {
                        tBindings[0] = binding;
                    }
                    else if (binding.propertyName.ToLower().Contains("localposition.y"))
                    {
                        tBindings[1] = binding;
                    }
                    else if (binding.propertyName.ToLower().Contains("localposition.z"))
                    {
                        tBindings[2] = binding;
                    }
                }
            }

            var curves = WarpingEditorUtility.ValidateCurves(_asset, 60f, tBindings);

            _asset.rootX = curves.X;
            _asset.rootY = curves.Y;
            _asset.rootZ = curves.Z;
        }
        private void ComputeTotalRootMotion(ref MotionWarpingAsset _asset)
        {
            if (_asset.animation == null) return;

            float sampleRate = _asset.animation.frameRate;
            if (Mathf.Approximately(sampleRate, 0f)) return;

            for (int i = 0; i < _asset.warpPhases.Count; i++)
            {
                var phase = _asset.warpPhases[i];

                float playback = phase.startTime;
                Vector3 lastValue = _asset.GetVectorValue(playback);

                phase.totalRootMotion = Vector3.zero;

                while (playback <= phase.endTime)
                {
                    // Accumulate the delta.
                    Vector3 value = _asset.GetVectorValue(playback);
                    Vector3 delta = value - lastValue;

                    phase.totalRootMotion.x += Mathf.Abs(delta.x);
                    phase.totalRootMotion.y += Mathf.Abs(delta.y);
                    phase.totalRootMotion.z += Mathf.Abs(delta.z);

                    lastValue = value;

                    playback += 1f / sampleRate;
                }

                _asset.warpPhases[i] = phase;
            }
        }
        private void GeneratePhases(ref MotionWarpingAsset _asset, float animationLength)
        {
            float totalTime = _asset.GetLength();
            if (Mathf.Approximately(totalTime, 0f)) return;

            _asset.warpPhases.Clear();

            float timeStep = totalTime / _asset.phasesAmount;

            for (int i = 0; i < _asset.phasesAmount; i++)
            {
                WarpPhase phase = new WarpPhase()
                {
                    minRate = 0f,
                    maxRate = 1f,
                    startTime = timeStep * i,
                    endTime = timeStep * i + timeStep,
                };

                phase.startTime = .1f * animationLength;
                phase.endTime = .7f * animationLength;
                _asset.warpPhases.Add(phase);
            }
        }
    }
}