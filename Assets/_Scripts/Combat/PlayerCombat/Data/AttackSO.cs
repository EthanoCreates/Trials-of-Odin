using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using Ami.BroAudio.Data;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "CombatSOs/AttackSO")]
public class AttackSO : ScriptableObject
{
    [FoldoutGroup("Attack Data")]
    [Range(0.1f, 5f)]
    public float damageMultiplier = 1f;
    [FoldoutGroup("Attack Data")]
    [Range(0, 20)]
    public int staminaCost = 5;
    [ShowIf(nameof(IsChargedAttack))]
    [FoldoutGroup("Attack Data")]
    [Range(0.1f, 3f)]
    public float chargingTimer = 1f;

    [FoldoutGroup("Transition Data")]
    [ShowIf(nameof(NotComboAttack))]
    public bool canCombo = true;

    [FoldoutGroup("Transition Data")]
    [ShowIf(nameof(HasLightComboTransition))]
    [ValueDropdown("GetLightCombosDropdown")]
    [LabelWidth(width: 150f)]
    public int lightComboTransition;

    [FoldoutGroup("Transition Data")]
    [ShowIf(nameof(HasHeavyComboTransition))]
    [ValueDropdown("GetHeavyCombosDropdown")]
    [LabelWidth(width: 150f)]
    public int heavyComboTransition;

    [FoldoutGroup("Transition Data")]
    [ShowIf(nameof(CanHaveRecoveryAttack))]
    public bool hasRecoveryAttack = false;

    [FoldoutGroup("Transition Data")]
    [ShowIf(nameof(hasRecoveryAttack))]
    [ValueDropdown("GetRecoveryDropdown")]
    public int recoveryAttack;

    [FoldoutGroup("Animation Data")]
    public AttackClipType animationState;

    [FoldoutGroup("Animation Data")]
    [Range(0.1f, 3f)]
    public float animationSpeed = 1f;

    [FoldoutGroup("Effects")]
    public bool overrideSwingSound;

    [HorizontalGroup("Effects/SwingSound")]
    [ShowIf(nameof(overrideSwingSound))]
    [ValueDropdown(nameof(GetSwingAudioClips))]
    [OnValueChanged(nameof(SetSwingAudioClip))]
    [LabelText("Swing Sound")]
    [LabelWidth(width: 130f)]
    public int swingVelocity = 1;

    [FoldoutGroup("Effects")]
    [ShowIf(nameof(overrideSwingSound))]
    [MinMaxSlider(-3f, 3f, true)]
    public Vector2 swingSoundPitch = new(0.9f, 1.1f);

    [FoldoutGroup("Effects")]
    public bool overrideEffortSound;

    [HorizontalGroup("Effects/EffortSound")]
    [ShowIf(nameof(overrideEffortSound))]
    [ValueDropdown(nameof(GetEffortAudioClips))]
    [OnValueChanged(nameof(SetEffortAudioClip))]
    [LabelText("Effort Sound")]
    [LabelWidth(width:130f)]
    public int effortVelocity = 1;

    [FoldoutGroup("Effects")]
    public bool overrideHitSound;

    [HorizontalGroup("Effects/HitSound")]
    [ShowIf(nameof(overrideHitSound))]
    [ValueDropdown(nameof(GetHitAudioClips))]
    [OnValueChanged(nameof(SetHitAudioClip))]
    [LabelText("Hit Sound")]
    [LabelWidth(width: 130f)]
    public int hitVelocity = 1;

    #region EditorValues
#if UNITY_EDITOR
    public bool IsHeavyAttack { private get; set; }
    public bool IsComboAttack { private get; set; }
    public bool IsChargedAttack { private get; set; }
    public bool IsRecoveryAttack { private get; set; }
    public bool IsAimAttack { private get; set; }

    [HideInInspector]
    public AnimatorOverrideController animatorOverrideController;
    [ShowIf("ShowAnimation")]
    [ShowInInspector]
    [FoldoutGroup("Animation Data")]
    public AnimationClip animation
    {
        get
        {
            if (animatorOverrideController != null) return animatorOverrideController[animationState.ToString()];
            else return null;
        }
        private set { animatorOverrideController[animationState.ToString()] = value; }
    }
    #region EditAnimation

    [FoldoutGroup("Animation Data")]
    [Button("Edit Animation", ButtonSizes.Small)]
    private void EditAnimationEvents()
    {
        if (animation == null)
        {
            Debug.LogWarning("No animation clip is assigned to edit.");
            return;
        }

        // Check if Ragnar is in the scene, spawn if not
        GameObject previewCharacter = GameObject.FindGameObjectWithTag("Player") ?? InstantiateRagnar();

        // Get the Animator from Ragnar
        Animator animator = previewCharacter.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on Ragnar.");
            return;
        }

        // Set the AnimatorOverrideController
        if (animatorOverrideController == null)
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        animator.runtimeAnimatorController = animatorOverrideController;

        // Set the selected animation
        if (animatorOverrideController != null && animation != null)
        {
            animatorOverrideController["YourAnimationStateName"] = animation;
            animator.Play("YourAnimationStateName", 0, 0f);
        }

        // Open the Animation window
        EditorApplication.ExecuteMenuItem("Window/Animation/Animation");

        // Select Ragnar in the Hierarchy
        Selection.activeGameObject = previewCharacter;

        // Use delayCall to interact with Animation window after it's open
        EditorApplication.delayCall += () =>
        {
            // Set animation clip in the Animation window via reflection
            SetAnimationClipInWindow();
        };
    }

    private GameObject InstantiateRagnar()
    {
        string path = "Assets/Prefabs/Players/Playable Characters/Ragnar.prefab";
        GameObject previewCharacter = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path)) as GameObject;
        previewCharacter.name = "Ragnar";
        previewCharacter.transform.position = new Vector3(0f, 1f, 0f);
        return previewCharacter;
    }

    // Reflection method to set the animation clip in the AnimationWindow
    private void SetAnimationClipInWindow()
    {
        var animationWindowType = Type.GetType("UnityEditor.AnimationWindow,UnityEditor");
        if (animationWindowType != null)
        {
            var animationWindow = GetAnimationWindowInstance(animationWindowType);
            if (animationWindow != null)
            {
                SetAnimationClip(animationWindow, animation);
                Debug.Log("Animation clip set in the Animation window.");
            }
            else
            {
                Debug.LogWarning("Animation Window instance not found.");
            }
        }
        else
        {
            Debug.LogError("AnimationWindow type not found.");
        }
    }

    // Reflection method to get the current open AnimationWindow instance
    private object GetAnimationWindowInstance(Type animationWindowType)
    {
        var getAnimationWindowMethod = animationWindowType.GetMethod("GetAllAnimationWindows", BindingFlags.NonPublic | BindingFlags.Static);
        if (getAnimationWindowMethod != null)
        {
            var animationWindows = getAnimationWindowMethod.Invoke(null, null) as System.Collections.Generic.List<UnityEditor.AnimationWindow>;
            return animationWindows?.FirstOrDefault();
        }
        return null;
    }

    // Set the animation clip in the AnimationWindow
    private void SetAnimationClip(object animationWindow, AnimationClip clip)
    {
        var animationClipProperty = animationWindow.GetType().GetProperty("animationClip", BindingFlags.Public | BindingFlags.Instance);
        if (animationClipProperty != null)
            animationClipProperty.SetValue(animationWindow, clip);
        else
            Debug.LogError("Could not find the 'animationClip' property in the AnimationWindow.");
    }
    #endregion


    [HideInInspector]
    public int lightCombosListLength;
    [HideInInspector]
    public int heavyCombosListLength;
    [HideInInspector]
    public int recoveryAttacksListLength;
    [HideInInspector]
    public AudioEntity effortSounds;
    [HideInInspector]
    public AudioEntity swingSounds;
    [HideInInspector]
    public AudioEntity hitSounds;

    [HorizontalGroup("Effects/EffortSound", width:.45f)]
    [ShowIf(nameof(overrideEffortSound))]
    [ReadOnly]
    [HideLabel]
    [SerializeField] private AudioClip effortAudioClip;

    [HorizontalGroup("Effects/HitSound", width: .45f)]
    [ShowIf(nameof(overrideHitSound))]
    [ReadOnly]
    [HideLabel]
    [SerializeField] private AudioClip hitAudioClip;

    [HorizontalGroup("Effects/SwingSound", width: .45f)]
    [ShowIf(nameof(overrideSwingSound))]
    [ReadOnly]
    [HideLabel]
    [SerializeField] private AudioClip swingAudioClip;

    private void SelectAudioClip(AudioEntity audioEntity, ref AudioClip audioClip, AudioClip value, ref int velocity)
    {
        if (audioClip == value) return;

        audioClip = value;

        // Find and set the velocity corresponding to the selected clip.
        BroAudioClip matchingClip = audioEntity.Clips.FirstOrDefault(audio => audio.AudioClip == value);
        if (matchingClip != null)
        {
            velocity = matchingClip.Velocity;
        }
    }

    private IEnumerable<ValueDropdownItem<int>> GetEffortAudioClips() => GetAudioClips(effortSounds);
    private IEnumerable<ValueDropdownItem<int>> GetSwingAudioClips() => GetAudioClips(swingSounds);
    private IEnumerable<ValueDropdownItem<int>> GetHitAudioClips() => GetAudioClips(hitSounds);

    private IEnumerable<ValueDropdownItem<int>> GetAudioClips(AudioEntity audioEntity)
    {
        for (int i = 0; i < audioEntity.Clips.Count(); i++)
        {
            BroAudioClip BroAudioClip = audioEntity.Clips[i];
            AudioClip SelectedAudioClip = BroAudioClip.AudioClip;
            int velocity = BroAudioClip.Velocity;
            yield return new ValueDropdownItem<int>("[" + velocity + "] " + SelectedAudioClip.name, velocity);
        }
    }

    private void SetEffortAudioClip() => GetSelectedAudioClip(effortSounds, ref effortVelocity, ref effortAudioClip);
    private void SetSwingAudioClip() => GetSelectedAudioClip(swingSounds, ref swingVelocity, ref swingAudioClip);
    private void SetHitAudioClip() => GetSelectedAudioClip(hitSounds, ref hitVelocity, ref hitAudioClip);

    private void GetSelectedAudioClip(AudioEntity audioEntity, ref int velocity, ref AudioClip audioClip)
    {
        foreach (BroAudioClip broAudioClip in audioEntity.Clips)
            if (broAudioClip.Velocity == velocity)
                audioClip = broAudioClip.AudioClip;
    }

    private IEnumerable<ValueDropdownItem<int>> GetLightCombosDropdown()
    {
        for (int i = 0; i < lightCombosListLength; i++)
            yield return new ValueDropdownItem<int>("Light Combo" + (i + 1), i);
    }

    private IEnumerable<ValueDropdownItem<int>> GetHeavyCombosDropdown()
    {
        for (int i = 0; i < heavyCombosListLength; i++)
            yield return new ValueDropdownItem<int>("Heavy Combo" + (i + 1), i);
    }

    private IEnumerable<ValueDropdownItem<int>> GetRecoveryDropdown()
    {
        for (int i = 0; i < recoveryAttacksListLength; i++)
            yield return new ValueDropdownItem<int>("Recovery Attack" + (i + 1), i);
    }

    private bool HasLightComboTransition()
    {
        return !IsAimAttack && canCombo && (IsHeavyAttack || (!IsHeavyAttack && !IsComboAttack && !hasRecoveryAttack));
    }
    private bool HasHeavyComboTransition()
    {
        return !IsAimAttack && canCombo && (!IsHeavyAttack || (IsHeavyAttack && !IsComboAttack && !hasRecoveryAttack));
    }
    private bool NotComboAttack()
    {
        if (!IsComboAttack && !IsAimAttack) return true;
        else { if (!canCombo) canCombo = true; return false; }
    }
    private bool CanHaveRecoveryAttack() { return !IsComboAttack && !IsRecoveryAttack && !IsAimAttack; }
    private bool ShowAnimation() { return animatorOverrideController != null; }
#endif
#endregion
}
public enum AttackClipType
{
    HeavyCombo1,
    HeavyCombo2,
    HeavyCombo3,
    HeavyChargedAttack,
    LightCombo1,
    LightCombo2,
    LightChargedAttack,
    HeavyAerialAttack,
    LightAerialAttack,
    HeavySprintAttack,
    LightSprintAttack,
    HeavyAimAttack,
    LightAimAttack,
    RecoveryAttack1,
    RecoveryAttack2,
    FinisherAttack,
}
