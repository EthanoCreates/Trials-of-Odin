using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : Singleton<PlayerUI>
{
    [SerializeField] private Slider staminaBarUI;
    [Space(10)]
    [Header("HealthUIs")]
    [SerializeField] private Slider healthBarUI;
    [SerializeField] private Slider healthBarEasingUI;

    [SerializeField] private GameObject AimUI;

    [SerializeField] private List<Toggle> comboCounter;
    [SerializeField] private Image odinUI;
    [SerializeField] private float odinFill;
    [SerializeField] private int odinFillAmount;
    [SerializeField] private int comboCount;
    private bool isRaging;

    public Slider StaminaBarUI { get { return staminaBarUI; } }
    public Slider HealthUI { get { return healthBarUI; } }
    public Slider HealthBarEasingUI { get { return healthBarEasingUI; } }

    private void Start()
    {
        HealthBarEasingUI.value = 1;
        healthBarUI.value = 1;
        staminaBarUI.value = 1;
        ResetComboCountUI();
        PlayerStateMachine.OnAnyPlayerSpawn += PlayerStateMachine_OnAnyPlayerSpawn;
    }

    private void PlayerStateMachine_OnAnyPlayerSpawn(object sender, System.EventArgs e)
    {
        odinFillAmount = PlayerStateMachine.LocalInstance.CombatManager.ComboCountForRage;
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
