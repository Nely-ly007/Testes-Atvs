using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador básico de player para plataforma 2D: movimento horizontal + pulo.
/// Usa a API do novo Input System diretamente via Keyboard.current (sem precisar
/// de um Input Actions Asset), pra ser rápido de plugar. Se quiser migrar para
/// Action Maps/PlayerInput como no restante da disciplina, é só trocar a leitura
/// de _horizontalInput e do pulo pelas suas actions.
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
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        _horizontalInput = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) _horizontalInput -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) _horizontalInput += 1f;

        _isGrounded = groundCheck != null &&
                      Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (keyboard.spaceKey.wasPressedThisFrame && _isGrounded)
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