using UnityEngine;

//================================================================================================
// [순수 C# 클래스] 입력 데이터 수집 및 상태 결정 담당
//================================================================================================
public class PlayerInput_2D
{
    // 외부 객체가 상태를 읽을 수 있도록 프로퍼티 개방
    public float HorizontalInput { get; private set; }
    public bool IsCharging { get; private set; }
    public bool IsChargeStarted { get; private set; }
    public float CurrentChargeTime { get; private set; }
    public float CurrentJumpAngle { get; private set; }
    public bool IsJumpTriggered { get; private set; }

    // 설정값
    private float _maxChargeTime;
    private float _defaultJumpAngle;
    private float _maxJumpAngle;
    private float _minJumpAngle;

    public PlayerInput_2D(float maxChargeTime, float defaultJumpAngle, float maxJumpAngle, float minJumpAngle)
    {
        _maxChargeTime = maxChargeTime;
        _defaultJumpAngle = defaultJumpAngle;
        _maxJumpAngle = maxJumpAngle;
        _minJumpAngle = minJumpAngle;
    }

    // 외부(Player2D)에서 매 Update마다 호출하는 메인 입력 갱신 함수
    public void UpdateInput(float deltaTime, bool canJump)
    {
        IsChargeStarted = false; // 매 프레임 초기화
        HorizontalInput = Input.GetAxisRaw("Horizontal");

        if (canJump)
        {
            HandleJumpInput(deltaTime);
        }
        else if (IsCharging)
        {
            // 체공 상태 등 유예 시간이 지났으나 충전 중인 상태에서 키를 떼었을 때
            if (Input.GetKeyUp(KeyCode.Space))
            {
                TriggerJump();
            }
        }
    }

    private void HandleJumpInput(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCharging();
        }
        else if (IsCharging && Input.GetKeyUp(KeyCode.Space))
        {
            TriggerJump();
        }
        else if (IsCharging && Input.GetKey(KeyCode.Space))
        {
            UpdateCharging(deltaTime);
        }
    }

    private void StartCharging()
    {
        IsCharging = true;
        IsChargeStarted = true;
        CurrentChargeTime = 0f;
        CurrentJumpAngle = _defaultJumpAngle;
    }

    private void UpdateCharging(float deltaTime)
    {
        CurrentChargeTime += deltaTime;
        CurrentChargeTime = Mathf.Clamp(CurrentChargeTime, 0f, _maxChargeTime);

        // 상하 입력에 따른 점프 각도 조절
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (verticalInput > 0.1f) CurrentJumpAngle = _maxJumpAngle;
        else if (verticalInput < -0.1f) CurrentJumpAngle = _minJumpAngle;
        else CurrentJumpAngle = _defaultJumpAngle;
    }

    private void TriggerJump()
    {
        IsCharging = false;
        IsJumpTriggered = true;
    }

    public void ResetJumpTrigger()
    {
        IsJumpTriggered = false;
    }
}