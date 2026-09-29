using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Baixa - comportamento C: alterna tiro mirado e leque em cadência alta (mais difícil).
    public class Boss_LowC : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 0.25f;
        [SerializeField] private float bulletSpeed = 8f;
        private float _next;
        private bool _toggle;

        protected override void OnEnter(Animator animator) { _next = 0f; _toggle = false; }

        protected override void OnTick(Animator animator, float timer)
        {
            Shooter.MoveTowardsPlayerX(1.8f);

            if (timer < _next) return;
            _next = timer + fireInterval;

            if (_toggle) Shooter.FireAtPlayer(0, bulletSpeed);
            else Shooter.FireSpread(0, bulletSpeed, count: 3, spreadAngle: 30f);
            _toggle = !_toggle;
        }
    }
}
