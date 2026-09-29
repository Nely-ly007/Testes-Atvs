using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Alta - comportamento C: parado, leque de 3 tiros.
    public class Boss_HighC : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 1.2f;
        [SerializeField] private float bulletSpeed = 4f;
        private float _next;

        protected override void OnEnter(Animator animator) => _next = 0f;

        protected override void OnTick(Animator animator, float timer)
        {
            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireSpread(0, bulletSpeed, count: 3, spreadAngle: 40f);
        }
    }
}
