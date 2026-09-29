using UnityEngine;
using ShooterBoss.Pooling;
using ShooterBoss.Bullets;

namespace ShooterBoss.Boss
{
    /// <summary>
    /// Fica no mesmo GameObject do chefão (que tem o Animator).
    /// Os StateMachineBehaviour pegam esta referência via
    /// animator.GetComponent<BossShooter>() dentro de OnStateEnter/Update.
    /// </summary>
    public class BossShooter : MonoBehaviour
    {
        [SerializeField] private BulletPoolManager pool;
        [SerializeField] private Transform[] firePoints; // pode ter mais de um cano
        [SerializeField] private Transform playerTarget;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private Rect movementBounds = new Rect(-3.5f, 2f, 7f, 2.5f); // área do chefão (topo da tela)

        public Transform PlayerTarget => playerTarget;
        public Rect MovementBounds => movementBounds;

        public void FireStraight(int firePointIndex, float speed, int damage = 1)
        {
            var fp = firePoints[firePointIndex];
            pool.Fire(fp.position, Vector2.down, speed, Bullet.Owner.Boss, damage);
        }

        public void FireAtPlayer(int firePointIndex, float speed, int damage = 1)
        {
            var fp = firePoints[firePointIndex];
            Vector2 dir = ((Vector2)playerTarget.position - (Vector2)fp.position).normalized;
            pool.Fire(fp.position, dir, speed, Bullet.Owner.Boss, damage);
        }

        public void FireSpread(int firePointIndex, float speed, int count, float spreadAngle, int damage = 1)
        {
            var fp = firePoints[firePointIndex];
            float start = -spreadAngle / 2f;
            float step = count > 1 ? spreadAngle / (count - 1) : 0f;
            for (int i = 0; i < count; i++)
            {
                float angle = start + step * i;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.down;
                pool.Fire(fp.position, dir, speed, Bullet.Owner.Boss, damage);
            }
        }

        public void MoveHorizontal(float direction, float speedMultiplier = 1f)
        {
            Vector3 pos = transform.position + Vector3.right * direction * moveSpeed * speedMultiplier * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, movementBounds.xMin, movementBounds.xMax);
            transform.position = pos;
        }

        public void MoveTowardsPlayerX(float speedMultiplier = 1f)
        {
            float dir = Mathf.Sign(playerTarget.position.x - transform.position.x);
            MoveHorizontal(dir, speedMultiplier);
        }
    }
}
