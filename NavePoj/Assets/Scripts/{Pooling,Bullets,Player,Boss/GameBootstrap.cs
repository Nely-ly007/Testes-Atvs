using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShooterBoss.Core
{
    /// <summary>
    /// Único script da cena _Boot. Carrega Gameplay e GUI em modo Additive
    /// (as duas juntas formam "o jogo rodando na tela") e descarrega a _Boot
    /// em seguida, já que ela não tem mais função depois disso.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        private void Start() => StartCoroutine(LoadGame());

        private IEnumerator LoadGame()
        {
            yield return SceneManager.LoadSceneAsync(SceneNames.Gameplay, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(SceneNames.GUI, LoadSceneMode.Additive);

            // Define a Gameplay como cena "ativa" (onde objetos instanciados sem cena
            // explícita vão parar, ex: as balas da Pool, os powerups) antes de descartar a _Boot.
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneNames.Gameplay));

            yield return SceneManager.UnloadSceneAsync(SceneNames.Boot);
        }
    }
}
