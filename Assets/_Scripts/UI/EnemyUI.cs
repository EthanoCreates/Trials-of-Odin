using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject headUIObject;

    [FoldoutGroup("HealthBar")]
    [SerializeField] private Health health;
    [FoldoutGroup("HealthBar")]
    [SerializeField] private Slider healthBarUI;
    [FoldoutGroup("HealthBar")]
    [SerializeField] private Slider healthBarEasingUI;
    [FoldoutGroup("HealthBar")]
    [SerializeField] private float healthBarEasing = 1f;

    [FoldoutGroup("FinisherAvailible")]
    [SerializeField] private GameObject finisherUIObject;
    [FoldoutGroup("FinisherAvailible")]
    [SerializeField] private Image finisherRadialTimer;


    public Slider HealthUI { get { return healthBarUI; } }
    public Slider HealthBarEasingUI { get { return healthBarEasingUI; } }

    private void Awake()
    {
        HealthBarEasingUI.value = 1;
        HealthUI.value = 1;

        health.OnTakeDamage += Health_OnTakeDamage;
    }

    private Coroutine healthEasingCoroutine;

    private void Health_OnTakeDamage(object sender, System.EventArgs e)
    {
        float fillAmount = health.GetCurrentHealth() / health.GetMaxHealth();
        if (fillAmount < 0) fillAmount = 0;
        healthBarUI.value = fillAmount;

        // Stop previous coroutine if it exists
        if (healthEasingCoroutine != null)
            StopCoroutine(healthEasingCoroutine);

        // Start a new easing coroutine
        healthEasingCoroutine = StartCoroutine(HealthDisplayEasing(fillAmount));
    }


    private IEnumerator HealthDisplayEasing(float targetFillAmount)
    {
        while (healthBarEasingUI.value - targetFillAmount > 0.01f)
        {
            healthBarEasingUI.value = Mathf.MoveTowards(healthBarEasingUI.value, targetFillAmount, Time.deltaTime * healthBarEasing);
            yield return null;
        }

        healthBarEasingUI.value = targetFillAmount;

        if (targetFillAmount <= 0) headUIObject.SetActive(false);
    }


    public void StartFinisherTimer(float finisherTimer)
    {
        finisherUIObject.SetActive(true);
        StartCoroutine(DisplayFinisherTimer(finisherTimer));
    }

    private IEnumerator DisplayFinisherTimer(float finisherTimer)
    {
        finisherRadialTimer.fillAmount = 1;
        float elapsedTime = 0;

        while (elapsedTime < finisherTimer)
        {
            elapsedTime += Time.deltaTime;
            finisherRadialTimer.fillAmount = Mathf.Lerp(1, 0, elapsedTime / finisherTimer);
            yield return null;
        }

        finisherRadialTimer.fillAmount = 0;
        finisherUIObject.SetActive(false);
    }

}
