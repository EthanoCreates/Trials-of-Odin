using System;
using TrialsOfOdin.State;
using UnityEngine;

public class PlayerMovementUtility
{
    public event EventHandler OnTransformToCamera;

    protected CharacterController characterController;
    public Transform Transform { get; private set; }
    public Transform PlayerCamera { get; private set; }
    public Vector3 TargetDirection { get; set; }
    public float TurnAmount { get; private set; }
    public float Speed { get; set; }
    public float VerticalVelocity { get; set; }
    public float RotationSmoothTime { get; private set; } = 0.12f;
    public float SpeedChangeRate { get; private set; } = 10.0f;
    public float Gravity { get; private set; } = -9.81f;

    private float rotationVelocity = 0;
    private float targetRotation;
    private const float speedOffset = 0.1f;
    Vector3 verticalVelocityV3 = new(0f, 0f, 0f); 
    private Vector2 movementInput;

    //Used in aim/guard turning
    private bool isMoving = false;
    private float turnSpeed = 5f;
    private float turnMagnitude = 10f;
    private float currentTurnValue = 0f;
    private float targetTurnValue = 0f;
    public PlayerMovementUtility()
    {
        this.characterController = PlayerStateMachine.LocalInstance.CharacterController;
        this.Transform = characterController.transform;
        this.PlayerCamera = MainCamera.Instance.transform;
    }

    //for moving the player the set velocity and direction
    public void MovePlayer()
    {
        verticalVelocityV3.y = VerticalVelocity;
        characterController.Move(TargetDirection.normalized * (Speed * Time.deltaTime) +
                         verticalVelocityV3 * Time.deltaTime);
    }

    public Vector3 HandleMovement(float targetSpeed)
    {
        movementInput = GameInput.Instance.GetMovementInput().ReadValue<Vector2>();

        if (movementInput == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            Speed = Mathf.Round(Speed * 1000f) / 1000f;
        }
        else
        {
            Speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(movementInput.x, 0.0f, movementInput.y).normalized;

        if (movementInput != Vector2.zero)
        {
            RotatePlayer(inputDirection);
        }

        TargetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        return new Vector3(inputDirection.x, 0f, inputDirection.z);
    }

    private void RotatePlayer(Vector3 inputDirection)
    {
        targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + PlayerCamera.eulerAngles.y;

        float rotation = Mathf.SmoothDampAngle(Transform.eulerAngles.y, PlayerCamera.eulerAngles.y, ref rotationVelocity, RotationSmoothTime);

        Transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }
    public void ApplyGravity(float GravityMultiplier)
    {
        VerticalVelocity += Gravity * Time.deltaTime * GravityMultiplier;
    }

    public void TransformToCamera()
    {
        OnTransformToCamera?.Invoke(this, EventArgs.Empty);
    }

    private void TurnIfNotMoving()
    {
        if (!isMoving && Speed > 2) isMoving = true;
        else if (isMoving && Speed <= 2) isMoving = false;
    }

    public bool TurnToFaceAimDirection()
    {
        TurnIfNotMoving();

        if (isMoving) return false;

        Vector3 cameraForward = PlayerCamera.forward.Flatten();

        Vector3 playerForward = Transform.forward.Flatten();

        float angleDifference = Vector3.SignedAngle(playerForward, cameraForward, Vector3.up);

        targetTurnValue = Mathf.Clamp(angleDifference / 180f, -1f, 1f);

        currentTurnValue = Mathf.Lerp(currentTurnValue, targetTurnValue, turnSpeed * Time.deltaTime);

        TurnAmount = currentTurnValue * turnMagnitude;

        TransformToCamera();
        return true;
    }
}
