using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("추적할 대상")]
    public Transform target; // 에디터에서 플레이어 오브젝트를 여기에 드래그 앤 드롭합니다.

    [Header("카메라 설정")]
    [Tooltip("카메라가 목표에 도달하는 데 걸리는 대략적인 시간입니다. (값이 작을수록 빨리 따라갑니다)")]
    public float smoothTime = 0.25f; // Lerp의 speed 대신 SmoothDamp의 time을 사용합니다.

    [Tooltip("캐릭터를 기준으로 카메라가 위치할 오프셋입니다. Y값을 올리면 카메라가 위를 비춥니다.")]
    public Vector3 offset = new Vector3(0f, 2f, -10f); // Y값을 2f로 변경하여 기본적으로 캐릭터보다 살짝 위를 비추게 합니다.

    // SmoothDamp가 현재 카메라의 이동 속도를 내부적으로 계산하고 기억하기 위한 변수
    private Vector3 velocity = Vector3.zero;

    // 플레이어의 이동이 끝난 후 카메라가 따라가도록 LateUpdate 사용
    void LateUpdate()
    {
        // 타겟(플레이어)이 파괴되었거나 할당되지 않았다면 실행하지 않음
        if (target == null) return;

        // 카메라가 있어야 할 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // Vector3.Lerp 대신 Vector3.SmoothDamp를 사용합니다.
        // 타겟과의 거리에 비례해서 자동으로 속도를 조절하는 스프링 같은 역할을 합니다.
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // 카메라 위치 업데이트
        transform.position = smoothedPosition;
    }
}