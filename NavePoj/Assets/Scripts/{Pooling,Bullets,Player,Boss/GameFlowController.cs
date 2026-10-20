using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ShooterBoss.Player;
using ShooterBoss.Boss;

namespace ShooterBoss.Core
{
    /// <summary>
    /// Substitui o antigo GameManager.cs. Fica na cena Gameplay, escuta o fim de
    /// jogo (vitória ou derrota) e troca pra cena Resultado. LoadScene em modo
    /// Single descarrega Gameplay e GUI sozinho, não precisa descarregar na mão.
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private BossHealth bossHealth;
        [Tooltip("Tempo (em segundos reais, não afetado por timeScale) que o jogo fica congelado antes de trocar pra cena Resultado.")]
        [SerializeField] private float delayBeforeResultScene = 1.5f;

        private bool _gameOver;

        private void OnEnable()
        {
            playerHealth.OnPlayerDied += HandlePlayerDied;
            bossHealth.OnBossDefeated += HandleBossDefeated;
        }

        private void OnDisable()
        {
            playerHealth.OnPlayerDied -= HandlePlayerDied;
            bossHealth.OnBossDefeated -= HandleBossDefeated;
        }

        private void HandlePlayerDied() => EndGame(false);
        private void HandleBossDefeated() => EndGame(true);

        private void EndGame(bool playerWon)
        {
            if (_gameOver) return;
            _gameOver = true;

            Time.timeScale = 0f; // congela o jogo no exato momento da vitória/derrota
            MatchResult.Set(playerWon);
            StartCoroutine(GoToResultSceneAfterDelay());
        }

        private IEnumerator GoToResultSceneAfterDelay()
        {
            // WaitForSecondsRealtime, não WaitForSeconds: com timeScale em 0 a versão
            // normal (e o Invoke/InvokeRepeating) nunca dispararia.
            yield return new WaitForSecondsRealtime(delayBeforeResultScene);

            Time.timeScale = 1f; // reseta ANTES de trocar de cena, senão a Resultado nasce pausada
            SceneManager.LoadScene(SceneNames.Resultado, LoadSceneMode.Single);
        }
    }
}
