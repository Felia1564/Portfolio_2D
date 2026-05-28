using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    // 싱글톤 패턴 (MotherBrain과 동일한 방식으로 구성)
    public static StageManager Instance { get; private set; }

    // 현재 씬에 띄워져 있는 스테이지(맵) 프리팹을 기억하기 위한 변수
    private GameObject _currentStageObj;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }


    private void Start()
    {
        string startStagePath = "Stage/stage_start_01";
        LoadNextStage(startStagePath);
    }

    public bool LoadNextStage(string nextStagePrefabPath)
    {
        GameObjManager.Inst.ClearAllFieldObjs();

        // 동기 로드, 약간의 게임 멈춤 발생함
        GameObject loadedStagePrefab = ResourceManager.Inst.LoadAssetSync<GameObject>(nextStagePrefabPath);

        if (loadedStagePrefab != null)
        {
            // [수정된 안전장치] 
            // 새 맵을 만들기 전에, 기존 맵 오브젝트를 명확하게 타겟팅하여 제거합니다.
            if (_currentStageObj != null)
            {
                GameObject mapToDestroy = _currentStageObj;
                _currentStageObj = null; // 참조를 먼저 비워줍니다.
                Destroy(mapToDestroy);   // 임시 변수에 담긴 진짜 오브젝트를 파괴합니다.
            }

            // 2. 새 맵 생성 및 기억
            _currentStageObj = Instantiate(loadedStagePrefab);
            return true;
        }

        Debug.LogError($"[{nextStagePrefabPath}] 맵 로딩에 실패했습니다.");
        return false;
    }

    public async UniTask LoadNextStageSequence(string nextStagePrefabPath, GameObject playerObj)
    {
        // =========================================================
        // 1. 플레이어 조작 끊기 (상태 변경)
        // =========================================================
        MotherBrain.Instance.ChangeGameState(GameState.PauseGame);

        TransitionUI transitionUI = UIManager.Instance.OpenTransitionUI();
        if (transitionUI == null) return; // 에러 방지 방어 코드

        // =========================================================
        // 2 & 3. 화면 전환 UI의 가림막 띄우고 가리기 애니메이션 재생 및 완벽 대기
        // =========================================================
        await transitionUI.PlayTransitionInAsync();


        // =========================================================
        // 4. 화면이 완전히 덮인 후, 맵 동기 로드 시작
        // =========================================================
        bool isSuccess = LoadNextStage(nextStagePrefabPath);

        if (!isSuccess)
        {
            UIManager.Instance.CloseTransitionUI();
            MotherBrain.Instance.ChangeGameState(GameState.InGame);
            return;
        }

        // =========================================================
        // 5-1. 동기 로드 완료 확인 후, 플레이어 (0, 0) 이동 및 물리 가속도 제거
        // =========================================================
        if (playerObj != null)
        {
            playerObj.transform.position = new Vector3(0, 0, playerObj.transform.position.z);
            Rigidbody2D playerRigid = playerObj.GetComponent<Rigidbody2D>();
            if (playerRigid != null) playerRigid.linearVelocity = Vector2.zero;
        }

        // 플레이어가 이동하고 바닥 콜라이더에 안착할 수 있도록 아주 미세한 유예 시간 부여
        await UniTask.Delay(50, ignoreTimeScale: true);


        // =========================================================
        // 5-2 & 6. 화면 밝힘 애니메이션 재생 후 UI 끄기, 조작 복구
        // =========================================================
        // 위화감 없이 빠르거나 자연스럽게 걷어내는 애니메이션 대기
        await transitionUI.PlayTransitionOutAsync();

        UIManager.Instance.CloseTransitionUI();

        // 다시 플레이어 조작 가능하게 락 해제
        MotherBrain.Instance.ChangeGameState(GameState.InGame);
    }
}
