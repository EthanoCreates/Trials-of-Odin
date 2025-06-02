namespace TrialsOfOdin.State
{
    public class CombatOrientState : PlayerState
    {
        public CombatOrientState(StateUtilities utilities, ECharacterState stateKey) : base(utilities, stateKey) { }


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

        public override ECharacterState GetNextState()
        {
            return StateKey;
        }

        public override void UpdateState() { }
    }
}