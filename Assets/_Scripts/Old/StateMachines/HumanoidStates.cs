
namespace TrialsOfOdin
{
    public enum ECharacterState
    {
        Grounded, //Root State

        Block, //walk sub state
        Attack,
        Aim,
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

        AerialMovement,
    }
}
