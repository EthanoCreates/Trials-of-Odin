using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "PlayerSOs/PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float sprintSpeed = 9f;
    public float sprintTime = 15f;
    public float jumpHeight = 2f;

    [FoldoutGroup("Layers")]
    public LayerMask groundLayers;
    [FoldoutGroup("Layers")]
    public LayerMask vaultLayers;
    [FoldoutGroup("Layers")]
    public LayerMask climbLayers;
}
