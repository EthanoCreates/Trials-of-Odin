public class PlayerCombatOrientState : PlayerState
{
    public PlayerCombatOrientState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState stateKey) : base(utilities, stateKey) { }


    public override void EnterState()
    {
        AnimationRequestor.OnAttackAnim += AnimationRequestor_OnAttackAnim;
    }

    private void AnimationRequestor_OnAttackAnim(object sender, AttackAnimationData e)
    {
        RotateTransformToCamera();
    }

    private void RotateTransformToCamera()
    {
        MovementUtility.TransformToCamera();
    }

    public override void ExitState() { }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return StateKey;
    }

    public override void UpdateState() { }
}