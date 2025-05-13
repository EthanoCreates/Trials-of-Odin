using UnityEngine;

public abstract class HumanoidCombatManager
{
    public abstract void Finisher(GameObject player);

    public bool isFinisherable;
    public bool beingFinishered;
}