using System;
using UnityEngine;
using TrialsOfOdin.Combat;
using TrialsOfOdin.Stats;
using TrialsOfOdin;
public class StateUtilities
{
    private readonly CharacterStats stats;

    public Action<BaseState<ECharacterState>> UpdateCurrentState;
    public Func<ECharacterState, BaseState<ECharacterState>> GetStateInstance;
    public CharacterContext Context { get; private set; }
    public CombatSystemPro CombatSystem { get; private set; }
    public PlayerMovementUtility MovementUtility { get; private set; }
    public AnimationRequestor AnimationRequestor { get; private set; }
    public SoundRequestor SoundRequestor { get; private set; }

    public StateUtilities(CombatSystemPro combatSystem, CharacterContext humanoidSMContext, Action<BaseState<ECharacterState>> updateCurrentState, Func<ECharacterState, BaseState<ECharacterState>> getStateInstance)
    {
        UpdateCurrentState = updateCurrentState;
        GetStateInstance = getStateInstance;
        Context = humanoidSMContext;
        MovementUtility = new PlayerMovementUtility();
        AnimationRequestor = new AnimationRequestor(combatSystem);
        SoundRequestor = new SoundRequestor();
        CombatSystem = combatSystem;
        this.stats = combatSystem.Stats;
    }

    public void RecoverStamina()
    {
        stats.Stamina.RecoverStamina();
    }
    public bool TrySprintWithStamina()
    {
        return stats.Stamina.TrySprint();
    }

    public bool HasSufficientStamina(float staminaCost)
    {
        return stats.Stamina.TryUseStamina(staminaCost);
    }

    public bool CouldPerformActionWithStamina(float staminaCost)
    {
        return stats.Stamina.HasStamina(staminaCost);
    }
}
