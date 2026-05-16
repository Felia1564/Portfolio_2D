using UnityEngine;

//================================================================================================
// [MonoBehaviour] 점선 궤적의 시각화 전담
//================================================================================================
public class TrajectoryRenderer : MonoBehaviour
{
    [Header("궤적 설정")]
    [SerializeField] private GameObject GameObject_DotPrefab;
    [SerializeField] private int _trajectoryStepCount = 15;
    [SerializeField] private float _trajectoryTimeStep = 0.1f;

    private GameObject[] _trajectoryDots;

    public void InitTrajectory()
    {
        if (GameObject_DotPrefab != null)
        {
            GameObject dotBox = new GameObject("Trajectory_DotBox");
            dotBox.transform.SetParent(this.transform); // 하이어라키 정리 목적

            _trajectoryDots = new GameObject[_trajectoryStepCount];
            for (int i = 0; i < _trajectoryStepCount; i++)
            {
                _trajectoryDots[i] = Instantiate(GameObject_DotPrefab, transform.position, Quaternion.identity, dotBox.transform);
                _trajectoryDots[i].SetActive(false);
            }
        }
    }

    public void SetTrajectoryActive(bool isActive)
    {
        if (_trajectoryDots == null) return;

        foreach (var dot in _trajectoryDots)
        {
            if (dot != null && dot.activeSelf != isActive)
            {
                dot.SetActive(isActive);
            }
        }
    }

    public Vector3 DrawTrajectory(Vector2 startPos, Vector2 initialVelocity, float gravityScale)
    {
        // 기본 반환값은 시작 위치로 설정
        Vector3 peakPosition = startPos;
        float highestY = float.MinValue; // 가장 높은 Y값을 추적하기 위한 기준값

        if (_trajectoryDots == null) return peakPosition;

        Vector2 gravityVector = Physics2D.gravity * gravityScale;

        for (int i = 0; i < _trajectoryDots.Length; i++)
        {
            float t = i * _trajectoryTimeStep;
            // 포물선 운동 방정식 (P = P0 + v0*t + 0.5*g*t^2)
            Vector2 position = startPos + initialVelocity * t + 0.5f * gravityVector * t * t;

            if (_trajectoryDots[i] != null)
            {
                _trajectoryDots[i].transform.position = position;
            }

            // [핵심 변경] 루프를 돌며 Y 좌표가 가장 높은 지점을 갱신합니다.
            if (position.y > highestY)
            {
                highestY = position.y;
                peakPosition = position; // 최정점 좌표 저장
            }
        }

        // 이제 최종 착지지가 아닌, 포물선의 '최고 정점' 위치를 반환합니다!
        return peakPosition;
    }
}