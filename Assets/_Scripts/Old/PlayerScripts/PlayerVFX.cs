using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVFX : MonoBehaviour
{
    public enum EImpactVFXs
    {
        LightImpact,
        HeavyImpact,
    }

    [SerializeField] private MMFeedbacks lightImpactFeedback;
    [SerializeField] private MMFeedbacks heavyImpactFeedback;
    [SerializeField] private UniversalRendererData universalRendererData;
    private ScriptableRendererFeature colorIsolationFeature;
    private ScriptableRendererFeature tiltShiftFeature;

    private Dictionary<EImpactVFXs, MMFeedbacks> impactVFXs;

    private void Awake()
    {
        PopulateImpactDictionary();
        FindColorIsolationFeature();
    }

    public void PlayImpactFeedBack(EImpactVFXs impactType)
    {
        impactVFXs[impactType].PlayFeedbacks();
    }

    private void FindColorIsolationFeature()
    {
        foreach (var feature in universalRendererData.rendererFeatures)
        {
            if (feature.name == "ColorIsolation") // Check by name or type
            {
                colorIsolationFeature = feature;
                continue;
            }
            else if(feature.name == "TiltShift")
            {
                tiltShiftFeature = feature;
                continue;
            }
        }

        Debug.LogWarning("Color Isolation feature not found in Universal Renderer Data.");
    }

    public void EnableRageVFX()
    {
        colorIsolationFeature.SetActive(true);
        tiltShiftFeature.SetActive(true);
    }

    public void DisableRageVFX()
    {
        colorIsolationFeature.SetActive(false);
        tiltShiftFeature.SetActive(false);
    }

    private void OnDestroy()
    {
        DisableRageVFX();
    }

    private void PopulateImpactDictionary()
    {
        impactVFXs = new Dictionary<EImpactVFXs, MMFeedbacks>() 
        {
            { EImpactVFXs.LightImpact, lightImpactFeedback },
            { EImpactVFXs.HeavyImpact, heavyImpactFeedback},
        };
    }
}
