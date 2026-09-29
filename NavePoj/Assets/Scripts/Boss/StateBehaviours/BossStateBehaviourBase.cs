using UnityEngine;

namespace ShooterBoss.Boss.StateBehaviours
{
    /// <summary>
    /// Base comum para os 9 StateMachineBehaviour do chefão.
    /// Cada estado do Animator Controller usa uma subclasse diferente
    /// (Boss_HighA, Boss_HighB, Boss_HighC, Boss_MediumA... Boss_LowC).
    /// </summary>
    public abstract class BossStateBehaviourBase : StateMachineBehaviour
    {
        protected BossShooter Shooter;
        protected BossHealth Health;
        protected float Timer;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Shooter = animator.GetComponent<BossShooter>();
            Health = animator.GetComponent<BossHealth>();
            Timer = 0f;
            OnEnter(animator);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Enquanto invulnerável (troca de fase), o chefão fica parado e sem atirar.
            if (Health != null && Health.IsInvulnerable) return;

            Timer += Time.deltaTime;
            OnTick(animator, Timer);
        }

        protected abstract void OnEnter(Animator animator);
        protected abstract void OnTick(Animator animator, float timer);
    }
}
