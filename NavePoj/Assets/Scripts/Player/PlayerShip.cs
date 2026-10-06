using UnityEngine;
using UnityEngine.InputSystem;
using ShooterBoss.Pooling;
using ShooterBoss.Bullets;

namespace ShooterBoss.Player
{
    /// <summary>
    /// Movimento livre pela fase + tiro para cima (shooter vertical).
    /// Tiro único ao apertar, e tiro repetido em intervalo fixo enquanto
    /// o botão é mantido pressionado.
    /// </summary>
    public class PlayerShip : MonoBehaviour
    {
        [Header("Movimento")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private Rect movementBounds = new Rect(-4f, -4.5f, 8f, 8f); // x,y,largura,altura

        [Header("Tiro")]
        [SerializeField] private BulletPoolManager pool;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 12f;
        [SerializeField, Range(0.2f, 1f)] private float fireInterval = 0.3f;
        [SerializeField] private PlayerInput playerInput;
        [Tooltip("Nome exato da Action de tiro no Input Actions Asset.")]
        [SerializeField] private string fireActionName = "Fire";

        private Vector2 _moveInput;
        private float _fireTimer;
        private float _fireIntervalMultiplier = 1f; // powerup mexe aqui
        private InputAction _fireAction;

        private void Awake()
        {
            if (playerInput == null) playerInput = GetComponent<PlayerInput>();
            _fireAction = playerInput.actions.FindAction(fireActionName);
        }

        // --- Input System (PlayerInput com Send Messages, ou troque por Actions geradas) ---
        public void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();
        public void OnFire(InputValue value)
        {
            // Dispara imediato ao apertar. NÃO usamos isso pra saber quando parar de
            // atirar — o estado de "segurando" é lido direto da Action no Update
            // (HandleHoldToFire), porque a mensagem de "soltar" o botão nem sempre
            // chega via Send Messages dependendo da Interaction configurada na Action,
            // e se ela não chegar a navinha fica atirando pra sempre.
            if (value.isPressed) TryShoot();
        }

        private void Update()
        {
            Move();
            HandleHoldToFire();
        }

        private void Move()
        {
            Vector3 pos = transform.position + (Vector3)(_moveInput.normalized * moveSpeed * Time.deltaTime);
            pos.x = Mathf.Clamp(pos.x, movementBounds.xMin, movementBounds.xMax);
            pos.y = Mathf.Clamp(pos.y, movementBounds.yMin, movementBounds.yMax);
            transform.position = pos;
        }

        private void HandleHoldToFire()
        {
            // Fonte da verdade é o estado atual da Action, não uma flag setada
            // por evento — assim não tem como ficar "preso" atirando pra sempre.
            bool isHeld = _fireAction != null && _fireAction.IsPressed();
            if (!isHeld)
            {
                _fireTimer = 0f;
                return;
            }

            _fireTimer += Time.deltaTime;
            float interval = fireInterval * _fireIntervalMultiplier;
            if (_fireTimer >= interval)
            {
                _fireTimer = 0f;
                TryShoot();
            }
        }

        private void TryShoot()
        {
            pool.Fire(firePoint.position, Vector2.up, bulletSpeed, Bullet.Owner.Player);
            _fireTimer = 0f;
        }

        /// <summary>Powerup chama isto: multiplicador menor = atira mais rápido.</summary>
        public void SetFireRateMultiplier(float multiplier, float duration)
        {
            _fireIntervalMultiplier = multiplier;
            CancelInvoke(nameof(ResetFireRate));
            Invoke(nameof(ResetFireRate), duration);
        }

        private void ResetFireRate() => _fireIntervalMultiplier = 1f;
    }
}