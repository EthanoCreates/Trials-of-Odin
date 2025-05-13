public interface IAimAttacks
{
    public enum EAimAttackTypes
    {
        Throw,
        Shoot,
    }

    void HeavyAimAttack();
    EAimAttackTypes HeavyAimAttackType { get; }
    void LightAimAttack();
    EAimAttackTypes LightAimAttackType { get; }
    public int GetAmmoAmount();
    public bool IsReloaded();
}
