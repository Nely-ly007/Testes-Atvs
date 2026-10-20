using UnityEngine;
using ShooterBoss.Player;
using ShooterBoss.Boss;

namespace ShooterBoss.Core
{
    /// <summary>
    /// Ponto de acesso pra cena GUI encontrar PlayerHealth/BossHealth, que vivem
    /// na cena Gameplay. Com carregamento Additive em runtime não dá pra arrastar
    /// referência de um objeto de uma cena pra um objeto de outra cena direto no
    /// Inspector (elas só existem juntas se abertas ao mesmo tempo no editor) —
    /// esse singleton resolve isso sem precisar de FindObjectOfType espalhado.
    ///
    /// Coloque este componente num GameObject vazio na cena Gameplay (pode ser
    /// o mesmo objeto do GameFlowController) e arraste o Player e o Boss nos campos.
    /// </summary>
    public class GameplayRefs : MonoBehaviour
    {
        public static GameplayRefs Instance { get; private set; }

        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private BossHealth bossHealth;

        public PlayerHealth PlayerHealth => playerHealth;
        public BossHealth BossHealth => bossHealth;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Mais de um GameplayRefs na cena Gameplay — mantendo o primeiro.");
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
