using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [SerializeField] private GameObject targetUI;
    public GameObject targetParent;

    private void Start()
    {
        targetParent.GetComponentInChildren<Health>().OnDeath += EnemyTarget_OnDeath;
    }

    private void EnemyTarget_OnDeath(object sender, System.EventArgs e)
    {
        this.gameObject.SetActive(false);
    }

    public void ActivateTargetUI()
    {
        targetUI.SetActive(true);
    }

    public void DisableTargetUI()
    {
        targetUI.SetActive(false);
    }
}
