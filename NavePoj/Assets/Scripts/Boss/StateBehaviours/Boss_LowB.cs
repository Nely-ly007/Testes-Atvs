using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Baixa - comportamento B: leque de 7 tiros rápido + movimento lateral rápido.
    public class Boss_LowB : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 0.6f;
        [SerializeField] private float bulletSpeed = 6.5f;
        private float _next;
        private float _dir = 1f;

        protected override void OnEnter(Animator animator) { _next = 0f; _dir = 1f; }

        protected override void OnTick(Animator animator, float timer)
        {
            Shooter.MoveHorizontal(_dir, 1.5f);
            if (Shooter.transform.position.x >= Shooter.MovementBounds.xMax) _dir = -1f;
            if (Shooter.transform.position.x <= Shooter.MovementBounds.xMin) _dir = 1f;

            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireSpread(0, bulletSpeed, count: 7, spreadAngle: 90f);
        }
    }
}
