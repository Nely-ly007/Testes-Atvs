using System;
using UnityEngine;

namespace ShooterBoss.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxLives = 3;
        [SerializeField] private float invulnerabilityAfterHit = 1f;

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

        private System.Collections.IEnumerator InvulnerabilityWindow()
        {
            _invulnerable = true;
            yield return new WaitForSeconds(invulnerabilityAfterHit);
            _invulnerable = false;
        }
    }
}
