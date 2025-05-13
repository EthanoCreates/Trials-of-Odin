using UnityEngine;

public class DummyContext : EnemyAISMContext
{
    public DummyContext(Transform transform, EnemyDataSO dummyData)
    {
        Transform = transform;
        GroundLayers = dummyData.groundLayers;

        DummyAnimator animator = Transform.GetComponent<DummyAnimator>();

        animator.OnAnimationFinished += (object sender, System.EventArgs e) => AnimFinished = true;
        animator.OnAnimationStarted += (object sender, System.EventArgs e) => AnimFinished = false;
    }

    public float VerticalVelocity { get; set; }
}
