using UnityEngine;
using UnityEngine.Pool;
using ShooterBoss.Bullets;

namespace ShooterBoss.Pooling
{
    /// <summary>
    /// Um componente = uma pool. Coloque uma instância para a bala do player
    /// e outra para a bala do chefão (prefabs diferentes ou tag diferente).
    /// Usa a classe pronta da Unity (UnityEngine.Pool.ObjectPool<T>),
    /// exigido pelo enunciado — não reimplementa o padrão Pool.
    /// </summary>
    public class BulletPoolManager : MonoBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxSize = 100;
        [SerializeField] private bool collectionChecks = true;

        private IObjectPool<Bullet> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<Bullet>(
                createFunc: CreateBullet,
                actionOnGet: b => { if (b != null) b.gameObject.SetActive(true); },
                actionOnRelease: b => { if (b != null) b.gameObject.SetActive(false); },
                // Ao sair do Play ou descarregar a cena (ex: Gameplay -> Resultado), a Unity
                // já destruiu as balas e o Clear() da pool ainda chama este callback nelas.
                // "b != null" usa o null-check da Unity, que detecta objeto destruído.
                actionOnDestroy: b => { if (b != null) Destroy(b.gameObject); },
                collectionCheck: collectionChecks,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);
        }

        private Bullet CreateBullet()
        {
            Bullet b = Instantiate(bulletPrefab, transform);
            b.Pool = _pool;
            b.gameObject.SetActive(false);
            return b;
        }

        /// <summary>Pega uma bala da pool e já dispara na direção informada.</summary>
        public Bullet Fire(Vector2 position, Vector2 direction, float speed, Bullet.Owner owner, int damage = -1)
        {
            Bullet b = _pool.Get();

            // Defensivo: se alguma bala do pool foi destruída por fora (ex: cena recarregada),
            // descarta e pega outra em vez de estourar MissingReferenceException.
            while (b == null)
                b = _pool.Get();

            b.Fire(position, direction, speed, owner, damage);
            return b;
        }
    }
}