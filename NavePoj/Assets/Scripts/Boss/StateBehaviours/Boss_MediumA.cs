using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Média - comportamento A: move rápido, tiro mirado no player.
    public class Boss_MediumA : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 0.7f;
        [SerializeField] private float bulletSpeed = 6f;
        private float _next;
        private float _dir = 1f;

        protected override void OnEnter(Animator animator) { _next = 0f; _dir = 1f; }

        protected override void OnTick(Animator animator, float timer)
        {
            Shooter.MoveHorizontal(_dir, 1f);
            if (Shooter.transform.position.x >= Shooter.MovementBounds.xMax) _dir = -1f;
            if (Shooter.transform.position.x <= Shooter.MovementBounds.xMin) _dir = 1f;

            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireAtPlayer(0, bulletSpeed);
        }
    }
}
