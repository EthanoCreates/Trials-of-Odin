using TrialsOfOdin.Stats;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    private PlayerUI playerUI;
    [SerializeField] private readonly Health health;
    [SerializeField] private readonly Stamina stamina;

    private void Start()
    {
        playerUI = PlayerUI.Instance;
        health.OnTakeDamage += Health_OnTakeDamage;
        stamina.OnStaminaChanged += Stamina_OnStaminaChanged;
    }

    private void Stamina_OnStaminaChanged(float currentStamina, float maxStamina, bool shouldLerpUI)
    {
        if (shouldLerpUI) playerUI.LerpStaminaBar(currentStamina, maxStamina);
        else playerUI.UpdateStaminaBar(currentStamina, maxStamina);
    }

    private void Health_OnTakeDamage(float currentHealth, float maxHealth)
        => playerUI.UpdateHealthBar(currentHealth, maxHealth);
    
}
