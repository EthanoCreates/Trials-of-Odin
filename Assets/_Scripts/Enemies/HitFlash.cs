using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Health health;

    [SerializeField] private float flashIntensity;
    [SerializeField] private float flashDuration;

    private Material material;
    private Color originalColor;

    private void Start()
    {
        Debug.Log("Hello");
        material = skinnedMeshRenderer.materials[0];
        originalColor = material.color;
        health.OnTakeDamage += Health_OnTakeDamage;
    }

    private void Health_OnTakeDamage(object sender, System.EventArgs e)
    {
        Debug.Log("Hello234");
        StopCoroutine(StartHitFlash());
        StartCoroutine(StartHitFlash());
    }

    private IEnumerator StartHitFlash()
    {
        Debug.Log("HitFlash");
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            float lerp = Mathf.Clamp01(elapsedTime / flashDuration);
            float intensity = Mathf.Lerp(flashIntensity, 1f, lerp);
            material.color = originalColor * intensity;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        material.color = originalColor; // Reset to original color
    }
}
