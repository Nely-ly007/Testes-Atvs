using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    // Vida Baixa - comportamento A: tiro mirado bem rápido, movendo.
    public class Boss_LowA : BossStateBehaviourBase
    {
        [SerializeField] private float fireInterval = 0.3f;
        [SerializeField] private float bulletSpeed = 7.5f;
        private float _next;

        protected override void OnEnter(Animator animator) => _next = 0f;

        protected override void OnTick(Animator animator, float timer)
        {
            Shooter.MoveTowardsPlayerX(1.5f);

            if (timer < _next) return;
            _next = timer + fireInterval;
            Shooter.FireAtPlayer(0, bulletSpeed);
        }
    }
}
