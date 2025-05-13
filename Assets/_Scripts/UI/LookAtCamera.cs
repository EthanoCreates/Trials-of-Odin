using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCamera;

    private void Awake()
    {
        if (PlayerStateMachine.LocalInstance == null)
        {
            PlayerStateMachine.OnAnyPlayerSpawn += PlayerStateMachine_OnAnyPlayerSpawn;
        }
        else
        {
            GetPlayerCamera();
        }
    }

    private void PlayerStateMachine_OnAnyPlayerSpawn(object sender, System.EventArgs e)
    {
        if (PlayerStateMachine.LocalInstance != null)
        {
           GetPlayerCamera();
        }
    }

    private void GetPlayerCamera()
    {
        mainCamera = MainCamera.Instance.transform;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;
        transform.LookAt(transform.position + mainCamera.forward);
    }
    private void OnDestroy()
    {
        PlayerStateMachine.OnAnyPlayerSpawn -= PlayerStateMachine_OnAnyPlayerSpawn;
    }
}


