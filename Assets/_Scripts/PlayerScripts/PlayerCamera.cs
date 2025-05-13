using FIMSpace.FProceduralAnimation;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public event EventHandler OnNewTargetSelected;

    [Header("Cinemachine References")]
    [SerializeField] private CinemachineCamera normalViewVirtualCamera;
    [SerializeField] private CinemachineCamera aimViewVirtualCamera;
    [SerializeField] private Transform cinemachineCameraTarget;

    [Space(10)]
    [Header("Useful Camera Values")]
    [SerializeField] private float lockOnRadius = 20;
    [SerializeField] private float minimumViewableAngle = -50;
    [SerializeField] private float maximumViewableAngle = 50;
    [SerializeField] private bool lockedOn;
    [SerializeField] private bool lockingOn;
    [ShowIf(nameof(lockedOn))]
    [SerializeField] private EnemyTarget currentTarget;
    [SerializeField] private Transform leftLockOnTarget;
    [SerializeField] private Transform rightLockOnTarget;
    private List<Transform> availibleTargets = new List<Transform>();
    [SerializeField] private bool lockCameraPosition = false;
    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -50.0f;
    [SerializeField] private bool isAiming = false;
    [SerializeField] private float lockOnFollowSpeed = .2f;
    [SerializeField] private float playerCorrectionRotationSpeed = 10f;
    private bool canSwitchTarget = true;

    // cinemachine x & y
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private const float lookThreshold = 0.01f;
    private Transform playerTransform;



    void Start()
    {
        PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;
        if (localPlayerStateMachine.gameObject == this.transform.parent.gameObject)
        {
            playerTransform = localPlayerStateMachine.transform;
            cinemachineTargetYaw = cinemachineCameraTarget.rotation.eulerAngles.y;

            Cursor.lockState = CursorLockMode.Locked;

            localPlayerStateMachine.Context.OnResetCamera += LocalInstance_OnResetCamera;
            localPlayerStateMachine.MovementUtility.OnTransformToCamera += LocalInstance_OnTransformToCamera;


            normalViewVirtualCamera.Priority = 2;
            aimViewVirtualCamera.Priority = 1;
        }
        else
        {
            normalViewVirtualCamera.Priority = 0;
            aimViewVirtualCamera.Priority = 0;
        }
    }

    private void Instance_OnLockOn(object sender, EventArgs e)
    {
        lockedOn = !lockedOn;

        if(lockedOn)
        {
            lockingOn = true;
            GetTargetEnemy();
        }
        else
        {
            cinemachineTargetYaw = playerTransform.eulerAngles.y;
            ClearLockOnTargets();
        }
    }

    private void LocalInstance_OnTransformToCamera(object sender, EventArgs e)
    {
        TransformToCamera();
    }

    private void LocalInstance_OnResetCamera(object sender, System.EventArgs e)
    {
        CameraToForward();
    }

    private void OnEnable()
    {
        GameInput gameInput = GameInput.Instance;

        gameInput.OnAimStarted += Aim_started;
        gameInput.OnAimFinished += Aim_canceled;
        gameInput.OnLockOn += Instance_OnLockOn;
    }

    private void OnDisable()
    {
        GameInput gameInput = GameInput.Instance;

        gameInput.OnAimStarted -= Aim_started;
        gameInput.OnAimFinished -= Aim_canceled;
        gameInput.OnLockOn -= Instance_OnLockOn;

        PlayerStateMachine localPlayerStateMachine = PlayerStateMachine.LocalInstance;

        localPlayerStateMachine.Context.OnResetCamera -= LocalInstance_OnResetCamera;
        localPlayerStateMachine.MovementUtility.OnTransformToCamera -= LocalInstance_OnTransformToCamera;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void FixedUpdate()
    {
      
    }

    private void GetTargetEnemy()
    {
        float shortestDistance = Mathf.Infinity;

        ScanForTargets(1f);
        
        //geting closest target
        for(int j = 0; j < availibleTargets.Count; j++)
        {
            float distanceFromTarget = Vector3.Distance(playerTransform.position, availibleTargets[j].position);

            if(distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                currentTarget = availibleTargets[j].GetComponent<EnemyTarget>();
            }
        }
        if (currentTarget != null) {
            StopCoroutine(LockingOn());
            StartCoroutine(LockingOn());
            currentTarget.ActivateTargetUI();
            OnNewTargetSelected?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            lockedOn = false;
        }
    }

    private void GetLeftAndRightTarget()
    {
        ScanForTargets(2f);

        float shortestDistanceOfRightTarget = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;

        //getting left and right targets
        for (int j = 0; j < availibleTargets.Count; j++)
        {
            Vector3 relativeEnemyPosition = MainCamera.Instance.transform.InverseTransformPoint(availibleTargets[j].transform.position);
            var distanceFromLeftTarget = relativeEnemyPosition.x;
            var distanceFromRightTarget = relativeEnemyPosition.x;

            if (availibleTargets[j] == currentTarget.transform) continue;


            if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
            {
                shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                leftLockOnTarget = availibleTargets[j];
            }
            else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
            {
                shortestDistanceOfRightTarget = distanceFromRightTarget;
                rightLockOnTarget = availibleTargets[j];
            }
        }
    }

    private void ScanForTargets(float angleMultiplier)
    {
        Collider[] colliders = Physics.OverlapSphere(playerTransform.position, lockOnRadius, WorldUtilityManager.Instance.TargetLayer);

        Transform camTransform = MainCamera.Instance.transform;

        for (int i = 0; i < colliders.Length; i++)
        {
            Transform lockOnTransform = colliders[i].transform;
            Vector3 lockOnPosition = lockOnTransform.position;

            Vector3 lockOnTargetsDirection = lockOnPosition - camTransform.position;
            float viewableAngle = Vector3.Angle(lockOnTargetsDirection, camTransform.forward);

            if (viewableAngle > (minimumViewableAngle * angleMultiplier) && viewableAngle < (maximumViewableAngle * angleMultiplier))
            {
                RaycastHit hit;
                //Environment check
                if (Physics.Linecast(camTransform.position, lockOnPosition, out hit, WorldUtilityManager.Instance.EnvironmentLayer))
                    continue; 
                else
                    availibleTargets.Add(lockOnTransform);
            }

        }
    }

    private void SwitchLockOnTarget(bool right)
    {
        rightLockOnTarget = null;
        leftLockOnTarget = null;

        GetLeftAndRightTarget();

        if (right)
        {
            if (rightLockOnTarget != null)
            {
                currentTarget.DisableTargetUI();
                currentTarget = rightLockOnTarget.GetComponent<EnemyTarget>();
                currentTarget.ActivateTargetUI();
            }
        }
        else if (leftLockOnTarget != null) 
        {
            currentTarget.DisableTargetUI();
            currentTarget = leftLockOnTarget.GetComponent<EnemyTarget>();
            currentTarget.ActivateTargetUI();
        }
    }


    private void ClearLockOnTargets()
    {
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        currentTarget.DisableTargetUI();
        currentTarget = null;
        availibleTargets.Clear();
    }

    private void CameraRotation()
    {
        if (lockedOn)
        {
            if (lockingOn) return;
            CheckForSwitchTarget();

            if (!currentTarget.gameObject.activeSelf) { ClearLockOnTargets(); lockedOn = false; TransformToCamera(); return; }

            normalViewVirtualCamera.LookAt = currentTarget.transform;
            TransformToCamera();
        }
        else
        {
            normalViewVirtualCamera.LookAt = null;
            // if there is an input and camera position is not fixed
            if (GameInput.Instance.GetLookInput().ReadValue<Vector2>().sqrMagnitude >= lookThreshold && !lockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = GameInput.Instance.GetCurrentInputDevice() is Mouse ? 1.0f : Time.deltaTime;

                cinemachineTargetYaw += GameInput.Instance.GetLookInput().ReadValue<Vector2>().x * deltaTimeMultiplier;
                cinemachineTargetPitch += GameInput.Instance.GetLookInput().ReadValue<Vector2>().y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.rotation = Quaternion.Euler(cinemachineTargetPitch,
                cinemachineTargetYaw, 0.0f);
        }
    }

    private void CheckForSwitchTarget()
    {
        Vector2 lookInput = GameInput.Instance.GetLookInput().ReadValue<Vector2>();
        float lookThreshold = 0.8f;  // Controller stick threshold
        float mouseThreshold = 20f;  // Adjust based on sensitivity
        float mouseResetThreshold = 5f;
        float resetThreshold = 0.5f; // Must move below this to reset switch

        if (GameInput.Instance.GetCurrentInputDevice() is Mouse)
        {
            // Detect fast left/right mouse movement
            if (canSwitchTarget)
            {
                if (lookInput.x > mouseThreshold)
                {
                    SwitchLockOnTarget(right: true);
                    canSwitchTarget = false;
                }
                else if (lookInput.x < -mouseThreshold)
                {
                    SwitchLockOnTarget(right: false);
                    canSwitchTarget = false;
                }
            }
        }
        else
        {
            // Controller stick movement to the right/left
            if (canSwitchTarget)
            {
                if (lookInput.x > lookThreshold)
                {
                    SwitchLockOnTarget(right: true);
                    canSwitchTarget = false;
                }
                else if (lookInput.x < -lookThreshold)
                {
                    SwitchLockOnTarget(right: false);
                    canSwitchTarget = false;
                }
            }
        }

        // Reset switch permission when input drops below reset threshold
        if ((Mathf.Abs(lookInput.x) < resetThreshold) || (GameInput.Instance.GetCurrentInputDevice() is Mouse && Mathf.Abs(lookInput.x) == mouseResetThreshold))
        {
            canSwitchTarget = true;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Aim_canceled(object sender, System.EventArgs e)
    {
        normalViewVirtualCamera.Priority = 2;
        PlayerUI.Instance.DisableAimUI();
        isAiming = false;
    }

    private void Aim_started(object sender, System.EventArgs e)
    {
        normalViewVirtualCamera.Priority = 0;
        PlayerUI.Instance.EnableAimUI();
        isAiming = true;
    }

    public bool IsAiming()
    {
        return isAiming;
    }

    private void CameraToForwardIfNotClose()
    {
        if (Mathf.Abs(cinemachineTargetPitch - playerTransform.eulerAngles.x) > 45)
        {
            cinemachineTargetPitch = playerTransform.eulerAngles.x;
        }
        if (Mathf.Abs(cinemachineTargetYaw - playerTransform.eulerAngles.y) > 30)
        {
            cinemachineTargetYaw = playerTransform.eulerAngles.y;
        }
    }

    private void CameraToForward()
    {
        cinemachineTargetPitch = playerTransform.eulerAngles.x;
        cinemachineTargetYaw = playerTransform.eulerAngles.y;
    }


    private void TransformToCamera()
    {
        StopCoroutine(TransformToCameraInSpeed());
        StartCoroutine(TransformToCameraInSpeed());
    }

    private IEnumerator LockingOn()
    {
        lockingOn = true; // Ensure it's marked as locking on

        normalViewVirtualCamera.LookAt = currentTarget.transform;

        float startYRotation = playerTransform.eulerAngles.y;
        float elapsedTime = 0f;
        float duration = 1f / playerCorrectionRotationSpeed;

        Vector3 playerDirection = (currentTarget.transform.position - playerTransform.position).normalized;
        float endPlayerYRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg;

        duration *= 2;

        // Store initial camera rotation
        Quaternion startCameraRotation = cinemachineCameraTarget.transform.rotation;

        Vector3 cameraDirection = (currentTarget.transform.position - MainCamera.Instance.transform.position).normalized;
        float endCameraYRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg;

        Quaternion targetCameraRotation = Quaternion.Euler(20, endCameraYRotation, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Smoothly interpolate player's rotation
            float currentYRotation = Mathf.LerpAngle(startYRotation, endPlayerYRotation, t);
            playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, currentYRotation, playerTransform.eulerAngles.z);

            // Smoothly interpolate camera rotation to match player direction
            cinemachineCameraTarget.transform.rotation = Quaternion.Slerp(startCameraRotation, targetCameraRotation, t);

            yield return null;
        }

        // Ensure final values are set
        playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, endPlayerYRotation, playerTransform.eulerAngles.z);
        cinemachineCameraTarget.rotation = targetCameraRotation;


        lockingOn = false; // Unlock after completing
    }

    private IEnumerator TransformToCameraInSpeed()
    {
        float startYRotation = playerTransform.eulerAngles.y;
        float endYRotation = cinemachineTargetYaw;
        float elapsedTime = 0f;
        float duration = 1f / playerCorrectionRotationSpeed;

        if (lockedOn) {

            Vector3 direction = (currentTarget.transform.position - playerTransform.position).normalized;
            endYRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            duration = duration * 4;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentYRotation = Mathf.LerpAngle(startYRotation, endYRotation, elapsedTime / duration);
            playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, currentYRotation, playerTransform.eulerAngles.z);
            yield return null;
        }

        // Ensure final rotation matches exactly
        playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, endYRotation, playerTransform.eulerAngles.z);
    }
}
