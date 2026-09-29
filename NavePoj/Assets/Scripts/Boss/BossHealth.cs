using System;
using UnityEngine;

namespace ShooterBoss.Boss
{
    public enum BossLifeState { High, Medium, Low }

    /// <summary>
    /// Controla HP do chefão e dispara os parâmetros do Animator Controller.
    /// As StateMachineBehaviours leem "lifeState" e "invulnerable" para decidir
    /// o comportamento — esta classe NÃO implementa uma state machine própria.
    /// </summary>
    public class BossHealth : MonoBehaviour
    {
        [SerializeField] private int maxHp = 500; // ajuste para dar ~1min de luta
        [SerializeField] private float invulnerabilityDuration = 2f;
        [SerializeField] private Animator animator;

        private static readonly int LifeStateParam = Animator.StringToHash("lifeState"); // int: 0=High,1=Medium,2=Low
        private static readonly int InvulnerableParam = Animator.StringToHash("invulnerable"); // bool

        public int CurrentHp { get; private set; }
        public int MaxHp => maxHp;
        public BossLifeState CurrentLifeState { get; private set; } = BossLifeState.High;
        public bool IsInvulnerable { get; private set; }

        public event Action<int, int> OnHpChanged; // (current, max)
        public event Action<BossLifeState> OnLifeStateChanged;
        public event Action OnBossDefeated;

        private void Awake() => CurrentHp = maxHp;

        private void Start()
        {
            OnHpChanged?.Invoke(CurrentHp, maxHp);
            animator.SetInteger(LifeStateParam, (int)CurrentLifeState);
        }

        public void TakeDamage(int amount)
        {
            if (IsInvulnerable || CurrentHp <= 0) return;

            CurrentHp = Mathf.Max(0, CurrentHp - amount);
            OnHpChanged?.Invoke(CurrentHp, maxHp);

            if (CurrentHp <= 0)
            {
                OnBossDefeated?.Invoke();
                return;
            }

            BossLifeState newState = ComputeLifeState();
            if (newState != CurrentLifeState)
            {
                CurrentLifeState = newState;
                OnLifeStateChanged?.Invoke(newState);
                animator.SetInteger(LifeStateParam, (int)newState);
                StartCoroutine(InvulnerabilityWindow());
            }
        }

        private BossLifeState ComputeLifeState()
        {
            float pct = (float)CurrentHp / maxHp;
            if (pct <= 0.2f) return BossLifeState.Low;
            if (pct <= 0.5f) return BossLifeState.Medium;
            return BossLifeState.High;
        }

        private System.Collections.IEnumerator InvulnerabilityWindow()
        {
            IsInvulnerable = true;
            animator.SetBool(InvulnerableParam, true);
            yield return new WaitForSeconds(invulnerabilityDuration);
            IsInvulnerable = false;
            animator.SetBool(InvulnerableParam, false);
        }
    }
}
