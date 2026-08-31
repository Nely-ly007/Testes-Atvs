using UnityEngine;

/// <summary>
/// Controlador básico de player para plataforma 2D: movimento horizontal + pulo.
/// Usa Input.GetAxis por simplicidade/velocidade de entrega. Se quiser manter o
/// padrão Input System (Action Maps) usado no restante da disciplina, troque o
/// bloco de leitura de input pelas suas actions geradas (Move / Jump).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Checagem de chão")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private float _horizontalInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        _isGrounded = groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_horizontalInput * moveSpeed, _rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
