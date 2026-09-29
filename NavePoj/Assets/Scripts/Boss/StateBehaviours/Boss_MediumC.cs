using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Média - comportamento C: persegue o eixo X do player, tiro reto rápido.
    public class Boss_MediumC : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 0.5f;
        [SerializeField] private float bulletSpeed = 6.5f;
        private float _next;

        protected override void OnEnter(Animator animator) => _next = 0f;

        protected override void OnTick(Animator animator, float timer)
        {
            Shooter.MoveTowardsPlayerX(1.2f);

            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireStraight(0, bulletSpeed);
        }
    }
}
