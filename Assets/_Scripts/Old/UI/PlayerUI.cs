using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TrialsOfOdin.State;
using TrialsOfOdin.Stats;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : Singleton<PlayerUI>
{
    [Header("HealthUIs")]
    [SerializeField] private Slider healthBarUI;
    [SerializeField] private Slider healthBarEasingUI;
    [SerializeField] private float healthBarEasing = 1;
    [Space(10)]
    [Header("StaminaUIs")]
    [SerializeField] private Slider staminaBarUI;
    [SerializeField] private float staminaBarEasing = 1;
    private Coroutine staminaUIRoutine;
    private float staminaUIValue;

    [SerializeField] private GameObject AimUI;

    [SerializeField] private List<Toggle> comboCounter;
    [SerializeField] private Image odinUI;
    [SerializeField] private float odinFill;
    [SerializeField] private int odinFillAmount;
    [SerializeField] private int comboCount;
    private bool isRaging;

    private void Start()
    {
        healthBarEasingUI.value = 1;
        healthBarUI.value = 1;
        staminaBarUI.value = 1;
        ResetComboCountUI();
        PlayerStateMachine.OnAnyPlayerSpawn += PlayerStateMachine_OnAnyPlayerSpawn;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float newFillAmount = maxHealth > 0 ? currentHealth / maxHealth : 0f;

        healthBarUI.value = newFillAmount;
        StopCoroutine(nameof(HealthDisplayEasing));
        StartCoroutine(HealthDisplayEasing(newFillAmount));
    }

    private IEnumerator HealthDisplayEasing(float targetFillAmount)
    {
        while (Mathf.Abs(healthBarEasingUI.value - targetFillAmount) > 0.01f)
        {
            healthBarEasingUI.value = Mathf.Lerp(healthBarEasingUI.value, targetFillAmount, Time.deltaTime * healthBarEasing);
            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set precisely
        healthBarEasingUI.value = targetFillAmount;
    }

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBarUI.value = currentStamina / maxStamina;
        staminaUIValue = currentStamina;
    }

    public void LerpStaminaBar(float currentStamina, float maxStamina)
    {
        if (staminaUIRoutine != null) StopCoroutine(staminaUIRoutine);
        staminaUIRoutine = StartCoroutine(LerpStaminaUI(currentStamina, maxStamina));
    }

    private IEnumerator LerpStaminaUI(float currentStamina, float maxStamina)
    {
        float startValue = staminaUIValue;
        float targetValue = currentStamina;
        float elapsedTime = 0f;

        while (elapsedTime < staminaBarEasing)
        {
            staminaUIValue = Mathf.Lerp(startValue, targetValue, elapsedTime / staminaBarEasing);
            staminaBarUI.value = staminaUIValue / maxStamina;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        staminaUIValue = targetValue;
        staminaBarUI.value = staminaUIValue / maxStamina;
    }

    private void PlayerStateMachine_OnAnyPlayerSpawn(object sender, System.EventArgs e)
    {
        //odinFillAmount = PlayerStateMachine.LocalInstance.CombatManager.ComboCountForRage;
    }

    public void IncreaseComboCount()
    {
        if (isRaging) return;
        if(odinFill <= odinFillAmount) comboCount++;
        if (comboCount == comboCounter.Count + 1) comboCount = 1;

        ResetComboCountUI();

        for(int i = 0; i < comboCount; i++)
        {
            comboCounter[i].isOn = true;
        }

        if(comboCount == comboCounter.Count)
        {
            OdinFillUI();
        }
    }

    public void ResetComboCountUI()
    {
        foreach(Toggle comboToggle in comboCounter)
        {
            comboToggle.isOn = false;
        }
    }

    public void ResetAll()
    {
        ResetComboCountUI();
        comboCount = 0;
        odinFill = 0;
    }

    public void OdinFillUI()
    {
        if (odinFill < odinFillAmount) 
        {
            odinFill++;
            float targetFill = odinFill / odinFillAmount;
            StartCoroutine(LerpOdinFill(targetFill, 0.3f));
        }
    }

    private IEnumerator LerpOdinFill(float targetFill, float duration)
    {
        float elapsed = 0f;
        float startFill = odinUI.fillAmount;

        while (elapsed < duration)
        {
            odinUI.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        odinUI.fillAmount = targetFill; 
    }

    public bool CanRage()
    {
        if (odinFill == odinFillAmount) return true;
        return false;
    }

    public void RageUI(float duration)
    {
        StartCoroutine(RageTimerUI(duration));
        isRaging = true;
    }

    private IEnumerator RageTimerUI(float duration)
    {
        float elapsed = 0f;
        float startFill = odinUI.fillAmount;

        while (elapsed < duration)
        {
            odinUI.fillAmount = Mathf.Lerp(startFill, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        odinUI.fillAmount = 0; 
        isRaging = false;
    }


    public void EnableAimUI()
    {
        AimUI.SetActive(true);
    }

    public void DisableAimUI()
    {
        AimUI.SetActive(false);
    }
}
