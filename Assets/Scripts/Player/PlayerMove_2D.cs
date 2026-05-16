using UnityEngine;

//================================================================================================
// [순수 C# 클래스] 물리적 이동, 점프, 지면 및 경사면 체크 담당
//================================================================================================
public class PlayerMove_2D
{
    private Rigidbody2D _rigidBody;
    private Transform _groundCheck;
    private LayerMask _groundLayer;

    private Vector2 _checkSize;
    private float _maxSlopeAngle;

    private float _moveSpeed;
    private float _airControl;
    private float _gravityScale;
    private float _chargingGravityScale;
    private float _minJumpForce;
    private float _maxJumpForce;

    // 경사면 및 점프 처리를 위한 상태 변수
    private Vector2 _slopeNormal;
    private bool _isOnSlope;
    private float _lastJumpTime; // [해결 3] 진짜 점프를 뛰었을 때를 구별하기 위한 타이머

    public bool LookRight { get; private set; } = true;

    public PlayerMove_2D(
        Rigidbody2D rb, Transform groundCheck, LayerMask groundLayer,
        Vector2 checkSize, float maxSlopeAngle,
        float moveSpeed, float airControl, float gravityScale, float chargingGravityScale,
        float minJumpForce, float maxJumpForce)
    {
        _rigidBody = rb;
        _groundCheck = groundCheck;
        _groundLayer = groundLayer;
        _checkSize = checkSize;
        _maxSlopeAngle = maxSlopeAngle;
        _moveSpeed = moveSpeed;
        _airControl = airControl;
        _gravityScale = gravityScale;
        _chargingGravityScale = chargingGravityScale;
        _minJumpForce = minJumpForce;
        _maxJumpForce = maxJumpForce;
    }

    public void InitPhysics()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidBody.gravityScale = _gravityScale;
        _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public bool CheckGrounded()
    {
        // [해결 3] 오르막길을 오를 때 발생하는 Y축 속도를 "점프 중"으로 오인하는 현상 방지
        if (Time.time - _lastJumpTime < 0.2f)
        {
            _isOnSlope = false;
            _slopeNormal = Vector2.up;
            return false;
        }

        // 지면 판정 자체는 낭떠러지 등에서 떨어지지 않게 박스캐스트를 유지합니다.
        RaycastHit2D hit = Physics2D.BoxCast(_groundCheck.position, _checkSize, 0f, Vector2.down, 0.15f, _groundLayer);

        if (hit.collider != null)
        {
            // [해결 4] 경사면 덜컹거림(Stuttering) 원인 해결
            // BoxCast는 캐릭터보다 넓어서 경사면에 '미리' 닿아버립니다. 
            // 평지인데도 각도가 꺾여 공중으로 붕 뜨는 현상을 막기 위해, 
            // 실제 각도는 정중앙에서 아래로 쏘는 얇은 선(Raycast)에서만 가져옵니다.
            RaycastHit2D rayHit = Physics2D.Raycast(_groundCheck.position, Vector2.down, 0.5f, _groundLayer);

            // 중앙 레이저가 맞았다면 그 각도를 쓰고, 낭떠러지 끝이라 빗나갔다면 BoxCast 각도를 씁니다.
            _slopeNormal = (rayHit.collider != null) ? rayHit.normal : hit.normal;

            float slopeAngle = Vector2.Angle(Vector2.up, _slopeNormal);

            if (slopeAngle > _maxSlopeAngle)
            {
                _isOnSlope = false;
                return false;
            }

            _isOnSlope = slopeAngle > 0.1f;
            return true;
        }

        _isOnSlope = false;
        return false;
    }

    public void Move(float horizontalInput, bool isCharging, bool isGrounded, bool isJumpLocked)
    {
        if (isCharging)
        {
            if (isGrounded)
            {
                // 충전 중인데 바닥이라면 완벽히 고정
                _rigidBody.gravityScale = 0f;
                _rigidBody.linearVelocity = Vector2.zero;
            }
            else
            {
                // 공중에서 충전 시 서서히 하강하도록 설정
                _rigidBody.gravityScale = _chargingGravityScale;
                _rigidBody.linearVelocity = new Vector2(0, _rigidBody.linearVelocity.y);
            }
        }
        else if (isGrounded)
        {
            // [해결 3] 지면(평지 및 허용된 경사면)에서는 중력을 0으로 만들어 미끄러짐 방지
            _rigidBody.gravityScale = 0f;

            if (horizontalInput != 0f)
            {
                // 평지와 경사면 모두, 법선(Normal)을 기준으로 수직 벡터를 구하면 완벽한 이동 방향이 나옵니다.
                Vector2 moveDirection = -Vector2.Perpendicular(_slopeNormal);
                _rigidBody.linearVelocity = moveDirection.normalized * (horizontalInput * _moveSpeed);
            }
            else
            {
                // 입력이 없으면 경사면에서도 완벽히 멈춤
                _rigidBody.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            // [해결 3-1] 공중이거나 급경사(벽)에 닿아 지면 판정이 안 될 때는 중력을 다시 복구
            _rigidBody.gravityScale = _gravityScale;

            if (horizontalInput != 0 && !isJumpLocked)
            {
                // 공중 제어 (Air Control)
                float targetSpeed = horizontalInput * _moveSpeed;
                if (Mathf.Abs(_rigidBody.linearVelocity.x) > Mathf.Abs(targetSpeed) &&
                    Mathf.Sign(_rigidBody.linearVelocity.x) == Mathf.Sign(horizontalInput))
                {
                    targetSpeed = _rigidBody.linearVelocity.x;
                }
                float newVelocityX = Mathf.Lerp(_rigidBody.linearVelocity.x, targetSpeed, _airControl * Time.fixedDeltaTime * 10f);
                _rigidBody.linearVelocity = new Vector2(newVelocityX, _rigidBody.linearVelocity.y);
            }
        }
    }

    public void ApplyChargeStartPhysics()
    {
        // 공중 충전 시 순간적으로 하강 속도 급감 (중력 스케일은 Move 함수에서 관리함)
        if (_rigidBody.linearVelocity.y < 0)
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, _rigidBody.linearVelocity.y * 0.1f);
    }

    public float CalculateJumpForce(float currentChargeTime, float maxChargeTime)
    {
        float chargeRatio = currentChargeTime / maxChargeTime;
        return _minJumpForce + (_maxJumpForce - _minJumpForce) * chargeRatio;
    }

    public Vector2 CalculateJumpVelocity(float force, float jumpAngle)
    {
        float jumpAngleRadians = jumpAngle * Mathf.Deg2Rad;
        float forceX = Mathf.Cos(jumpAngleRadians) * force;
        float forceY = Mathf.Sin(jumpAngleRadians) * force;
        if (!LookRight) forceX = -forceX;

        return new Vector2(forceX, forceY) / _rigidBody.mass;
    }

    public void ExecuteJump(float currentChargeTime, float maxChargeTime, float jumpAngle)
    {
        _lastJumpTime = Time.time; // [해결 3] 진짜 점프를 뛴 시각을 기록
        _rigidBody.gravityScale = _gravityScale;

        float force = CalculateJumpForce(currentChargeTime, maxChargeTime);
        Vector2 jumpVelocity = CalculateJumpVelocity(force, jumpAngle);

        _rigidBody.linearVelocity = Vector2.zero; // 이전 속도 무시
        _rigidBody.AddForce(jumpVelocity * _rigidBody.mass, ForceMode2D.Impulse);
    }

    public void CheckAndFlip(float horizontalInput)
    {
        if (horizontalInput > 0 && !LookRight) Flip();
        else if (horizontalInput < 0 && LookRight) Flip();
    }

    private void Flip()
    {
        LookRight = !LookRight;
        Vector3 scaler = _rigidBody.transform.localScale;
        scaler.x *= -1;
        _rigidBody.transform.localScale = scaler;
    }
}