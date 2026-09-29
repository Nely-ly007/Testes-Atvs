using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Alta - comportamento A: parado, tiro reto lento.
    public class Boss_HighA : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 1f;
        [SerializeField] private float bulletSpeed = 4f;
        private float _next;

        protected override void OnEnter(Animator animator) => _next = 0f;

        protected override void OnTick(Animator animator, float timer)
        {
            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireStraight(0, bulletSpeed);
        }
    }
}
