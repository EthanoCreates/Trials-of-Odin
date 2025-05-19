#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using TrialsOfOdin.Combat;
using UnityEditor;

public class WeaponEditor : OdinMenuEditorWindow
{
    [MenuItem("Tools/Weapon Editor 4.0")]

    private static void OpenWindow()
    {
        GetWindow<WeaponEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        tree.AddAllAssetsAtPath("Weapon Data", "Assets/ScriptableObjects/Weapons/PlayerWeapons", typeof(WeaponSO), true, true);

        tree.EnumerateTree().AddIcons<WeaponSO>(x => x.Icon);

        return tree;
    }
}
#endif
