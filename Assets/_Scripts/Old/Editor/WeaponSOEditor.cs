//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using Sirenix.OdinInspector;
//using Sirenix.OdinInspector.Editor;
//using System.IO;
//using System.Linq;

//[CustomEditor(typeof(WeaponSO))]
//public class WeaponSOEditor : OdinEditor
//{
//    private SerializedProperty animatorOverrideController;
//    private static WeaponSO weaponSO;

//    private CreateNewAttackData createNewAttackData;

//    // Dictionary to track foldout states
//    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();


//    protected override void OnEnable()
//    {
//        base.OnEnable();
//        weaponSO = (WeaponSO)target;
//        animatorOverrideController = serializedObject.FindProperty("animatorOverrideController");

//        // Initialize foldout states
//        InitializeFoldoutStates();

//        // Initialize CreateNewAttackData
//        createNewAttackData = new CreateNewAttackData();

//        AnimatorOverrideController controller = (AnimatorOverrideController)animatorOverrideController.objectReferenceValue;

//        FillVisualizer(weaponSO.lightCombos, weaponSO.lightCombosVisualizer, controller);
//        FillVisualizer(weaponSO.heavyCombos, weaponSO.heavyCombosVisualizer, controller);
//        FillVisualizer(weaponSO.chargedLightAttack, weaponSO.chargedLightAttackVisualizer, controller);
//        FillVisualizer(weaponSO.chargedHeavyAttack, weaponSO.chargedHeavyAttackVisualizer, controller);
//        FillVisualizer(weaponSO.sprintAttack, weaponSO.sprintAttackVisualizer, controller);
//        FillVisualizer(weaponSO.aerialAttack, weaponSO.aerialAttackVisualizer, controller);
//        FillVisualizer(weaponSO.aimHeavyAttack, weaponSO.aimHeavyAttackVisualizer, controller);
//        FillVisualizer(weaponSO.aimLightAttack, weaponSO.aimLightAttackVisualizer, controller);
//        FillVisualizer(weaponSO.equip, weaponSO.equipVisualizer, controller);
//        FillVisualizer(weaponSO.unEquip, weaponSO.unequipVisualizer, controller);
//    }

//    private void InitializeFoldoutStates()
//    {
//        foldoutStates["Light Combo Attacks"] = false;
//        foldoutStates["Heavy Combo Attacks"] = false;
//        foldoutStates["Charged Light Attacks"] = false;
//        foldoutStates["Charged Heavy Attacks"] = false;
//        foldoutStates["Sprint Attacks"] = false;
//        foldoutStates["Aerial Attacks"] = false;
//        foldoutStates["Aiming Heavy Attacks"] = false;
//        foldoutStates["Aiming Light Attacks"] = false;
//        foldoutStates["Equip Attacks"] = false;
//        foldoutStates["Unequip Attacks"] = false;
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        // Default WeaponSO Inspector GUI
//        base.OnInspectorGUI();

//        // Show specific override slots from the AnimatorOverrideController
//        AnimatorOverrideController controller = (AnimatorOverrideController)animatorOverrideController.objectReferenceValue;

//        if (controller != null)
//        {
//            //EditorGUILayout.Space(10);
//            //EditorGUILayout.LabelField("Animation Overrides", EditorStyles.boldLabel);

//            // Display AttackSO lists with their corresponding animation slots
//            //DisplayFoldoutGroup("Light Combo Attacks", weaponSO.lightCombos, controller);
//            //DisplayFoldoutGroup("Heavy Combo Attacks", weaponSO.heavyCombos, controller);
//            //DisplayFoldoutGroup("Charged Light Attacks", weaponSO.chargedLightAttack, controller);
//            //DisplayFoldoutGroup("Charged Heavy Attacks", weaponSO.chargedHeavyAttack, controller);
//            //DisplayFoldoutGroup("Sprint Attacks", weaponSO.sprintAttack, controller);
//            //DisplayFoldoutGroup("Aerial Attacks", weaponSO.aerialAttack, controller);
//            //DisplayFoldoutGroup("Aiming Heavy Attacks", weaponSO.aimHeavyAttack, controller);
//            //DisplayFoldoutGroup("Aiming Light Attacks", weaponSO.aimLightAttack, controller);
//            //DisplayFoldoutGroup("Equip Attacks", weaponSO.equip, controller);
//            //DisplayFoldoutGroup("Unequip Attacks", weaponSO.unEquip, controller);


//            //DisplayExtraAnimationsFoldout(controller);



//            EditorGUILayout.Space(10);
//            DrawHelpersSection(controller); // Call the new method to draw the Helpers section
//        }

//        // Display button for creating new AttackSO
//        //DrawCreateNewAttackDataButton();

//        serializedObject.ApplyModifiedProperties();
//    }

//    private void FillVisualizer(List<AttackSO> attackSOs, List<WeaponSO.animationDataVisualizer> visualizerList, AnimatorOverrideController controller)
//    {
//        // Clear the visualizer list before populating it
//        visualizerList.Clear();

//        for (int i = 0; i < attackSOs.Count; i++)
//        {
//            AttackSO attackSO = attackSOs[i];
//            if (attackSO != null)
//            {
//                // Create a new animationDataVisualizer instance
//                var dataVisualizer = new WeaponSO.animationDataVisualizer
//                {
//                    // Get the animation clip based on the AttackSO's clip type from the AnimatorOverrideController
//                    clip = controller[attackSO.attackClipType.ToString()],
//                    attackSO = attackSO,
//                    weaponSO = weaponSO,
//                    list = attackSOs,
//                    indexInList = i,
//                };


//                // Display the animation clip field in the inspector
//                //dataVisualizer.clip = (AnimationClip)EditorGUILayout.ObjectField($"Current Clip ({attackSO.attackClipType})", dataVisualizer.clip, typeof(AnimationClip), false);

//                // Add the visualizer to the corresponding list
//                visualizerList.Add(dataVisualizer);

//                // Update the animation in the AnimatorOverrideController if a new one is assigned
//                if (dataVisualizer.clip != controller[attackSO.attackClipType.ToString()])
//                {
//                    controller[attackSO.attackClipType.ToString()] = dataVisualizer.clip;
//                }
//            }
//        }
//    }

//    private void DrawHelpersSection(AnimatorOverrideController controller)
//    {
//        EditorGUILayout.LabelField("Helpers", EditorStyles.boldLabel);

//        // Draw the Auto Fill Base Attacks button
//        if (GUILayout.Button("Auto Fill Base Attacks"))
//        {
//            AutoFillBaseData();// Call the method from WeaponSO to auto-fill data
//            serializedObject.Update(); // Refresh the inspector
//        }

//        if(GUILayout.Button("Auto Fill One Hand Combat Movement"))
//        {
//            AutoFillOneHandCombatMovement(controller);
//            serializedObject.Update();
//        }
//    }


//    private void DrawCreateNewAttackDataButton()
//    {
//        // Toggle the foldout state
//        bool isExpanded = foldoutStates.ContainsKey("Create New AttackSO") && foldoutStates["Create New AttackSO"];
//        isExpanded = EditorGUILayout.Foldout(isExpanded, "Create New AttackSO", true);
//        foldoutStates["Create New AttackSO"] = isExpanded;

//        if (isExpanded)
//        {
//            // Draw the button for creating new AttackSO
//            if (GUILayout.Button("Create New AttackSO"))
//            {
//                createNewAttackData.CreateNewData();
//                // Refresh lists
//                serializedObject.Update();
//            }

//            // Show fields for setting AttackSO data
//            EditorGUILayout.Space();
//            createNewAttackData.attackName = EditorGUILayout.TextField("Attack Name", createNewAttackData.attackName);
//            createNewAttackData.attackClipType = (AttackSO.AttackClipType)EditorGUILayout.EnumPopup("Attack Clip Type", createNewAttackData.attackClipType);
//            createNewAttackData.damageMultiplier = EditorGUILayout.FloatField("Damage Multiplier", createNewAttackData.damageMultiplier);
//            createNewAttackData.animationSpeed = EditorGUILayout.FloatField("Animation Speed", createNewAttackData.animationSpeed);

//            // Display the newly created AttackSO
//            EditorGUILayout.LabelField("Newly Created AttackSO");
//            createNewAttackData.attackSO = (AttackSO)EditorGUILayout.ObjectField(createNewAttackData.attackSO, typeof(AttackSO), false);
//        }
//    }

//    private void DisplayFoldoutGroup(string groupName, List<AttackSO> attackSOs, AnimatorOverrideController controller)
//    {
//        // Toggle the foldout state
//        bool isExpanded = foldoutStates.ContainsKey(groupName) && foldoutStates[groupName];
//        isExpanded = EditorGUILayout.Foldout(isExpanded, groupName, true);
//        foldoutStates[groupName] = isExpanded;

//        if (isExpanded)
//        {
//            if (attackSOs != null && attackSOs.Count > 0)
//            {
//                for (int i = 0; i < attackSOs.Count; i++)
//                {
//                    AttackSO attackSO = attackSOs[i];
//                    if (attackSO != null)
//                    {
//                        // Convert the enum to a string to pass as a slot name
//                        string slotName = attackSO.attackClipType.ToString();

//                        EditorGUILayout.BeginVertical(GUI.skin.box);

//                        // Display the animation override
//                        ShowAndUpdateAnimationSlot(controller, slotName);

//                        // Display AttackSO data
//                        EditorGUI.BeginChangeCheck();
//                        AttackSO newAttackSO = (AttackSO)EditorGUILayout.ObjectField("AttackSO Asset", attackSO, typeof(AttackSO), false);

//                        if (newAttackSO != attackSO)
//                        {
//                            // Update the list with the new AttackSO
//                            attackSOs[i] = newAttackSO;
//                            EditorUtility.SetDirty(weaponSO);
//                            EditorUtility.SetDirty(newAttackSO); // Ensure the new AttackSO is also marked as dirty
//                        }

//                        attackSO.damageMultiplier = EditorGUILayout.FloatField("Damage Multiplier", attackSO.damageMultiplier);
//                        attackSO.animationSpeed = EditorGUILayout.FloatField("Animation Speed", attackSO.animationSpeed);
//                        if (EditorGUI.EndChangeCheck())
//                        {
//                            // Mark the ScriptableObject as dirty to save changes
//                            EditorUtility.SetDirty(attackSO);
//                        }

//                        // Button to remove AttackSO from the list
//                        if (GUILayout.Button("Remove Attack"))
//                        {
//                            Undo.RecordObject(weaponSO, "Remove Attack");
//                            attackSOs.RemoveAt(i);
//                            EditorUtility.SetDirty(weaponSO);
//                            break; // Break to avoid modifying the list while iterating
//                        }

//                        EditorGUILayout.EndVertical();
//                    }
//                }
//            }
//            else
//            {
//                // Display message when the list is empty
//                EditorGUILayout.LabelField("No attacks found in this category.", EditorStyles.boldLabel);
//            }

//            // Button to add a new AttackSO
//            if (GUILayout.Button("Add New Attack"))
//            {
//                // Create a new AttackSO instance with default values
//                AttackSO newAttackSO = ScriptableObject.CreateInstance<AttackSO>();

//                // Add the new AttackSO to the list
//                if (newAttackSO != null)
//                {
//                    attackSOs.Add(newAttackSO);
//                    EditorUtility.SetDirty(weaponSO);
//                }
//            }
//        }
//    }

//    private void DisplayExtraAnimationsFoldout(AnimatorOverrideController controller)
//    {
//        bool isExpanded = foldoutStates.ContainsKey("Extra Animations") && foldoutStates["Extra Animations"];
//        isExpanded = EditorGUILayout.Foldout(isExpanded, "Extra Animations", true);
//        foldoutStates["Extra Animations"] = isExpanded;

//        if (isExpanded)
//        {
//            // Define a list of extra animation slots you want to show (e.g., "Idle", "Run", "Jump", etc.)
//            string[] extraAnimationSlots = new string[] {
//                    "Idle",
//                    "WalkForward",
//                    "WalkBackward",
//                    "WalkLeft",
//                    "WalkRight",
//                    "WalkForwardLeft",
//                    "WalkForwardRight",
//                    "WalkBackwardLeft",
//                    "WalkBackwardRight",
//                    "RunForward",
//                    "RunBackward",
//                    "RunLeft",
//                    "RunRight",
//                    "RunForwardLeft",
//                    "RunForwardRight",
//                    "RunBackwardLeft",
//                    "RunBackwardRight",
//                    "Sprint",
//                    "Block",
//                    "Jump"

//            }; // Add your extra animations here

//            foreach (string slotName in extraAnimationSlots)
//            {
//                EditorGUILayout.BeginVertical(GUI.skin.box);
//                ShowAndUpdateAnimationSlot(controller, slotName);
//                EditorGUILayout.EndVertical();
//            }
//        }
//    }

//    private void AutoFillOneHandCombatMovement(AnimatorOverrideController myController)
//    {
//        // Define the base path for the FBX files containing movement animations
//        string basePathWalk = "Assets/Plugins/Kevin Iglesias/Melee Warrior Animations/Animations/OneHanded/Movement/1H@Walk01.fbx";
//        string basePathRun = "Assets/Plugins/Kevin Iglesias/Melee Warrior Animations/Animations/OneHanded/Movement/1H@Run01.fbx";
//        string basePathSprint = "Assets/Plugins/Kevin Iglesias/Melee Warrior Animations/Animations/OneHanded/Movement/1H@Sprint01.fbx";

//        // Load the walk and run animation clips from the respective FBX files
//        AnimationClip[] walkClips = LoadAnimationClipsFromFBX(basePathWalk);
//        AnimationClip[] runClips = LoadAnimationClipsFromFBX(basePathRun);
//        AnimationClip[] sprintClips = LoadAnimationClipsFromFBX(basePathSprint);

//        if (walkClips == null || runClips == null)
//        {
//            Debug.LogError("Failed to load animation clips from FBX files.");
//            return;
//        }

//        // Define corresponding animation clip names from the FBX files3
//        Dictionary<string, string> animationMappings = new Dictionary<string, string>()
//    {
//        { "WalkForward", "1H@Walk01 - Forward" },
//        { "WalkBackward", "1H@Walk01 - Backward" },
//        { "WalkLeft", "1H@Walk01 - Left" },
//        { "WalkRight", "1H@Walk01 - Right" },
//        { "WalkForwardLeft", "1H@Walk01 - ForwardLeft" },
//        { "WalkForwardRight", "1H@Walk01 - ForwardRight" },
//        { "WalkBackwardLeft", "1H@Walk01 - BackwardLeft" },
//        { "WalkBackwardRight", "1H@Walk01 - BackwardRight" },
//        { "RunForward", "1H@Run01 - Forward" },
//        { "RunBackward", "1H@Run01 - Backward" },
//        { "RunLeft", "1H@Run01 - Left" },
//        { "RunRight", "1H@Run01 - Right" },
//        { "RunForwardLeft", "1H@Run01 - ForwardLeft" },
//        { "RunForwardRight", "1H@Run01 - ForwardRight" },
//        { "RunBackwardLeft", "1H@Run01 - BackwardLeft" },
//        { "RunBackwardRight", "1H@Run01 - BackwardRight" },
//        { "Sprint", "1H@Sprint01" }
//    };

//        // Iterate through the dictionary and assign the corresponding animation clips to the slots
//        foreach (var mapping in animationMappings)
//        {
//            string slotName = mapping.Key;
//            string clipName = mapping.Value;

//            // Try to find the matching clip from the loaded walk or run clips
//            AnimationClip matchingClip = walkClips.FirstOrDefault(c => c.name == clipName) ??
//                                         runClips.FirstOrDefault(c => c.name == clipName) ??
//                                         sprintClips.FirstOrDefault(c => c.name == clipName);

//            if (matchingClip != null)
//            { 
            
//                // Assign the found clip to the AnimatorOverrideController
//                AnimatorOverrideController controller = myController; // Replace with your actual controller reference
//                controller[slotName] = matchingClip;

//                // Mark the controller as dirty to save changes
//                EditorUtility.SetDirty(controller);
//            }
//            else
//            {
//                Debug.LogWarning($"Clip {clipName} not found in FBX files.");
//            }
//        }
//    }

//    // Helper method to load animation clips from an FBX file
//    private AnimationClip[] LoadAnimationClipsFromFBX(string fbxPath)
//    {
//        // Load all assets from the FBX file and filter to get only the AnimationClips
//        return AssetDatabase.LoadAllAssetsAtPath(fbxPath)
//                            .OfType<AnimationClip>()
//                            .ToArray();
//    }


//    private void AutoFillBaseData()
//    {
//        // If you're using AssetDatabase (Editor only)
//#if UNITY_EDITOR
//        string basePath = "Assets/ScriptableObjects/Weapons/PlayerWeapons/BaseWeaponSOs/";

//        // Light Combo Attacks
//        AttackSO baseLightCombo1 = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseLightCombo1.asset");
//        AttackSO baseLightCombo2 = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseLightCombo2.asset");
//        AssignToList(ref weaponSO.lightCombos, baseLightCombo1, baseLightCombo2);

//        // Heavy Combo Attacks
//        AttackSO baseHeavyCombo1 = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseHeavyCombo1.asset");
//        AttackSO baseHeavyCombo2 = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseHeavyCombo2.asset");
//        AttackSO baseHeavyCombo3 = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseHeavyCombo3.asset");
//        AssignToList(ref weaponSO.heavyCombos, baseHeavyCombo1, baseHeavyCombo2, baseHeavyCombo3);

//        // Charged Light and Heavy Attacks
//        AttackSO baseChargedLightAttack = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseChargedLightAttack.asset");
//        AttackSO baseChargedHeavyAttack = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseChargedHeavyAttack.asset");
//        AssignToList(ref weaponSO.chargedLightAttack, baseChargedLightAttack);
//        AssignToList(ref weaponSO.chargedHeavyAttack, baseChargedHeavyAttack);

//        // Special Attacks - Sprint
//        AttackSO baseHeavySprintAttack = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseHeavySprintAttack.asset");
//        AttackSO baseLightSprintAttack = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseLightSprintAttack.asset");
//        AssignToList(ref weaponSO.sprintAttack, baseHeavySprintAttack, baseLightSprintAttack);

//        // Aerial Attack
//        AttackSO baseAerialAttack = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseAerialAttack.asset");
//        AssignToList(ref weaponSO.aerialAttack, baseAerialAttack);

//        // Aiming Attacks
//        AttackSO baseAimHeavyAttack = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseAimHeavyAttack.asset");
//        AttackSO baseAimLightAttack = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseAimLightAttack.asset");
//        AssignToList(ref weaponSO.aimHeavyAttack, baseAimHeavyAttack);
//        AssignToList(ref weaponSO.aimLightAttack, baseAimLightAttack);

//        // Equip and Unequip Attacks
//        AttackSO baseEquip1 = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseEquip.asset");
//        AttackSO baseUnEquip1 = AssetDatabase.LoadAssetAtPath<AttackSO>(basePath + "BaseUnEquip.asset");
//        AssignToList(ref weaponSO.equip, baseEquip1);
//        AssignToList(ref weaponSO.unEquip, baseUnEquip1);

//#endif
//    }

//    // Helper method to assign AttackSO to lists
//    private void AssignToList(ref List<AttackSO> list, params AttackSO[] items)
//    {
//        if (list == null)
//            list = new List<AttackSO>();

//        list.Clear();
//        list.AddRange(items);
//    }


//    private void ShowAndUpdateAnimationSlot(AnimatorOverrideController controller, string slotName)
//    {
//        if (controller == null)
//        {
//            EditorGUILayout.LabelField("Controller is null");
//            return;
//        }

//        // Retrieve the animation clip assigned to the slot
//        AnimationClip currentClip = controller[slotName];
//        if (currentClip == null)
//        {
//            EditorGUILayout.LabelField($"Slot {slotName} not found");
//            return;
//        }

//        EditorGUI.BeginChangeCheck();

//        // Display the current animation clip and allow changing it
//        AnimationClip newClip = (AnimationClip)EditorGUILayout.ObjectField($"Current Clip ({slotName})", currentClip, typeof(AnimationClip), false);

//        if (EditorGUI.EndChangeCheck())
//        {
//            // Update the controller with the new animation clip
//            controller[slotName] = newClip;
//            EditorUtility.SetDirty(controller);
//        }
//    }



//    public class CreateNewAttackData
//    {
//        public CreateNewAttackData()
//        {
//            attackSO = ScriptableObject.CreateInstance<AttackSO>();
//        }

//        [InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Hidden)]
//        public AttackSO attackSO;

//        [SerializeField] public string attackName;
//        [SerializeField] public AttackSO.AttackClipType attackClipType;
//        [SerializeField] public float damageMultiplier;
//        [SerializeField] public float animationSpeed;

//        public void CreateNewData()
//        {
//            if (string.IsNullOrEmpty(attackName))
//            {
//                EditorUtility.DisplayDialog("Invalid Input", "Please enter a name for the AttackSO.", "OK");
//                return;
//            }

//            // Create a new instance of AttackSO
//            AttackSO newAttackSO = ScriptableObject.CreateInstance<AttackSO>();
//            newAttackSO.attackClipType = attackClipType;
//            newAttackSO.damageMultiplier = damageMultiplier;
//            newAttackSO.animationSpeed = animationSpeed;

//            string directoryPath = "Assets/ScriptableObjects/Weapons/PlayerWeapons/" + weaponSO.name + "/Animations";
//            string path = Path.Combine(directoryPath, attackName + ".asset");

//            // Check if the directory exists, if not, create it
//            if (!Directory.Exists(directoryPath))
//            {
//                Directory.CreateDirectory(directoryPath);
//            }

//            // Create the asset
//            AssetDatabase.CreateAsset(newAttackSO, path);
//            AssetDatabase.CreateAsset(newAttackSO, path);
//            AssetDatabase.SaveAssets();

//            // Update the attackSO reference
//            attackSO = newAttackSO;
//        }
//    }
//}

