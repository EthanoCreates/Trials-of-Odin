using UnityEngine;

namespace TrialsOfOdin.State
{
    public class RunState : PlayerState
    {
        private float runSpeed;
        private float sprintSpeed;
        private float sprintTime;
        private bool canTacticalSprint;
        private float runToSprintLerp;

        public RunState(StateUtilities utilities, ECharacterState estate) : base(utilities, estate) { }

        public override void EnterState()
        {
            RunSetUp();
        }

        public override ECharacterState GetNextState()
        {
            return CheckForStateChange();
        }

        public override void UpdateState()
        {
            Run();
            MovementUtility.MovePlayer();
        }

        public override void ExitState() { CombatSystem.IsSprinting = false; }

        private void Run()
        {
            // Check if we can start tactical sprinting
            if (!canTacticalSprint)
            {
                //may need to use other method here used in setup
                canTacticalSprint = GameInput.Instance.controllerTacticalSprintCheck();
            }

            // Handle tactical sprinting if applicable
            if (canTacticalSprint && Context.SprintTimer > 0 && GameInput.Instance.GetMovementInput().ReadValue<Vector2>().y > 0.7f)
            {
                if (Utilities.TrySprintWithStamina())
                {
                    CombatSystem.IsSprinting = true;
                    runToSprintLerp = Mathf.Lerp(runToSprintLerp, sprintSpeed, 0.03f);
                    AnimationRequestor.AnimateLocomotion(MovementUtility.HandleMovement(runToSprintLerp), MovementUtility.Speed);
                    return;
                }
            }

            // Reset tactical sprint if conditions are not met
            canTacticalSprint = false;
            CombatSystem.IsSprinting = false;

            // Recover sprint timer if below max
            if (Context.SprintTimer <= sprintTime)
            {
                Utilities.RecoverStamina();
            }

            // Handle normal running
            AnimationRequestor.AnimateLocomotion(MovementUtility.HandleMovement(runSpeed), MovementUtility.Speed);
        }

        private void RunSetUp()
        {
            runSpeed = Context.RunSpeed;
            runToSprintLerp = runSpeed;
            sprintSpeed = Context.SprintSpeed;
            sprintTime = Context.SprintTime;
            Context.SprintTimer = sprintTime;
            canTacticalSprint = GameInput.Instance.IsTacticalSprinting();
        }
    }
}