using UnityEngine;

public class PlayerRunState : PlayerState
{
    private float runSpeed;
    private float sprintSpeed;
    private float sprintTime;
    private bool canTacticalSprint;
    private float runToSprintLerp;

    public PlayerRunState(PlayerUtilities utilities, PlayerStateMachine.EPlayerState estate) : base(utilities, estate) { }

    public override void EnterState()
    {
        RunSetUp();
    }

    public override PlayerStateMachine.EPlayerState GetNextState()
    {
        return CheckForStateChange();
    }

    public override void UpdateState()
    {
        Run();
        MovementUtility.MovePlayer();
    }

    public override void ExitState(){ }

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
                runToSprintLerp = Mathf.Lerp(runToSprintLerp, sprintSpeed, 0.03f);
                AnimationRequestor.AnimateLocomotion(MovementUtility.HandleMovement(runToSprintLerp), MovementUtility.Speed);
                return;
            }
        }

        // Reset tactical sprint if conditions are not met
        canTacticalSprint = false;

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
