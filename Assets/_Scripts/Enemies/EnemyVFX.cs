using INab.Dissolve;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    public enum EImpactVFXs
    {
        LightImpact,
        HeavyImpact,
    }

    [SerializeField] private MMFeedbacks lightImpactFeedback;
    [SerializeField] private MMFeedbacks heavyImpactFeedback;
    [SerializeField] private Dissolver dissolver;
    [SerializeField] private List<Material> dissolveMaterials;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private Dictionary<EImpactVFXs, MMFeedbacks> impactVFXs;

    private void Start()
    {
        PopulateImpactDictionary();
    }

    public void PlayImpactFeedBack(EImpactVFXs impactType)
    {
        impactVFXs[impactType].PlayFeedbacks();
    }

    private void PopulateImpactDictionary()
    {
        impactVFXs = new Dictionary<EImpactVFXs, MMFeedbacks>()
        {
            { EImpactVFXs.LightImpact, lightImpactFeedback },
            { EImpactVFXs.HeavyImpact, heavyImpactFeedback},
        };
    }

    public void Dissolve()
    {
        skinnedMeshRenderer.materials = dissolveMaterials.ToArray();
        StartCoroutine(DelayedDissolve());
    }

    IEnumerator DelayedDissolve()
    {
        yield return new WaitForSeconds(0.1f);
        DissolveMaterials();
    }

    public void DissolveMaterials()
    {
        dissolver.FindMaterials();
        dissolver.Dissolve();
    }
}
