//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    [Header("추적할 대상")]
//    public Transform target; // 에디터에서 플레이어 오브젝트를 여기에 드래그 앤 드롭합니다.

//    [Header("카메라 설정")]
//    [Tooltip("카메라가 목표에 도달하는 데 걸리는 대략적인 시간입니다. (값이 작을수록 빨리 따라갑니다)")]
//    public float smoothTime = 0.25f; // Lerp의 speed 대신 SmoothDamp의 time을 사용합니다.

//    [Tooltip("캐릭터를 기준으로 카메라가 위치할 오프셋입니다. Y값을 올리면 카메라가 위를 비춥니다.")]
//    public Vector3 offset = new Vector3(0f, 2f, -10f); // Y값을 2f로 변경하여 기본적으로 캐릭터보다 살짝 위를 비추게 합니다.

//    // SmoothDamp가 현재 카메라의 이동 속도를 내부적으로 계산하고 기억하기 위한 변수
//    private Vector3 velocity = Vector3.zero;

//    // 플레이어의 이동이 끝난 후 카메라가 따라가도록 LateUpdate 사용
//    void LateUpdate()
//    {
//        // 타겟(플레이어)이 파괴되었거나 할당되지 않았다면 실행하지 않음
//        if (target == null) return;

//        // 카메라가 있어야 할 목표 위치 계산
//        Vector3 desiredPosition = target.position + offset;

//        // Vector3.Lerp 대신 Vector3.SmoothDamp를 사용합니다.
//        // 타겟과의 거리에 비례해서 자동으로 속도를 조절하는 스프링 같은 역할을 합니다.
//        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

//        // 카메라 위치 업데이트
//        transform.position = smoothedPosition;
//    }
//}

//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    [Header("추적할 대상")]
//    public Transform target; // 에디터에서 플레이어 오브젝트를 여기에 드래그 앤 드롭합니다.

//    // 타겟의 Player_2D 컴포넌트를 캐싱해둘 변수
//    private Player_2D _player2D;

//    [Header("카메라 설정")]
//    [Tooltip("카메라가 목표에 도달하는 데 걸리는 대략적인 시간입니다. (값이 작을수록 빨리 따라갑니다)")]
//    public float smoothTime = 0.25f;

//    [Tooltip("캐릭터를 기준으로 카메라가 위치할 오프셋입니다.")]
//    public Vector3 offset = new Vector3(0f, 2f, -10f);

//    [Header("충전 시 카메라 당김 설정")]
//    [Tooltip("착지 예상 지점(TargetLandingPosition)으로 얼마나 당길지 비율 (0.0 ~ 1.0)")]
//    [Range(0f, 1f)]
//    public float pullRatio = 0.3f; // 0.3 이면 전체 거리의 30% 지점까지만 당겨집니다.

//    [Tooltip("카메라가 당겨지는 최대 거리 제한 (너무 멀리 화면 밖으로 나가는 것 방지)")]
//    public float maxPullDistance = 4f;

//    // SmoothDamp가 현재 카메라의 이동 속도를 내부적으로 계산하고 기억하기 위한 변수
//    private Vector3 velocity = Vector3.zero;

//    void LateUpdate()
//    {
//        // 타겟이 없다면 실행하지 않음
//        if (target == null) return;

//        // Player_2D 컴포넌트를 아직 못 찾았다면 찾아옵니다. (기존 인스펙터 연결을 깨지 않기 위함)
//        if (_player2D == null)
//            _player2D = target.GetComponent<Player_2D>();

//        // 1. 기본 목표 위치 (캐릭터 위치)
//        Vector3 basePosition = target.position;
//        Vector3 extraOffset = Vector3.zero;

//        // 2. 충전 중일 때 카메라 오프셋 추가
//        if (_player2D != null && _player2D.IsCharging)
//        {
//            // 캐릭터 위치에서 예상 착지 지점까지의 벡터 계산
//            Vector3 toLanding = _player2D.TargetLandingPosition - basePosition;
//            toLanding.z = 0f; // Z축(깊이)은 무시

//            // 지정한 비율(pullRatio)만큼만 당겨지도록 계산 (진짜 착지 지점까지 가지 않음)
//            extraOffset = toLanding * pullRatio;

//            // 만약 당겨지는 거리가 최대치(maxPullDistance)를 넘는다면 제한
//            extraOffset = Vector3.ClampMagnitude(extraOffset, maxPullDistance);
//        }

//        // 3. 최종 카메라 목표 위치 = 캐릭터 위치 + 카메라 당김 연산값 + 기본 오프셋
//        Vector3 desiredPosition = basePosition + extraOffset + offset;

//        // 4. SmoothDamp를 사용한 부드러운 이동
//        // extraOffset이 생길 때(충전 중)나 사라질 때(점프 직후) 모두 알아서 부드럽게 이동/복귀합니다.
//        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
//    }
//}

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("추적할 대상")]
    public Transform target; // 에디터에서 플레이어 오브젝트를 여기에 드래그 앤 드롭합니다.

    [Header("카메라 설정")]
    [Tooltip("카메라가 목표에 도달하는 데 걸리는 대략적인 시간입니다. (값이 작을수록 빨리 따라갑니다)")]
    public float smoothTime = 0.25f;

    [Tooltip("캐릭터를 기준으로 카메라가 기본적으로 위치할 오프셋입니다.")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("충전 시 카메라 당김(Pull) 설정")]
    [Tooltip("X축(좌우)으로 얼마나 당길지 결정하는 배수입니다. (예: 0.5 = 정점 거리의 절반만큼 당김)")]
    public float pullRatioX = 0.5f;

    [Tooltip("Y축(상하)으로 얼마나 당길지 결정하는 배수입니다. (0으로 설정하면 위아래로는 당겨지지 않음)")]
    public float pullRatioY = 0.0f;

    [Tooltip("카메라가 당겨지는 최대 거리 제한 (너무 멀리 화면 밖으로 나가는 것 방지)")]
    public float maxPullDistance = 4f;

    // SmoothDamp가 현재 카메라의 이동 속도를 내부적으로 계산하고 기억하기 위한 변수
    private Vector3 velocity = Vector3.zero;
    private Player_2D _player2D;

    void LateUpdate()
    {
        // 타겟이 없다면 실행하지 않음
        if (target == null) return;

        // Player2D 컴포넌트를 아직 못 찾았다면 찾아옵니다. (기존 인스펙터 연결 유지)
        if (_player2D == null)
            _player2D = target.GetComponent<Player_2D>();

        // 1. 기본 목표 위치 (캐릭터 위치)
        Vector3 basePosition = target.position;
        Vector3 extraOffset = Vector3.zero;

        // 2. 충전 중일 때 카메라 오프셋 추가
        if (_player2D != null && _player2D.IsCharging)
        {
            // 캐릭터 위치에서 궤적 최고점(Peak)까지의 벡터 계산
            Vector3 toPeak = _player2D.TargetLandingPosition - basePosition;
            toPeak.z = 0f; // Z축(깊이)은 무시

            // [핵심 변경] X축과 Y축에 각각 다른 배수를 적용합니다.
            extraOffset.x = toPeak.x * pullRatioX;
            extraOffset.y = toPeak.y * pullRatioY;

            // 만약 당겨지는 거리가 제한을 넘는다면, 방향은 유지한 채 길이만 제한(Clamp)합니다.
            if (extraOffset.magnitude > maxPullDistance)
            {
                extraOffset = extraOffset.normalized * maxPullDistance;
            }
        }

        // 3. 최종 목표 위치 계산 및 부드러운 이동 (SmoothDamp)
        Vector3 desiredPosition = basePosition + offset + extraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}