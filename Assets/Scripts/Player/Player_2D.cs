using UnityEngine;


// +) 어떤 컴포넌트가 필수로 필요하다는 것을 강제할 수 있다
[RequireComponent(typeof(Rigidbody2D))]

public class Player_2D : MonoBehaviour
{

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _jumpForce = 12f;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform _groundCheck;    // 발 밑에 배치할 빈 오브젝트
    [SerializeField] private float _checkRadius = 0.5f; // 체크 범위
    [SerializeField] private LayerMask _groundLayer;    // 지면으로 인식할 레이어 (Platforms 등)


    private Rigidbody2D _rigidBody;
    private bool _isGrounded;
    private float _horizontalInput;
    private bool _lookRight = true;


    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        // 2D 캐릭터가 물리 충돌 시 회전해서 넘어지는 것 방지
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }


    void Update()
    {
        // 1. 입력 받기 (Update에서 수행)
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. 점프 입력
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            Jump();
        }

        // 3. 캐릭터 방향 전환 (Flip)
        if (_horizontalInput > 0 && !_lookRight)
        {
            Flip();
        }
        else if (_horizontalInput < 0 && _lookRight)
        {
            Flip();
        }
    }


    void FixedUpdate()
    {
        // 4. 지면 체크 (물리 연산 전 수행)
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);

        // 5. 좌우 이동 처리
        Move();
    }


    #region ===== [이동 함수] =====
    void Move()
    {
        // Y축 속도는 유지하면서 X축 속도만 변경 (관성 유지)
        _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
    }

    void Jump()
    {
        // 순간적인 힘을 위로 가함
        _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, _jumpForce);
    }

    void Flip()
    {
        _lookRight = !_lookRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    #endregion


    // 에디터 뷰에서 지면 체크 범위를 시각적으로 확인
    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }
    }
}
