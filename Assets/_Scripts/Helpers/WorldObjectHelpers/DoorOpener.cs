using DG.Tweening;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;

    [SerializeField] private float rotateTarget = 85f;

    private void OnTriggerEnter(Collider other)
    {
        doorLeft.DORotate(new Vector3(0f, -rotateTarget, 0f), 3f, RotateMode.WorldAxisAdd);

        doorRight.DORotate(new Vector3(0f, rotateTarget, 0f), 3f, RotateMode.WorldAxisAdd).OnComplete(() =>
        {
            DoorOpener doorOpenerScript = doorRight.GetComponent<DoorOpener>();
            doorOpenerScript.enabled = false;
        });
    }
}
