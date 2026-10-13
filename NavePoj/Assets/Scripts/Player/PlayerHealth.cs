using System;
using UnityEngine;

namespace ShooterBoss.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxLives = 3;
        [SerializeField] private float invulnerabilityAfterHit = 2f; // igualado ao invulnerabilityDuration do BossHealth — 1.5s ainda era curto contra as rajadas de Vida Baixa

        public int CurrentLives { get; private set; }
        public event Action<int, int> OnLivesChanged; // (current, max)
        public event Action OnPlayerDied;

        private bool _invulnerable;

        private void Awake()
        {
            CurrentLives = maxLives;
        }

        private void Start() => OnLivesChanged?.Invoke(CurrentLives, maxLives);

        public void TakeDamage(int amount)
        {
            if (_invulnerable || CurrentLives <= 0) return;

            CurrentLives -= amount;
            CurrentLives = Mathf.Max(0, CurrentLives);
            OnLivesChanged?.Invoke(CurrentLives, maxLives);

            if (CurrentLives <= 0)
            {
                OnPlayerDied?.Invoke();
            }
            else
            {
                StartCoroutine(InvulnerabilityWindow());
            }
        }

        /// <summary>Usado pelo powerup de vida. Não passa de maxLives e não faz nada se o player já estiver morto.</summary>
        public void Heal(int amount)
        {
            if (CurrentLives <= 0) return;

            int before = CurrentLives;
            CurrentLives = Mathf.Min(maxLives, CurrentLives + amount);

            if (CurrentLives != before)
                OnLivesChanged?.Invoke(CurrentLives, maxLives);
        }

        /// <summary>True se já está com vida cheia — o powerup usa isso pra decidir se vale a pena spawnar.</summary>
        public bool IsFullLife => CurrentLives >= maxLives;

        private System.Collections.IEnumerator InvulnerabilityWindow()
        {
            _invulnerable = true;
            yield return new WaitForSeconds(invulnerabilityAfterHit);
            _invulnerable = false;
        }
    }
}