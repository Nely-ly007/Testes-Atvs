using UnityEngine;

namespace ShooterBoss.Powerups
{
    /// <summary>
    /// Spawna o prefab de HealthPowerup em intervalos aleatórios, numa posição X
    /// aleatória dentro da largura da fase, vindo de cima. Coloque este componente
    /// num GameObject vazio qualquer na cena (ex: "PowerupSpawner").
    /// </summary>
    public class HealthPowerupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject healthPowerupPrefab;
        [SerializeField] private float minInterval = 12f;
        [SerializeField] private float maxInterval = 20f;
        [Tooltip("Faixa de X onde o powerup pode nascer (bata com os mesmos movementBounds do player).")]
        [SerializeField] private float spawnXMin = -3.5f;
        [SerializeField] private float spawnXMax = 3.5f;
        [SerializeField] private float spawnY = 6f; // um pouco acima do topo visível da câmera

        private float _timer;
        private float _nextSpawnTime;

        private void Start() => RollNextSpawnTime();

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _nextSpawnTime)
            {
                _timer = 0f;
                RollNextSpawnTime();
                Spawn();
            }
        }

        private void Spawn()
        {
            float x = Random.Range(spawnXMin, spawnXMax);
            Vector3 pos = new Vector3(x, spawnY, 0f);
            Instantiate(healthPowerupPrefab, pos, Quaternion.identity);
        }

        private void RollNextSpawnTime() => _nextSpawnTime = Random.Range(minInterval, maxInterval);
    }
}