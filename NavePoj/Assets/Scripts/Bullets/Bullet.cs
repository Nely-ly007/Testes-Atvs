using UnityEngine;
using UnityEngine.Pool;

namespace ShooterBoss.Bullets
{
    /// <summary>
    /// Bala reutilizável. Não é Instantiate/Destroy diretamente:
    /// sempre passa pelo ObjectPool<Bullet> guardado em Pool.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        public enum Owner { Player, Boss }

        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private int damage = 1;

        public Owner BulletOwner { get; private set; }
        public IObjectPool<Bullet> Pool { get; set; }

        private Rigidbody2D _rb;
        private float _timer;

        private void Awake() => _rb = GetComponent<Rigidbody2D>();

        /// <summary>Chamado toda vez que a bala sai do pool (equivalente a um "spawn").</summary>
        public void Fire(Vector2 position, Vector2 direction, float speed, Owner owner, int overrideDamage = -1)
        {
            transform.position = position;
            transform.up = direction; // gira o sprite na direção do tiro
            BulletOwner = owner;
            damage = overrideDamage > 0 ? overrideDamage : damage;
            _rb.linearVelocity = direction.normalized * speed;
            _timer = 0f;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= lifeTime) Release();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (BulletOwner == Owner.Player && other.TryGetComponent(out ShooterBoss.Boss.BossHealth boss))
            {
                boss.TakeDamage(damage);
                Release();
            }
            else if (BulletOwner == Owner.Boss && other.TryGetComponent(out ShooterBoss.Player.PlayerHealth player))
            {
                player.TakeDamage(damage);
                Release();
            }
            else if (other.CompareTag("Bounds"))
            {
                Release();
            }
        }

        private void Release()
        {
            if (gameObject.activeSelf) Pool?.Release(this);
        }

        private void OnDisable() => _rb.linearVelocity = Vector2.zero;
    }
}
