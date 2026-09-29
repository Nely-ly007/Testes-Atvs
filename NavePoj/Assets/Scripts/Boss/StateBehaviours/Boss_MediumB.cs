using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Média - comportamento B: parado, leque de 5 tiros mais rápido.
    public class Boss_MediumB : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 0.9f;
        [SerializeField] private float bulletSpeed = 5.5f;
        private float _next;

        protected override void OnEnter(Animator animator) => _next = 0f;

        protected override void OnTick(Animator animator, float timer)
        {
            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireSpread(0, bulletSpeed, count: 5, spreadAngle: 60f);
        }
    }
}
