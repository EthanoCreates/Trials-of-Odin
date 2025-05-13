using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : Singleton<PlayerUIManager>
{
    [SerializeField] private float healthBarEasing = 1f;
    private Slider healthUI;
    private Slider healthEasingUI;
    private Slider staminaSlider;

    private void Start()
    {
        ReInitialize();
    }

    public void ReInitialize()
    {
        PlayerUI playerUI = PlayerUI.Instance;

        healthUI = playerUI.HealthUI;
        healthEasingUI = playerUI.HealthBarEasingUI;
        staminaSlider = playerUI.StaminaBarUI;
    }

    public void DisplayHealth(float newFillAmount)
    {
        healthUI.value = newFillAmount;
        StopCoroutine(nameof(HealthDisplayEasing));
        StartCoroutine(HealthDisplayEasing(newFillAmount));
    }

    private IEnumerator HealthDisplayEasing(float targetFillAmount)
    {
        while (Mathf.Abs(healthEasingUI.value - targetFillAmount) > 0.01f)
        {
            healthEasingUI.value = Mathf.Lerp(healthEasingUI.value, targetFillAmount, Time.deltaTime * healthBarEasing);
            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set precisely
        healthEasingUI.value = targetFillAmount;
    }
}
