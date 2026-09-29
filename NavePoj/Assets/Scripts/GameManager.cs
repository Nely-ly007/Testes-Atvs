using UnityEngine;
using ShooterBoss.Player;
using ShooterBoss.Boss;

namespace ShooterBoss
{
    /// <summary>
    /// Ponto único opcional para orquestrar o fim de jogo caso você não queira
    /// deixar essa responsabilidade só na UIManager (ex: parar spawns, tocar SFX).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private BossHealth bossHealth;

        public bool GameOver { get; private set; }

        private void OnEnable()
        {
            playerHealth.OnPlayerDied += () => EndGame(false);
            bossHealth.OnBossDefeated += () => EndGame(true);
        }

        private void EndGame(bool victory)
        {
            if (GameOver) return;
            GameOver = true;
            Debug.Log(victory ? "Jogador venceu o chefão." : "Jogador foi derrotado.");
            // UIManager já cuida da tela; aqui é onde você pausaria spawners extras, etc.
        }
    }
}
