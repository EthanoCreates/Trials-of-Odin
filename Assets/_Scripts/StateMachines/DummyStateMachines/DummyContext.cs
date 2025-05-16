using UnityEngine;

public class DummyContext : EnemyAISMContext
{
    public DummyContext(Transform transform, EnemyStats enemyStats, EnemyDataSO dummyData)
    {
        humanoidStats = enemyStats;
        Transform = transform;

        DummyAnimator animator = Transform.GetComponent<DummyAnimator>();

        animator.OnAnimationFinished += (object sender, System.EventArgs e) => AnimFinished = true;
        animator.OnAnimationStarted += (object sender, System.EventArgs e) => AnimFinished = false;
    }

    public float VerticalVelocity { get; set; }
}
