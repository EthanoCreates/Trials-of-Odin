using UnityEngine;

[CreateAssetMenu(menuName = "EnemySOs/EnemyStatesSO")]
public class EnemyStatesSO : ScriptableObject
{
    public EnemyStateDictionary.GroundedState groundedState;
    public EnemyStateDictionary.LandState landState;
    public EnemyStateDictionary.IdleState idleState;
    public EnemyStateDictionary.PatrolState patrolState;
    public EnemyStateDictionary.ChaseState chaseState;
    public EnemyStateDictionary.SearchState searchState;
    public EnemyStateDictionary.AttackState attackState;
    public EnemyStateDictionary.BlockState blockState;
    public EnemyStateDictionary.StunnedState stunnedState;
    public EnemyStateDictionary.FallingState fallState;
}
