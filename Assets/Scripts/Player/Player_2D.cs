//using UnityEngine;

//public enum PlayerState
//{
//    Idle
//}


//// +) 어떤 컴포넌트가 필수로 필요하다는 것을 강제할 수 있다
//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]

//public class Player_2D : MonoBehaviour
//{

//    [Header("이동 설정")]
//    [SerializeField] private float _moveSpeed = 8f;
//    [SerializeField] private float _jumpForce = 12f;

//    [Header("지면 체크 설정")]
//    [SerializeField] private Transform _groundCheck;    // 발 밑에 배치할 빈 오브젝트
//    [SerializeField] private float _checkRadius = 0.5f; // 체크 범위
//    [SerializeField] private LayerMask _groundLayer;    // 지면으로 인식할 레이어 (Platforms 등)


//    private Rigidbody2D _rigidBody;
//    private bool _isGrounded;
//    private float _horizontalInput;
//    private bool _lookRight = true;

//    private Animator _animator;
//    private bool isRun;


//    void Awake()
//    {
//        _rigidBody = GetComponent<Rigidbody2D>();
//        _animator = GetComponent<Animator>();

//        // 2D 캐릭터가 물리 충돌 시 회전해서 넘어지는 것 방지
//        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
//    }


//    void Update()
//    {
//        // 1. 입력 받기 (Update에서 수행)
//        _horizontalInput = Input.GetAxisRaw("Horizontal");

//        // 2. 점프 입력
//        if (Input.GetButtonDown("Jump") && _isGrounded)
//        {
//            Jump();
//        }

//        // 3. 캐릭터 방향 전환 (Flip)
//        if (_horizontalInput > 0 && !_lookRight)
//        {
//            Flip();
//        }
//        else if (_horizontalInput < 0 && _lookRight)
//        {
//            Flip();
//        }

//        UpdateAnimation();
//    }


//    void FixedUpdate()
//    {
//        // 4. 지면 체크 (물리 연산 전 수행)
//        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);

//        // 5. 좌우 이동 처리
//        Move();
//    }


//    #region ===== [이동 함수] =====
//    void Move()
//    {
//        // Y축 속도는 유지하면서 X축 속도만 변경 (관성 유지)
//        _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
//    }

//    void Jump()
//    {
//        // 순간적인 힘을 위로 가함
//        _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, _jumpForce);
//    }

//    void Flip()
//    {
//        _lookRight = !_lookRight;
//        Vector3 scaler = transform.localScale;
//        scaler.x *= -1;
//        transform.localScale = scaler;
//    }
//    #endregion


//    // 에디터 뷰에서 지면 체크 범위를 시각적으로 확인
//    private void OnDrawGizmos()
//    {
//        if (_groundCheck != null)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
//        }
//    }


//    #region ===== [AI - 애니메이션 함수] =====
//    void UpdateAnimation()
//    {
//        // _horizontalInput이 0이 아니라면(즉, 좌우 키를 누르고 있다면) 이동 중인 것으로 간주
//        // Mathf.Abs를 사용하여 음수(-1) 입력도 양수(1)로 바꿔서 확인합니다.
//        isRun = Mathf.Abs(_horizontalInput) > 0.1f;

//        // 애니메이터의 "isRun" 파라미터에 값을 전달합니다.
//        _animator.SetBool("isRun", isRun);
//    }
//    #endregion
//}





using UnityEngine;

//================================================================================================
// 1. 데이터 정의 (Data Definitions)
//================================================================================================
public enum PlayerState
{
    Idle,
    Charging,
    Jumping,
    Run
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player_2D : MonoBehaviour
{
    //============================================================================================
    // 2. 인스펙터 설정값 (Serialized Fields / Settings)
    //============================================================================================
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;
    [Tooltip("공중에서 방향을 틀 수 있는 정도 (0.1 ~ 1.0)")]
    [SerializeField][Range(0.1f, 1f)] private float _airControl = 0.6f;

    [Header("점프 충전 설정")]
    [SerializeField] private float _minJumpForce = 15f;
    [SerializeField] private float _maxJumpForce = 35f;
    [SerializeField] private float _maxChargeTime = 2f;

    [Tooltip("점프 직후 방향 전환이 불가능한 시간 (초)")]
    [SerializeField] private float _jumpLockTime = 0.3f;
    [Tooltip("플랫폼에서 떨어져도 점프가 가능한 유예 시간 (코요테 타임)")]
    [SerializeField] private float _coyoteTime = 0.1f;

    [Header("점프 각도 제어")]
    [SerializeField] private float _defaultJumpAngle = 60f;
    [SerializeField] private float _maxJumpAngle = 85f;
    [SerializeField] private float _minJumpAngle = 30f;

    [Header("물리 느낌 제어")]
    [SerializeField] private float _gravityScale = 4f;
    [SerializeField] private float _chargingGravityScale = 0.5f;

    [Header("궤적(점선) 시각화")]
    [Tooltip("궤적을 그릴 점(Dot) 프리팹")]
    [SerializeField] private GameObject _dotPrefab;
    [Tooltip("포물선을 구성하는 점의 개수")]
    [SerializeField] private int _trajectoryStepCount = 15;
    [Tooltip("각 점 사이의 시간 간격")]
    [SerializeField] private float _trajectoryTimeStep = 0.1f;

    [Header("지면 체크 레이어")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _checkRadius = 0.5f;
    [SerializeField] private LayerMask _groundLayer;

    //============================================================================================
    // 3. 내부 컴포넌트 및 상태 변수 (Private States)
    //============================================================================================
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private GameObject[] _trajectoryDots;

    private bool _isGrounded;
    private float _horizontalInput;
    private bool _lookRight = true;

    private bool _isCharging = false;
    private float _currentChargeTime = 0f;
    private float _currentJumpAngle;
    private float _currentJumpLockTimer = 0f;
    private float _coyoteTimeCounter = 0f;
    private bool isRun;

    //============================================================================================
    // 4. 초기화 및 엔진 라이프사이클 (Unity Lifecycle)
    //============================================================================================
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidBody.gravityScale = _gravityScale;
        _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        InitializeTrajectory();
    }

    void Update()
    {
        // 입력 수집 및 타이머 업데이트
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        if (_currentJumpLockTimer > 0f)
            _currentJumpLockTimer -= Time.deltaTime;

        // 지면 유예 시간(코요테 타임) 관리
        if (_isGrounded)
            _coyoteTimeCounter = _coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;

        // 점프 조작 가능 상태일 때 입력 처리
        if (_coyoteTimeCounter > 0f || _isCharging)
            HandleJumpInput();

        // 방향 전환 체크
        if (_currentJumpLockTimer <= 0f && _horizontalInput != 0f)
        {
            if (_horizontalInput > 0 && !_lookRight) Flip();
            else if (_horizontalInput < 0 && _lookRight) Flip();
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // 물리 기반 지면 체크
        _isGrounded = _rigidBody.linearVelocity.y > 0.1f ? false : Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);

        Move();
    }

    //============================================================================================
    // 5. 핵심 로직: 점프 시퀀스 (Jump Input & Logic)
    //============================================================================================
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCharging();
        }
        else if (_isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            Jump();
        }
        else if (_isCharging && Input.GetKey(KeyCode.Space))
        {
            UpdateCharging();
        }
    }

    private void StartCharging()
    {
        _isCharging = true;
        _currentChargeTime = 0f;
        _coyoteTimeCounter = 0f;
        _currentJumpAngle = _defaultJumpAngle;
        _rigidBody.gravityScale = _chargingGravityScale;
        _rigidBody.linearVelocity = Vector2.zero;

        // 공중 충전 시 하강 속도 급감 (체공 시간 부여)
        if (_rigidBody.linearVelocity.y < 0)
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, _rigidBody.linearVelocity.y * 0.1f);

        SetTrajectoryActive(true);
    }

    private void UpdateCharging()
    {
        _currentChargeTime += Time.deltaTime;
        _currentChargeTime = Mathf.Clamp(_currentChargeTime, 0f, _maxChargeTime);

        // 상하 입력에 따른 점프 각도 조절
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (verticalInput > 0.1f) _currentJumpAngle = _maxJumpAngle;
        else if (verticalInput < -0.1f) _currentJumpAngle = _minJumpAngle;
        else _currentJumpAngle = _defaultJumpAngle;

        DrawTrajectory();
    }

    //============================================================================================
    // 6. 물리적 이동 수행 (Physics Movement)
    //============================================================================================
    void Move()
    {
        if (_isCharging)
        {
            // 충전 중에는 좌우 이동 불가 (제자리 유지)
            _rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);
        }
        else if (_isGrounded)
        {
            _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
        }
        else if (_horizontalInput != 0 && _currentJumpLockTimer <= 0f)
        {
            // 공중 제어 (Air Control)
            float targetSpeed = _horizontalInput * _moveSpeed;
            if (Mathf.Abs(_rigidBody.linearVelocity.x) > Mathf.Abs(targetSpeed) &&
                Mathf.Sign(_rigidBody.linearVelocity.x) == Mathf.Sign(_horizontalInput))
            {
                targetSpeed = _rigidBody.linearVelocity.x;
            }
            float newVelocityX = Mathf.Lerp(_rigidBody.linearVelocity.x, targetSpeed, _airControl * Time.fixedDeltaTime * 10f);
            _rigidBody.linearVelocity = new Vector2(newVelocityX, _rigidBody.linearVelocity.y);
        }
    }

    void Jump()
    {
        _isCharging = false;
        _isGrounded = false;
        SetTrajectoryActive(false);
        _rigidBody.gravityScale = _gravityScale;
        _currentJumpLockTimer = _jumpLockTime;

        float chargeRatio = _currentChargeTime / _maxChargeTime;
        float currentForce = _minJumpForce + (_maxJumpForce - _minJumpForce) * chargeRatio;

        float jumpAngleRadians = _currentJumpAngle * Mathf.Deg2Rad;
        float forceX = Mathf.Cos(jumpAngleRadians) * currentForce;
        float forceY = Mathf.Sin(jumpAngleRadians) * currentForce;
        if (!_lookRight) forceX = -forceX;

        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
    }

    void Flip()
    {
        _lookRight = !_lookRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    //============================================================================================
    // 7. 시각적 피드백: 궤적 및 애니메이션 (Visuals & Feedback)
    //============================================================================================
    private void InitializeTrajectory()
    {
        if (_dotPrefab != null)
        {
            GameObject dotBox = new GameObject("Trajectory_DotBox");
            _trajectoryDots = new GameObject[_trajectoryStepCount];
            for (int i = 0; i < _trajectoryStepCount; i++)
            {
                _trajectoryDots[i] = Instantiate(_dotPrefab, transform.position, Quaternion.identity, dotBox.transform);
                _trajectoryDots[i].SetActive(false);
            }
        }
    }

    private void SetTrajectoryActive(bool isActive)
    {
        if (_trajectoryDots == null) return;
        foreach (var dot in _trajectoryDots)
        {
            if (dot != null) dot.SetActive(isActive);
        }
    }

    private void DrawTrajectory()
    {
        if (_trajectoryDots == null) return;

        float chargeRatio = _currentChargeTime / _maxChargeTime;
        float currentForce = _minJumpForce + (_maxJumpForce - _minJumpForce) * chargeRatio;

        float jumpAngleRadians = _currentJumpAngle * Mathf.Deg2Rad;
        float forceX = Mathf.Cos(jumpAngleRadians) * currentForce;
        float forceY = Mathf.Sin(jumpAngleRadians) * currentForce;
        if (!_lookRight) forceX = -forceX;

        Vector2 initialVelocity = new Vector2(forceX, forceY) / _rigidBody.mass;
        Vector2 gravityVector = Physics2D.gravity * _gravityScale;
        Vector2 startPosition = _groundCheck.position;

        for (int i = 0; i < _trajectoryDots.Length; i++)
        {
            float t = i * _trajectoryTimeStep;
            Vector2 pointPosition = startPosition + (initialVelocity * t) + (0.5f * gravityVector * t * t);
            _trajectoryDots[i].transform.position = pointPosition;
        }
    }

    private void UpdateAnimation()
    {
        isRun = Mathf.Abs(_horizontalInput) > 0.1f && _isGrounded && !_isCharging;
        _animator.SetBool("isRun", isRun);
        // _animator.SetBool("isCharging", _isCharging);
        // _animator.SetBool("isGrounded", _isGrounded);
    }

    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }
    }
}