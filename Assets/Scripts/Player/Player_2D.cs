using UnityEngine;

// 공통 데이터 정의
public enum PlayerState
{
    Idle,
    Charging,
    Jumping,
    Run
}

[RequireComponent(typeof(Rigidbody2D))]
public class Player_2D : MonoBehaviour
{
    //============================================================================================
    // 1. 인스펙터 설정값 (컨벤션 적용)
    //============================================================================================
    [Header("모노비헤이비어 및 유니티 참조 객체")]
    [SerializeField] private TrajectoryRenderer TrajectoryRenderer_Path;
    [SerializeField] private PlayerAnimator_2D PlayerAnimator_Anim;
    [SerializeField] private Transform Transform_GroundCheck;
    [SerializeField] private Rigidbody2D Rigidbody_Player;

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField][Range(0.1f, 1f)] private float _airControl = 0.6f;

    [Header("점프 충전 설정")]
    [SerializeField] private float _minJumpForce = 15f;
    [SerializeField] private float _maxJumpForce = 35f;
    [SerializeField] private float _maxChargeTime = 2f;
    [SerializeField] private float _jumpLockTime = 0.3f;
    [SerializeField] private float _coyoteTime = 0.1f;

    [Header("점프 각도 제어")]
    [SerializeField] private float _defaultJumpAngle = 60f;
    [SerializeField] private float _maxJumpAngle = 85f;
    [SerializeField] private float _minJumpAngle = 30f;

    [Header("물리 느낌 제어")]
    [SerializeField] private float _gravityScale = 4f;
    [SerializeField] private float _chargingGravityScale = 0.5f;

    [Header("지면 및 경사면 체크")]
    [Tooltip("지면 체크용 박스 크기 (원형 대신 박스 형태를 사용하여 벽 타기 방지)")]
    [SerializeField] private Vector2 _checkSize = new Vector2(0.8f, 0.1f);
    [Tooltip("등반 가능한 최대 경사각")]
    [SerializeField] private float _maxSlopeAngle = 60f;
    [SerializeField] private LayerMask _groundLayer;

    public bool IsCharging { get; private set; }
    public Vector3 TargetLandingPosition { get; set; }

    //============================================================================================
    // 2. 내부 컴포넌트 및 순수 C# 인스턴스
    //============================================================================================
    private PlayerInput_2D _playerInput;
    private PlayerMove_2D _playerMover;

    // 프로퍼티를 활용한 상태 변수 노출 (컨벤션 적용)
    public bool IsGrounded { get; private set; }
    public float CoyoteTimeCounter { get; private set; }
    public float CurrentJumpLockTimer { get; private set; }

    //============================================================================================
    // 3. 초기화 및 엔진 라이프사이클
    //============================================================================================
    private void Awake()
    {
        if (Rigidbody_Player == null) Rigidbody_Player = GetComponent<Rigidbody2D>();

        // 순수 C# 클래스 인스턴스화 및 설정값 주입
        _playerInput = new PlayerInput_2D(
            _maxChargeTime,
            _defaultJumpAngle,
            _maxJumpAngle,
            _minJumpAngle
        );

        _playerMover = new PlayerMove_2D(
            Rigidbody_Player,
            Transform_GroundCheck,
            _groundLayer,
            _checkSize,
            _maxSlopeAngle,
            _moveSpeed,
            _airControl,
            _gravityScale,
            _chargingGravityScale,
            _minJumpForce,
            _maxJumpForce
        );

        // 하위 모듈 초기화
        _playerMover.InitPhysics();
        TrajectoryRenderer_Path?.InitTrajectory();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        // 1) 타이머 업데이트
        if (CurrentJumpLockTimer > 0f)
            CurrentJumpLockTimer -= deltaTime;

        if (IsGrounded)
            CoyoteTimeCounter = _coyoteTime;
        else
            CoyoteTimeCounter -= deltaTime;

        // 2) 입력 처리 위임
        bool canJump = CoyoteTimeCounter > 0f || _playerInput.IsCharging;
        _playerInput.UpdateInput(deltaTime, canJump);

        // [중요 추가] 기수 컨벤션에 맞춘 프로퍼티에 현재 입력 상태를 실시간 동기화 (카메라 인식용)
        IsCharging = _playerInput.IsCharging;

        // 3) 상태에 따른 이벤트 처리 (충전 시작, 충전 중, 점프 등)
        if (_playerInput.IsChargeStarted)
        {
            _playerMover.ApplyChargeStartPhysics();

            // 충전을 시작할 때도 코요테 타임을 즉시 제거
            CoyoteTimeCounter = 0f;
        }

        // 동기화된 IsCharging 프로퍼티를 조건으로 사용합니다.
        if (IsCharging)
        {
            TrajectoryRenderer_Path?.SetTrajectoryActive(true);

            // 궤적 계산용 데이터 획득 및 그리기
            float force = _playerMover.CalculateJumpForce(_playerInput.CurrentChargeTime, _maxChargeTime);
            Vector2 initialVelocity = _playerMover.CalculateJumpVelocity(force, _playerInput.CurrentJumpAngle);

            if (TrajectoryRenderer_Path != null)
            {
                // 이제 궤적의 최종 목적지가 아닌 '최고 정점(Peak)'의 위치를 반환받아 카메라 목표지로 설정합니다.
                Vector3 peakPos = TrajectoryRenderer_Path.DrawTrajectory(Transform_GroundCheck.position, initialVelocity, _gravityScale);
                TargetLandingPosition = peakPos;
            }
        }
        else
        {
            TrajectoryRenderer_Path?.SetTrajectoryActive(false);
        }

        // 점프 트리거 체크
        if (_playerInput.IsJumpTriggered)
        {
            PerformJump();
        }

        // 4) 방향 전환 체크
        if (CurrentJumpLockTimer <= 0f && _playerInput.HorizontalInput != 0f)
        {
            _playerMover.CheckAndFlip(_playerInput.HorizontalInput);
        }

        // 5) 애니메이션 업데이트 위임
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // 지면 체크 및 물리적 이동 수행 위임
        IsGrounded = _playerMover.CheckGrounded();
        _playerMover.Move(_playerInput.HorizontalInput, _playerInput.IsCharging, IsGrounded, CurrentJumpLockTimer > 0f);
    }

    //============================================================================================
    // 4. 관제탑 내부 통제 로직
    //============================================================================================
    private void PerformJump()
    {
        IsGrounded = false;

        // [해결 1] 코요테 타임 중복 발동 방지
        // 점프를 수행하자마자 카운터를 0으로 강제 초기화하여, 점프 직후 허공에서 재점프되는 현상을 원천 차단합니다.
        CoyoteTimeCounter = 0f;

        TrajectoryRenderer_Path?.SetTrajectoryActive(false);
        CurrentJumpLockTimer = _jumpLockTime;

        // Mover를 통한 실제 물리 점프 실행
        _playerMover.ExecuteJump(_playerInput.CurrentChargeTime, _maxChargeTime, _playerInput.CurrentJumpAngle);
        _playerInput.ResetJumpTrigger();
    }

    private void UpdateAnimation()
    {
        bool isRun = Mathf.Abs(_playerInput.HorizontalInput) > 0.1f && IsGrounded && !_playerInput.IsCharging;
        PlayerAnimator_Anim?.UpdateAnimation(isRun, _playerInput.IsCharging, IsGrounded);
    }

    private void OnDrawGizmos()
    {
        if (Transform_GroundCheck != null)
        {
            Gizmos.color = Color.red;
            // 박스 형태로 기즈모를 그려 인스펙터에서 직관적으로 확인 가능하도록 변경
            Gizmos.DrawWireCube(Transform_GroundCheck.position, _checkSize);
        }
    }
}