using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Ami.BroAudio.Data;
using Ami.BroAudio;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

[CreateAssetMenu(menuName = "CombatSOs/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    #region General_UI_Layout
    protected const string LEFT_VERTICAL_GROUP = "Split/Left";
    protected const string RIGHT_VERTICAL_GROUP = "Split/Right";
    protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";
    protected const string DEBUG = "Split/Right/Debug";
    protected const string HELPERS = "Split/Right/Helpers";
    protected const string weaponFolderPath = "Assets/ScriptableObjects/Weapons/PlayerWeapons/";

    [VerticalGroup(LEFT_VERTICAL_GROUP)]

    [HideLabel, PreviewField(55)]
    [HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 67)]
    public Texture Icon;

    [FoldoutGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    public string Name;

    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    public float damage;

    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [LabelWidth(130f)]
    public float StanceBreakPower;

    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [LabelText("Overrider")]
    public AnimatorOverrideController animatorOverrideController;

    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [FoldoutGroup(GENERAL_SETTINGS_VERTICAL_GROUP + "/Weapon Slots")]
    [LabelWidth(100f)]
    public WeaponHolster.EHolsterType holsterSlot;

    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [FoldoutGroup(GENERAL_SETTINGS_VERTICAL_GROUP + "/Weapon Slots")]
    [LabelWidth(100f)]
    public WeaponHolster.EHolderType holderSlot;

    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [LabelWidth(100f)]
    public bool hasAimAttacks;

    [VerticalGroup(RIGHT_VERTICAL_GROUP)]

    [FoldoutGroup("Split/Right/Description")]
    [HideLabel, TextArea(3, 2)]
    public string Description;

    [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
    [FoldoutGroup("Split/Right/Notes")]
    [HideLabel, TextArea(2, 2)]
    public string Notes;

    [FoldoutGroup(LEFT_VERTICAL_GROUP + "/FX")]

    [FoldoutGroup(LEFT_VERTICAL_GROUP + "/FX" + "/SFX")]
    public SoundID swingSounds;

    public AudioEntity swingAsset { get { return GetAudioEntityFromID(swingSounds); } }

    [FoldoutGroup(LEFT_VERTICAL_GROUP + "/FX" + "/SFX")]
    public SoundID hitSounds;
    public AudioEntity hitAsset  { get { return GetAudioEntityFromID(hitSounds); } }
        
    [FoldoutGroup(LEFT_VERTICAL_GROUP + "/FX" + "/SFX")]
    public SoundID effortSounds;
    public AudioEntity effortAsset { get { return GetAudioEntityFromID(effortSounds); } }

    public AudioEntity GetAudioEntityFromID(SoundID soundID)
    {
        SoundID.TryGetAsset(soundID, out AudioAsset asset);
        if (asset == null) return null;
        foreach (AudioEntity audioEntity in asset.Entities)
        {
            if (audioEntity.ID == soundID.ID)
            {
                return audioEntity;
            }
        }
        return null;
    }


    [FoldoutGroup(LEFT_VERTICAL_GROUP + "/FX" + "/VFX")]
    [SerializeField] public List<GameObject> hitVFX = new List<GameObject>();

    [FoldoutGroup(DEBUG)]
    public bool animatorOverride;
    #endregion
    #region Helpers
#if UNITY_EDITOR

    [BoxGroup(HELPERS + "/Create New AttackSO/Data")]
    [LabelText("Animation State")]
    [SerializeField] private AttackClipType helperAnimationState;
    [BoxGroup(HELPERS + "/Create New AttackSO/Data")]
    [LabelText("Damage Multiplier")]
    [Range(0.1f, 5f)]
    [SerializeField] private float helperDamageMultiplier = 1f;
    [BoxGroup(HELPERS + "/Create New AttackSO/Data")]
    [LabelText("Animation Speed")]
    [Range(0.1f, 3f)]
    [SerializeField] private float helperAnimationSpeed = 1f;
    [BoxGroup(HELPERS + "/Create New AttackSO/Data")]
    private AttackSO createdAttackSO;

    private AttackSO CreateAttackSOAsset(AttackClipType animationState)
    {
        AttackSO attackSO = ScriptableObject.CreateInstance<AttackSO>();
        attackSO.animationState = animationState;

        string attackFolderPath = weaponFolderPath + Name + "/Attacks";

        if (!Directory.Exists(attackFolderPath))
        {
            Directory.CreateDirectory(attackFolderPath);
        }

        string assetName = attackSO.animationState.ToString();
        string assetPath = attackFolderPath + "/" + assetName + ".asset";

        int counter = 1;
        while (AssetDatabase.LoadAssetAtPath<AttackSO>(assetPath) != null)
        {
            assetPath = attackFolderPath + "/" + assetName + "_" + counter + ".asset";
            counter++;
        }

        AssetDatabase.CreateAsset(attackSO, assetPath);
        return attackSO;
    }

    [FoldoutGroup(HELPERS + "/Create New AttackSO")]
    [Button]
    public void CreateNewData()
    {
        AttackSO newAttackSO = CreateAttackSOAsset(helperAnimationState);
        newAttackSO.animationState = helperAnimationState;
        newAttackSO.damageMultiplier = helperDamageMultiplier;
        newAttackSO.animationSpeed = helperAnimationSpeed;

        AssetDatabase.SaveAssets();
        createdAttackSO = newAttackSO;
    }

    [FoldoutGroup(HELPERS)]
    [Button]
    public void ReVisualize()
    {
        DisplayExtraAnimationsFoldout();
        UpdateAllAttackData();
    }

    private enum AutoMovementHelpers
    {
        OneHandedCombat,
        Unarmed,
        Bow,
    }

    [BoxGroup(HELPERS + "/Auto Add Movement Animations/Data")]
    [SerializeField] private string baseAnimPath = "Assets/Plugins/Kevin Iglesias/Melee Warrior Animations/Animations/OneHanded/Movement";
    [BoxGroup(HELPERS + "/Auto Add Movement Animations/Data")]
    [SerializeField] private string walkFbx = "1H@Walk01";
    [BoxGroup(HELPERS + "/Auto Add Movement Animations/Data")]
    [SerializeField] private string runFbx = "1H@Run01";
    [BoxGroup(HELPERS + "/Auto Add Movement Animations/Data")]
    [OnValueChanged("AutoFillPaths")]
    [SerializeField] private AutoMovementHelpers autoSelectPath;

    private void AutoFillPaths()
    {
        switch (autoSelectPath)
        {
            case AutoMovementHelpers.OneHandedCombat:
                baseAnimPath = "Assets/ExtraAssets/Animations/Kevin Iglesias/Melee Warrior Animations/Animations/OneHanded/Movement";
                walkFbx = "1H@Walk01";
                runFbx = "1H@Run01";
                break;
            case AutoMovementHelpers.Unarmed:
                baseAnimPath = "Assets/ExtraAssets/Animations/Kevin Iglesias/Basic Motions/Animations/Movement";
                walkFbx = "BasicMotions@Walk01";
                runFbx = "BasicMotions@Run01";
                break;
            case AutoMovementHelpers.Bow:
                baseAnimPath = "Assets/ExtraAssets/Animations/Kevin Iglesias/Archer Animations/Animations/Movement";
                walkFbx = "Archer@Walk01";
                runFbx = "Archer@Run01";
                break;
        }
    }

    [FoldoutGroup(HELPERS)]
    [Button]
    public void CreateAndAssignDefaultAttackSOs()
    {
        lightCombos = new List<AttackSO>() { CreateAttackSOAsset(AttackClipType.LightCombo1), CreateAttackSOAsset(AttackClipType.LightCombo2) };
        heavyCombos = new List<AttackSO>() { CreateAttackSOAsset(AttackClipType.HeavyCombo1), CreateAttackSOAsset(AttackClipType.HeavyCombo2), CreateAttackSOAsset(AttackClipType.HeavyCombo3) };
        lightChargedAttack = CreateAttackSOAsset(AttackClipType.LightChargedAttack);
        heavyChargedAttack = CreateAttackSOAsset(AttackClipType.HeavyChargedAttack);
        lightAerialAttack = CreateAttackSOAsset(AttackClipType.LightAerialAttack);
        heavyAerialAttack = CreateAttackSOAsset(AttackClipType.HeavyAerialAttack);
        lightSprintAttack = CreateAttackSOAsset(AttackClipType.LightSprintAttack);
        heavySprintAttack = CreateAttackSOAsset(AttackClipType.HeavySprintAttack);
        lightAimAttack = CreateAttackSOAsset(AttackClipType.LightAimAttack);
        heavyAimAttack = CreateAttackSOAsset(AttackClipType.HeavyAimAttack);
        recoveryAttacks = new List<AttackSO>() { CreateAttackSOAsset(AttackClipType.RecoveryAttack1), CreateAttackSOAsset(AttackClipType.RecoveryAttack2) };
        UpdateAllAttackData();
        AssetDatabase.SaveAssets();
    }

    [FoldoutGroup(HELPERS + "/Auto Add Movement Animations")]
    [Button]
    private void AutoFillOneHandCombatMovement()
    {
        // Define the base path for the FBX files containing movement animations
        string basePathWalk = baseAnimPath + "/" + walkFbx + ".fbx";
        string basePathRun = baseAnimPath + "/" + runFbx + ".fbx";

        // Load the walk and run animation clips from the respective FBX files
        AnimationClip[] walkClips = LoadAnimationClipsFromFBX(basePathWalk);
        AnimationClip[] runClips = LoadAnimationClipsFromFBX(basePathRun);

        if (walkClips == null || runClips == null)
        {
            Debug.LogError("Failed to load animation clips from FBX files.");
            return;
        }

        // Define corresponding animation clip names from the FBX files3
        Dictionary<string, string> animationMappings = new Dictionary<string, string>()
        {
            { "WalkForward", walkFbx + " - Forward" },
            { "WalkBackward", walkFbx + " - Backward" },
            { "WalkLeft", walkFbx + " - Left" },
            { "WalkRight", walkFbx + " - Right" },
            { "WalkForwardLeft", walkFbx + " - ForwardLeft" },
            { "WalkForwardRight", walkFbx + " - ForwardRight" },
            { "WalkBackwardLeft", walkFbx + " - BackwardLeft" },
            { "WalkBackwardRight", walkFbx + " - BackwardRight" },
            { "RunForward", runFbx + " - Forward" },
            { "RunBackward", runFbx +" - Backward" },
            { "RunLeft", runFbx + " - Left" },
            { "RunRight", runFbx + " - Right" },
            { "RunForwardLeft", runFbx + " - ForwardLeft" },
            { "RunForwardRight", runFbx + " - ForwardRight" },
            { "RunBackwardLeft", runFbx + " - BackwardLeft" },
            { "RunBackwardRight", runFbx + " - BackwardRight" },
        };

        // Iterate through the dictionary and assign the corresponding animation clips to the slots
        foreach (var mapping in animationMappings)
        {
            string slotName = mapping.Key;
            string clipName = mapping.Value;

            // Try to find the matching clip from the loaded walk or run clips
            AnimationClip matchingClip = walkClips.FirstOrDefault(c => c.name == clipName) ??
                                         runClips.FirstOrDefault(c => c.name == clipName);

            if (matchingClip != null)
            {

                // Assign the found clip to the AnimatorOverrideController
                AnimatorOverrideController controller = animatorOverrideController; // Replace with your actual controller reference
                controller[slotName] = matchingClip;
            }
            else
            {
                Debug.LogWarning($"Clip {clipName} not found in FBX files.");
            }
        }
        DisplayExtraAnimationsFoldout();
    }

    private AnimationClip[] LoadAnimationClipsFromFBX(string fbxPath)
    {
        // Helper method to load animation clips from an FBX file
        // Load all assets from the FBX file and filter to get only the AnimationClips
        return AssetDatabase.LoadAllAssetsAtPath(fbxPath)
                            .OfType<AnimationClip>()
                            .ToArray();
    }
#endif
    #endregion

    [FoldoutGroup("Attacks")]

    [FoldoutGroup("Attacks/Combos")]
    [OnValueChanged(nameof(UpdateLightComboContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private List<AttackSO> lightCombos;
    public List<AttackSO> LightCombos { get { return lightCombos; } }
    [FoldoutGroup("Attacks/Combos")]
    [OnValueChanged(nameof(UpdateHeavyComboContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private List<AttackSO> heavyCombos;
    public List<AttackSO> HeavyCombos { get { return heavyCombos; } }
    [FoldoutGroup("Attacks/ChargedAttack")]
    [OnValueChanged(nameof(UpdateLightChargedContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO lightChargedAttack;
    public AttackSO LightChargedAttack { get { return lightChargedAttack; } }
    [FoldoutGroup("Attacks/ChargedAttack")]
    [OnValueChanged(nameof(UpdateHeavyChargedContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO heavyChargedAttack;
    public AttackSO HeavyChargedAttack { get { return heavyChargedAttack; } }
    [FoldoutGroup("Attacks/AerialAttack")]
    [OnValueChanged(nameof(UpdateLightAerialContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO lightAerialAttack;
    public AttackSO LightAerialAttack { get { return lightAerialAttack; } }
    [FoldoutGroup("Attacks/AerialAttack")]
    [OnValueChanged(nameof(UpdateHeavyAerialContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO heavyAerialAttack;
    public AttackSO HeavyAerialAttack { get { return heavyAerialAttack; } }
    [FoldoutGroup("Attacks/SprintAttack")]
    [OnValueChanged(nameof(UpdateLightSprintContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO lightSprintAttack;
    public AttackSO LightSprintAttack { get { return lightSprintAttack; } }
    [FoldoutGroup("Attacks/SprintAttack")]
    [OnValueChanged(nameof(UpdateHeavySprintContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO heavySprintAttack;
    public AttackSO HeavySprintAttack { get { return heavySprintAttack; } }
    [ShowIf(nameof(hasAimAttacks))]
    [FoldoutGroup("Attacks/AimAttack")]
    [OnValueChanged(nameof(UpdateLightAimContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO lightAimAttack;
    public AttackSO LightAimAttack { get { return lightAimAttack; } }
    [ShowIf(nameof(hasAimAttacks))]
    [FoldoutGroup("Attacks/AimAttack")]
    [OnValueChanged(nameof(UpdateHeavyAimContext))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO heavyAimAttack;
    public AttackSO HeavyAimAttack { get { return heavyAimAttack; } }
    [OnValueChanged(nameof(UpdateFinisherContext))]
    [FoldoutGroup("Attacks/FinisherAttack")]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private AttackSO finisherAttack;
    public AttackSO FinisherAttack { get { return finisherAttack; } }

    [OnValueChanged(nameof(UpdateRecoveryContext))]
    [FoldoutGroup("Attacks")]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [SerializeField] private List<AttackSO> recoveryAttacks;
    public List<AttackSO> RecoveryAttacks { get { return recoveryAttacks; } }

    [FoldoutGroup("Extra Animations")]
    public List<AnimDataVisualizer> otherWeaponAnims = new List<AnimDataVisualizer>();
    [FoldoutGroup("Extra Animations")]
    [SerializeField] private List<AnimDataVisualizer> movementAnims = new List<AnimDataVisualizer>();

    public AttackSO GetRecoveryAttack(int recoveryAttack)
    {
        return recoveryAttacks[recoveryAttack];
    }
#if UNITY_EDITOR
    private void OnEnable()
    {
        animatorOverrideControllerDebug = animatorOverrideController;
        ReVisualize();
    }

    #region SettingAttackFlags

    private void UpdateAllAttackData()
    {
        UpdateLightComboContext();
        UpdateHeavyComboContext();
        UpdateLightChargedContext();
        UpdateHeavyChargedContext();
        UpdateLightAerialContext();
        UpdateHeavyAerialContext();
        UpdateLightSprintContext();
        UpdateHeavySprintContext();
        UpdateLightAimContext();
        UpdateHeavyAimContext();
        UpdateFinisherContext();
        UpdateRecoveryContext();
    }

    private void UpdateLightComboContext()
    {
        foreach (AttackSO attackData in lightCombos)
        {
            SetAllAttackFlagsFalse(attackData);
            UpdateAllInjections(attackData);
            attackData.IsComboAttack = true;
        }
    }
    private void UpdateHeavyComboContext()
    {
        foreach (AttackSO attackData in heavyCombos)
        {
            SetAllAttackFlagsFalse(attackData);
            UpdateAllInjections(attackData);
            attackData.IsComboAttack = true;
            attackData.IsHeavyAttack = true;
        }
    }
    private void UpdateLightChargedContext()
    {
        if (lightChargedAttack == null) return;
        SetAllAttackFlagsFalse(lightChargedAttack);
        UpdateAllInjections(lightChargedAttack);
        lightChargedAttack.IsChargedAttack = true;
    }
    private void UpdateHeavyChargedContext()
    {
        if (heavyChargedAttack == null) return;
        SetAllAttackFlagsFalse(heavyChargedAttack);
        UpdateAllInjections(heavyChargedAttack);
        heavyChargedAttack.IsHeavyAttack = true;
        heavyChargedAttack.IsChargedAttack = true;
    }
    private void UpdateLightAerialContext()
    {
        if (lightAerialAttack == null) return;
        SetAllAttackFlagsFalse(lightAerialAttack);
        UpdateAllInjections(lightAerialAttack);
    }
    private void UpdateHeavyAerialContext()
    {
        if (heavyAerialAttack == null) return;
        SetAllAttackFlagsFalse(heavyAerialAttack);
        UpdateAllInjections(heavyAerialAttack);
        heavyAerialAttack.IsHeavyAttack = true;
    }
    private void UpdateLightSprintContext()
    {
        if (lightSprintAttack == null) return;
        SetAllAttackFlagsFalse(lightSprintAttack);
        UpdateAllInjections(lightSprintAttack);
    }
    private void UpdateHeavySprintContext()
    {
        if (heavySprintAttack == null) return;
        SetAllAttackFlagsFalse(heavySprintAttack);
        UpdateAllInjections(heavySprintAttack);
        heavySprintAttack.IsHeavyAttack = true;
    }
    private void UpdateLightAimContext()
    {
        if (lightAimAttack == null) return;
        SetAllAttackFlagsFalse(lightAimAttack);
        UpdateAllInjections(lightAimAttack);
        lightAimAttack.IsAimAttack = true;

    }
    private void UpdateHeavyAimContext()
    {
        if (heavyAimAttack == null) return;
        SetAllAttackFlagsFalse(heavyAimAttack);
        UpdateAllInjections(heavyAimAttack);
        heavyAimAttack.IsHeavyAttack = true;
        heavyAimAttack.IsAimAttack = true;
    }
    private void UpdateFinisherContext()
    {
        if (finisherAttack == null) return;
        SetAllAttackFlagsFalse(finisherAttack);
        UpdateAllInjections(finisherAttack);
    }

    private void UpdateRecoveryContext()
    {
        foreach (AttackSO attackData in recoveryAttacks)
        {
            SetAllAttackFlagsFalse(attackData);
            UpdateAllInjections(attackData);
            attackData.IsRecoveryAttack = true;
        }
    }

    private void UpdateAllInjections(AttackSO attackSO)
    {
        attackSO.animatorOverrideController = animatorOverrideController;
        attackSO.lightCombosListLength = lightCombos.Count;
        attackSO.heavyCombosListLength = heavyCombos.Count;
        attackSO.recoveryAttacksListLength = recoveryAttacks.Count;
        attackSO.swingSounds = this.swingAsset;
        attackSO.hitSounds = this.hitAsset;
        attackSO.effortSounds = this.effortAsset;
    }
    private void SetAllAttackFlagsFalse(AttackSO attackSO)
    {
        attackSO.IsComboAttack = false;
        attackSO.IsHeavyAttack = false;
        attackSO.IsChargedAttack = false;
        attackSO.IsRecoveryAttack = false;
        attackSO.IsAimAttack = false;
    }
    #endregion

    [ShowIf("animatorOverride")]
    [InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.CompletelyHidden)]
    [SerializeField] private AnimatorOverrideController animatorOverrideControllerDebug;

    private void DisplayExtraAnimationsFoldout()
    {
        movementAnims.Clear();
        otherWeaponAnims.Clear();
        string[] extraAnimationSlots = new string[] {
            "Idle",
            "WalkForward",
            "WalkBackward",
            "WalkLeft",
            "WalkRight",
            "WalkForwardLeft",
            "WalkForwardRight",
            "WalkBackwardLeft",
            "WalkBackwardRight",
            "RunForward",
            "RunBackward",
            "RunLeft",
            "RunRight",
            "RunForwardLeft",
            "RunForwardRight",
            "RunBackwardLeft",
            "RunBackwardRight",
            "Sprint",
            "Block",
            "Jump"
        };

        string[] extraWeaponSlots = new string[] {
            "Equip",
            "UnEquip",
            "AttackPrimer",
            "Reload",
            "Aim",
            "Load",
        };

        foreach (string slotName in extraAnimationSlots)
        {
            movementAnims.Add(new AnimDataVisualizer
            {
                baseAnimName = slotName,
                overrideClip = animatorOverrideController[slotName],
                controller = animatorOverrideController
            });
        }

        foreach (string slotName in extraWeaponSlots)
        {
            otherWeaponAnims.Add(new AnimDataVisualizer
            {
                baseAnimName = slotName,
                overrideClip = animatorOverrideController[slotName],
                controller = animatorOverrideController
            });
        }
    }
}
#endif

#if UNITY_EDITOR
[System.Serializable]
public class AnimDataVisualizer
{
    [HorizontalGroup("animDataVisualizer", Width = 0.35f)]
    [HideLabel]
    [ReadOnly]
    public string baseAnimName;

    [HorizontalGroup("animDataVisualizer", Width = 0.625f)]
    [OnValueChanged("UpdateClip")]
    [HideLabel]
    public AnimationClip overrideClip;
    [HideInInspector]
    public AnimatorOverrideController controller;

    public void UpdateClip()
    {
        if (overrideClip != null)
        {
            if (controller != null)
            {
                controller[baseAnimName] = overrideClip;
            }
        }
    }
}
#endif
