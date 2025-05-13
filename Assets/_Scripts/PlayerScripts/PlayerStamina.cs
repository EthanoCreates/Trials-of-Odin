using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina;
    [SerializeField] private float sprintStaminaCost = 10f;
    [SerializeField] private float staminaRecovery = 10f;
    [SerializeField] private float staminaDepletionEasing= .1f;
    [SerializeField] private bool isRecovering = true;
    private float targetStaminaDepletion;
    private Coroutine staminaUIRoutine;
    private float staminaUIValue; 
    [ShowInInspector]
    [ReadOnly] public float CurrentStamina { get; private set; }

    void Start()
    {
        CurrentStamina = maxStamina;
        staminaUIValue = CurrentStamina;
        UpdateStaminaUI();
    }

    public bool CanSprint()
    {
        if (CurrentStamina <= 0) { CurrentStamina = 0; return false; }

        CurrentStamina -= sprintStaminaCost * Time.deltaTime;
        staminaUIValue = CurrentStamina;

        UpdateStaminaUI();
        return true;
    }

    public bool HasSufficientStamina(float staminaCost)
    {
        if((CurrentStamina - staminaCost) < 0) return false;

        CurrentStamina -= staminaCost;
        StartStaminaUICoroutine();

        isRecovering = false;

        CancelInvoke(nameof(StartRecovering));
        Invoke(nameof(StartRecovering), Mathf.Clamp(.2f * staminaCost, 1f, 2f));

        return true;
    }

    public bool CouldPerformActionWithStamina(float staminaCost)
    {
        if ((CurrentStamina - staminaCost) < 0) return false;
        return true;
    }

    private void StartRecovering() => isRecovering = true;

    public void RecoverStamina()
    {
        if (!isRecovering) return;
        if (CurrentStamina >= maxStamina) { CurrentStamina = maxStamina; return; }

        CurrentStamina += staminaRecovery * Time.deltaTime;
        staminaUIValue = CurrentStamina;

        UpdateStaminaUI();
    }

    private void StartStaminaUICoroutine()
    {
        if (staminaUIRoutine != null) StopCoroutine(staminaUIRoutine);
        staminaUIRoutine = StartCoroutine(LerpStaminaUI());
    }

    private IEnumerator LerpStaminaUI()
    {
        float startValue = staminaUIValue;
        float targetValue = CurrentStamina;
        float elapsedTime = 0f;

        while (elapsedTime < staminaDepletionEasing)
        {
            staminaUIValue = Mathf.Lerp(startValue, targetValue, elapsedTime / staminaDepletionEasing);
            PlayerUI.Instance.StaminaBarUI.value = staminaUIValue / maxStamina;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        staminaUIValue = targetValue;
        PlayerUI.Instance.StaminaBarUI.value = staminaUIValue / maxStamina;
    }

    private void UpdateStaminaUI() => PlayerUI.Instance.StaminaBarUI.value = staminaUIValue / maxStamina;
}
