using UnityEngine;

namespace ShooterBoss.Environment
{
    /// <summary>
    /// Fundo infinito feito com 2+ cópias do mesmo sprite empilhadas verticalmente,
    /// sem gap entre elas. Quando uma peça sai completamente por baixo da câmera,
    /// ela é teleportada pro topo da pilha — dá a ilusão de loop infinito.
    ///
    /// Mais simples e confiável que mexer em offset de material/Tiled: não depende
    /// de Wrap Mode da textura nem quebra se o sprite estiver num Sprite Atlas.
    /// </summary>
    public class ScrollingBackgroundSprites : MonoBehaviour
    {
        [Tooltip("As cópias do sprite de fundo, empilhadas verticalmente sem espaço entre elas (ex: 2 ou 3 peças, todas do mesmo tamanho).")]
        [SerializeField] private Transform[] pieces;
        [SerializeField] private float scrollSpeed = 3f;
        [SerializeField] private Camera targetCamera;

        private float _pieceHeight;
        private float _bottomLimitY;

        private void Start()
        {
            if (targetCamera == null) targetCamera = Camera.main;

            var sr = pieces[0].GetComponent<SpriteRenderer>();
            _pieceHeight = sr.bounds.size.y;

            // um pouco abaixo da borda inferior da câmera: só reposiciona
            // depois que a peça já sumiu de vista por completo.
            float viewHalfHeight = targetCamera.orthographicSize;
            _bottomLimitY = targetCamera.transform.position.y - viewHalfHeight - _pieceHeight;
        }

        private void Update()
        {
            float step = scrollSpeed * Time.deltaTime;

            foreach (var piece in pieces)
                piece.position += Vector3.down * step;

            foreach (var piece in pieces)
            {
                if (piece.position.y < _bottomLimitY)
                {
                    float topMostY = HighestY();
                    piece.position = new Vector3(piece.position.x, topMostY + _pieceHeight, piece.position.z);
                }
            }
        }

        private float HighestY()
        {
            float max = float.NegativeInfinity;
            foreach (var p in pieces)
                if (p.position.y > max) max = p.position.y;
            return max;
        }
    }
}
