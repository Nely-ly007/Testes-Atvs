using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Alta - comportamento B: move devagar de um lado a outro, tiro reto.
    public class Boss_HighB : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 0.8f;
        [SerializeField] private float bulletSpeed = 4.5f;
        [SerializeField] private float moveSpeedMultiplier = 0.6f;
        private float _next;
        private float _dir = 1f;

        protected override void OnEnter(Animator animator) { _next = 0f; _dir = 1f; }

        protected override void OnTick(Animator animator, float timer)
        {
            Shooter.MoveHorizontal(_dir, moveSpeedMultiplier);
            if (Shooter.transform.position.x >= Shooter.MovementBounds.xMax) _dir = -1f;
            if (Shooter.transform.position.x <= Shooter.MovementBounds.xMin) _dir = 1f;

            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireStraight(0, bulletSpeed);
        }
    }
}
