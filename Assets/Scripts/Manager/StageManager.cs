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


    //public async UniTaskVoid LoadNextStageSequence(string nextStagePrefabPath)
    //{
    //    // =========================================================
    //    // 1. 화면 가리기 (Transition In)
    //    // =========================================================



    //    // =========================================================
    //    // 2. 로딩 UI 띄우고 다음 맵 프리팹 비동기 로드하기
    //    // =========================================================

    //    GameObject loadedStagePrefab = await ResourceManager.Inst.LoadAsset<GameObject>(nextStagePrefabPath);

    //    if (loadedStagePrefab == null)
    //    {
    //        Debug.LogError($"스테이지 로드 실패, 잘못된 주소입니다: {nextStagePrefabPath}");
    //        // 실패 시 로딩 연출만 끝내고 리턴할지, 재시도할지 등 예외 처리
    //    }


    //    // =========================================================
    //    // 3. 화면이 가려진 틈을 타 몰래 기존 맵 파괴 및 새 맵 생성
    //    // =========================================================

    //    if (_currentStageObj != null)
    //    {
    //        Destroy(_currentStageObj);
    //        // 기존 맵이 파괴되면서 남긴 찌꺼기 메모리를 확실히 청소해 줍니다.
    //        Resources.UnloadUnusedAssets();
    //    }

    //    if (loadedStagePrefab != null)
    //    {
    //        _currentStageObj = Instantiate(loadedStagePrefab);
    //    }

    //    // =========================================================
    //    // 4. 화면 밝히기 (Transition Out)
    //    // =========================================================

    //    // =========================================================
    //    // 5. 시퀀스 종료 및 정리
    //    // =========================================================

    //}
}
