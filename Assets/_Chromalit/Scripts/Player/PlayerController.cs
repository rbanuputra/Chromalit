using UnityEngine;

namespace Chromalit.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 1.5f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.05f);
        [SerializeField] private LayerMask groundLayer;

        [Header("Coyote Time & Jump Buffer")]
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBufferTime = 0.15f;

        // Cached components
        private Rigidbody2D _rb;
        private CapsuleCollider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        // State
        private float _moveInput;
        private bool _isGrounded;
        private bool _isFacingRight = true;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private bool _jumpConsumed;

        // Modifier dari ColorState (default 1.0)
        private float _speedModifier = 1f;
        private float _jumpModifier = 1f;

        // Animator hashes (hemat performa dibanding string)
        private static readonly int AnimIsRunning = Animator.StringToHash("isRunning");
        private static readonly int AnimIsGrounded = Animator.StringToHash("isGrounded");
        private static readonly int AnimYVelocity = Animator.StringToHash("yVelocity");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CapsuleCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();

            // Freeze rotation biar karakter nggak jungkir balik
            _rb.freezeRotation = true;
        }

        private void Update()
        {
            ReadInput();
            CheckGround();
            HandleCoyoteTime();
            HandleJumpBuffer();
            HandleJump();
            FlipSprite();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            Move();
            ApplyBetterJumpGravity();
        }

        // ─── Input ───────────────────────────────────────────

        private void ReadInput()
        {
            _moveInput = Input.GetAxisRaw("Horizontal"); // A/D atau Arrow keys

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            {
                _jumpBufferTimer = jumpBufferTime;
            }
        }

        // ─── Ground Check ────────────────────────────────────

        private void CheckGround()
        {
            _isGrounded = Physics2D.OverlapBox(
                groundCheckPoint.position,
                groundCheckSize,
                0f,
                groundLayer
            );
        }

        // ─── Coyote Time ─────────────────────────────────────

        private void HandleCoyoteTime()
        {
            if (_isGrounded)
            {
                _coyoteTimer = coyoteTime;
                _jumpConsumed = false;
            }
            else
            {
                _coyoteTimer -= Time.deltaTime;
            }
        }

        // ─── Jump Buffer ─────────────────────────────────────

        private void HandleJumpBuffer()
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpBufferTimer -= Time.deltaTime;
            }
        }

        // ─── Jump ────────────────────────────────────────────

        private void HandleJump()
        {
            if (_jumpBufferTimer > 0f && _coyoteTimer > 0f && !_jumpConsumed)
            {
                float finalJumpForce = jumpForce * _jumpModifier;
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, finalJumpForce);

                _jumpBufferTimer = 0f;
                _coyoteTimer = 0f;
                _jumpConsumed = true;
            }
        }

        // ─── Movement ────────────────────────────────────────

        private void Move()
        {
            float speed = moveSpeed * _speedModifier;

            // Sprint kalau tahan Shift
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed *= sprintMultiplier;
            }

            _rb.linearVelocity = new Vector2(_moveInput * speed, _rb.linearVelocity.y);
        }

        // ─── Better Jump (lebih responsif) ───────────────────

        private void ApplyBetterJumpGravity()
        {
            if (_rb.linearVelocity.y < 0f)
            {
                // Jatuh lebih cepat
                _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime);
            }
            else if (_rb.linearVelocity.y > 0f && !(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)))
            {
                // Lepas tombol jump = lompatan lebih pendek
                _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime);
            }
        }

        // ─── Sprite Flip ─────────────────────────────────────

        private void FlipSprite()
        {
            if (_moveInput > 0f && !_isFacingRight)
            {
                Flip();
            }
            else if (_moveInput < 0f && _isFacingRight)
            {
                Flip();
            }
        }

        private void Flip()
        {
            _isFacingRight = !_isFacingRight;
            _spriteRenderer.flipX = !_isFacingRight;
        }

        // ─── Animator ────────────────────────────────────────

        private void UpdateAnimator()
        {
            if (_animator == null) return;

            _animator.SetBool(AnimIsRunning, Mathf.Abs(_moveInput) > 0.01f);
            _animator.SetBool(AnimIsGrounded, _isGrounded);
            _animator.SetFloat(AnimYVelocity, _rb.linearVelocity.y);
        }

        // ─── Public API (dipanggil PlayerColorState) ─────────

        public void SetSpeedModifier(float modifier)
        {
            _speedModifier = modifier;
        }

        public void SetJumpModifier(float modifier)
        {
            _jumpModifier = modifier;
        }

        public bool IsGrounded => _isGrounded;
    }
}