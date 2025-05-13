using DG.Tweening;
using UnityEngine;

public class ElevatorHelper : MonoBehaviour
{
    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(triggered) return;

        triggered = true;
        transform.DOMoveY(transform.position.y - 36f, 10f);
    }
}
