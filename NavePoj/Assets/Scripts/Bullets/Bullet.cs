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
        [Tooltip("Margem extra (em viewport, 0-1) além da tela antes de reciclar a bala. 0 = recicla exatamente na borda.")]
        [SerializeField] private float offscreenMargin = 0.1f;

        public Owner BulletOwner { get; private set; }
        public IObjectPool<Bullet> Pool { get; set; }

        private Rigidbody2D _rb;
        private float _timer;
        private bool _released; // trava contra Release() duplicado no mesmo frame
        private static Camera _cam;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_cam == null) _cam = Camera.main;
        }

        /// <summary>Chamado toda vez que a bala sai do pool (equivalente a um "spawn").</summary>
        public void Fire(Vector2 position, Vector2 direction, float speed, Owner owner, int overrideDamage = -1)
        {
            transform.position = position;
            transform.up = direction; // gira o sprite na direção do tiro
            BulletOwner = owner;
            damage = overrideDamage > 0 ? overrideDamage : damage;
            _rb.linearVelocity = direction.normalized * speed;
            _timer = 0f;
            _released = false;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= lifeTime || IsOffscreen())
            {
                Release();
            }
        }

        /// <summary>
        /// Substitui a antiga checagem por tag "Bounds" (que não existia no projeto
        /// e gerava o erro "Tag: Bounds is not defined"). Usa a câmera pra saber se
        /// a bala saiu da área visível, sem precisar de collider/tag extra.
        /// </summary>
        private bool IsOffscreen()
        {
            if (_cam == null) return false; // sem câmera, confia só no lifeTime
            Vector3 vp = _cam.WorldToViewportPoint(transform.position);
            return vp.x < -offscreenMargin || vp.x > 1f + offscreenMargin
                || vp.y < -offscreenMargin || vp.y > 1f + offscreenMargin;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_released) return; // já liberada neste frame por outro callback de trigger

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
        }

        private void Release()
        {
            // _released vira true ANTES de chamar o pool, então uma segunda
            // chamada no mesmo frame (dois triggers, ou trigger + lifeTime)
            // nunca chega a liberar o mesmo objeto duas vezes.
            if (_released) return;
            _released = true;
            Pool?.Release(this);
        }

        private void OnDisable() => _rb.linearVelocity = Vector2.zero;
    }
}
