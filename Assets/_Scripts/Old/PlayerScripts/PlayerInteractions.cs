using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private LayerMask interactLayerMask;

    private Transform mainCamera;

    private void Start()
    {
        GameInput.Instance.OnInteract += Instance_OnInteract;
        mainCamera = MainCamera.Instance.transform;
    }

    private void Instance_OnInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 10f, interactLayerMask))
        {
            // Try to get the component implementing IInteractable
            IInteractables interactable = hit.collider.gameObject.GetComponent<IInteractables>();

            // Check if the object has an interactable component
            if (interactable != null)
            {
                // Call the Interact method on the interactable component
                interactable.Interact();
            }
        }
    }
}
