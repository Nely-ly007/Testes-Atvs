using UnityEngine;
using ShooterBoss.Player;

namespace ShooterBoss.Powerups
{
    /// <summary>
    /// Powerup de vida: fica parado (ou flutuando) na fase, e quando o player
    /// encosta, cura 1 vida e some. Não usa Object Pool porque, diferente das
    /// balas, aparece raramente (poucas unidades por partida) — Instantiate/Destroy
    /// aqui é o custo certo, não o gargalo que a Pool resolve.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class HealthPowerup : MonoBehaviour
    {
        [SerializeField] private int healAmount = 1;
        [Tooltip("Velocidade de queda/descida do powerup até sair da tela, se não for pego.")]
        [SerializeField] private float fallSpeed = 1.5f;
        [SerializeField] private float lifeTime = 8f; // some sozinho se o player não pegar

        private float _timer;

        private void Reset()
        {
            // garante que o Collider2D já vem configurado certo ao adicionar o componente
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void Update()
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            _timer += Time.deltaTime;
            if (_timer >= lifeTime) Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerHealth playerHealth)) return;

            // opcional: se já estiver com vida cheia, deixa o powerup continuar
            // caindo em vez de sumir à toa — comente a linha abaixo se preferir
            // que ele sempre seja coletado mesmo com vida cheia.
            if (playerHealth.IsFullLife) return;

            playerHealth.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}