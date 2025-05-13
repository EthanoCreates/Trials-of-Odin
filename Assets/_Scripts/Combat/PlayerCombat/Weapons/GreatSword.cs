using Sirenix.OdinInspector;
using UnityEngine.VFX;
using UnityEngine;

public class GreatSword : Weapon
{
    [FoldoutGroup("VFX")]

    [FoldoutGroup("VFX/Slash")]
    [HorizontalGroup("VFX/Slash/Row1", width: .05f)]
    [HideLabel]
    [SerializeField] private bool slash;

    [HorizontalGroup("VFX/Slash/Row1", width: .7f)]
    [SerializeField] private VisualEffect SlashVFX;


    [FoldoutGroup("VFX/ComplexTrail")]

    [HorizontalGroup("VFX/ComplexTrail/Row2", width: .05f)]
    [HideLabel]
    [SerializeField] private bool trail;

    [HorizontalGroup("VFX/ComplexTrail/Row2", width: .7f)]
    [SerializeField] private ParticleSystem trailVFX;


    [HorizontalGroup("VFX/ComplexTrail/Row3", width: .05f)]
    [HideLabel]
    [SerializeField] private bool hasParticles;

    [FoldoutGroup("VFX")]
    [HorizontalGroup("VFX/ComplexTrail/Row3", width: .7f)]
    [SerializeField] private ParticleSystem particles;

    [HorizontalGroup("VFX/ComplexTrail/Row4", width: .5f)]
    [HideLabel]
    [SerializeField] private ParticleSystem glow;

    [HorizontalGroup("VFX/ComplexTrail/Row4")]
    [Button] private void EnableGlow() => glow.Play();

    [HorizontalGroup("VFX/ComplexTrail/Row4")]
    [Button] private void DisableGlow() => glow.Stop();

    [FoldoutGroup("VFX/SimpleTrail")]

    [HorizontalGroup("VFX/SimpleTrail/Row5", width: .3f)]
    [HideLabel]
    [SerializeField] private bool cTrial;


    [HorizontalGroup("VFX/SimpleTrail/Row6")]
    [LabelText("Constant")]
    [SerializeField] private bool constant;


    [HorizontalGroup("VFX/SimpleTrail/Row6")]
    [Range(0, 20)]
    [HideLabel]
    [SerializeField] private float duration;


    [HorizontalGroup("VFX/SimpleTrail/Row6")]
    [Button]
    private void SetDuration()
    {

    }

    public override void EnableWeaponVFX()
    {
        if (SlashVFX != null && slash)
        {
            SlashVFX.Play();
        }
        if (trailVFX != null && trail)
        {
            trailVFX.Play();
        }

        if (particles != null && hasParticles)
        {
            particles.Play();
        }
    }

    public override void DisableWeaponVFX()
    {
        if (constant == true || !cTrial) return;
    }
}

