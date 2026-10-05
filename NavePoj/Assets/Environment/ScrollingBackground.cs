using UnityEngine;

namespace ShooterBoss.Environment
{
    /// <summary>
    /// Fundo infinito: desliza a textura do material em vez de mover um sprite
    /// de verdade, então nunca "acaba" — dá a sensação de a navinha estar
    /// indo rápido. Funciona com qualquer Renderer cujo material tenha uma
    /// textura (SpriteRenderer em modo "Tiled" ou um Quad com material Unlit).
    /// </summary>
    public class ScrollingBackground : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private float scrollSpeed = 2f;
        [Tooltip("Direção do movimento aparente do fundo. Pra dar sensação de nave subindo rápido, use (0,-1): o fundo desce.")]
        [SerializeField] private Vector2 scrollDirection = new Vector2(0f, -1f);
        [Tooltip("Nome da propriedade de textura no shader. _MainTex funciona pra Sprite/Unlit padrão; URP costuma usar _BaseMap.")]
        [SerializeField] private string texturePropertyName = "_MainTex";

        private Material _material;
        private Vector2 _offset;
        private int _texPropertyId;

        private void Awake()
        {
            if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
            // .material (não .sharedMaterial) cria uma instância própria,
            // então isso não afeta outros objetos que usem o mesmo material.
            _material = targetRenderer.material;
            _texPropertyId = Shader.PropertyToID(texturePropertyName);
        }

        private void Update()
        {
            _offset += scrollDirection.normalized * scrollSpeed * Time.deltaTime;
            _material.SetTextureOffset(_texPropertyId, _offset);
        }

        private void OnDestroy()
        {
            if (_material != null) Destroy(_material); // evita vazar material instanciado
        }
    }
}
