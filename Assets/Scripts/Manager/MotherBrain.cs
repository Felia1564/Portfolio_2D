using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    StartMenu,
    InGame,
    PauseGame,
    GameOver
}


public class MotherBrain : MonoBehaviour
{
    public static MotherBrain Instance { get; set; }

    public PlayerModel PlayerModel { get; private set; } = new PlayerModel();

    //private PlayerModel _playerModel = new PlayerModel();


    public GameState CurrentState { get; private set; } = GameState.StartMenu; // 기본
    //public GameState CurrentState { get; private set; } = GameState.InGame; // 테스트용 임시



    #region ==================================================================================================== [작동부]
    private void Awake()
    {
        if (Instance == null) Instance = this;

        Debug.Log("게임 매니저 Awake");


    }


    private void OnEnable()
    {
        Debug.Log("게임 매니저 OnEnable");
    }


    //private void OnDIsable()
    //{

    //}


    private void Start()
    {
        Debug.Log("게임 매니저 Start");

        UIManager.Instance.ShowStartupUIOnGameStart();

        Debug.Log("로딩 + 시작 UI 불러옴");

        // 차후 GameDataBase.cs의 CharacterData에 체력(HP) 항목이 추가되면, 
        // GameDataManager.Instance.GetCharacterData("PlayerID") 등을 활용해 값을 가져옵니다.
        int maxHpFromData = 100; // 임시 데이터

        PlayerModel.InitModel(maxHpFromData);
        Debug.Log("플레이어 체력 세팅 완료");
    }


    void FixedUpdate()
    {

    }


    void Update()
    {
        // ESC 키를 누르면 메뉴 오픈
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isPaused = UIManager.Instance.TogglePauseUI();
        }
    }

    void LateUpdate()
    {

    }
    #endregion


    #region ==================================================================================================== [게임 상태 관리]
    public void ChangeGameState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"게임 상태 변경됨: {CurrentState}");
    }


    public void StartGame()
    {
        ChangeGameState(GameState.InGame);

        Debug.Log("게임 시작");

        UIManager.Instance.OpenDialogueUI("dialogue_tutorial_1_1_001");
    }


    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다.");

#if UNITY_EDITOR
        // 유니티 에디터 환경에서 플레이 모드를 종료합니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 게임(exe, apk 등)에서 어플리케이션을 종료합니다.
        Application.Quit();
#endif
    }


    public void GameOver()
    {
        Debug.Log("게임오버");
        // 1. 플레이어 조작 비활성화 (선택 사항)
        // 2. 사운드 매니저를 통해 승리 BGM 재생
        // SoundManager.Instance.PlayBGM("Victory");

        // 3. UIManager를 통해 승리 UI 출력
        // (UIManager 구조에 맞춰 적절히 호출해주세요. 아래는 예시입니다.)
        UIManager.Instance.OpenGameOverUI();

        // 만약 UIManager가 아직 없고 직접 연결해서 쓴다면:
        // _uiVictory.ShowVictoryUI(); 
    }
    #endregion


    #region ==================================================================================================== [게임 기능]
    // 💡 1. 단순 수집품(보석 등 도감용) 획득 시 호출
    public void AddCollectedItem(string gemDataId)
    {
        if (PlayerModel == null) return;

        // PlayerModel 내부에서 중복 검사를 하고 리스트에 추가합니다.
        if (!PlayerModel.HasItem(gemDataId))
        {
            PlayerModel.CollectItem(gemDataId);

            // 💡 런타임 최적화: 도감 UI가 화면에 열려있는 상태라면 즉시 갱신하라고 UIManager에 지시!
            if (UIManager.Instance != null)
            {
                // UIManager 쪽에 이전에 만들어둔 갱신 함수 호출
                UIManager.Instance.RefreshEncyclopediaIfOpened();
            }
        }
        else
        {
            Debug.Log($"[MotherBrain] 이미 획득한 수집품입니다: {gemDataId}");
        }
    }

    // 💡 2. 스킬 해금(2단 점프, 대쉬 등) 아이템 획득 시 호출
    public void UnlockSkill(string skillId)
    {
        if (PlayerModel == null) return;

        // PlayerModel 내부에서 중복 검사를 하고 스킬을 해금합니다.
        if (!PlayerModel.HasSkill(skillId))
        {
            PlayerModel.UnlockSkill(skillId);

            // 💡 스킬을 획득했을 때 화면 중앙에 "새로운 스킬 획득!" 팝업을 띄우고 싶다면 여기서 처리
            if (UIManager.Instance != null)
            {
                // 예시: UIManager.Instance.OpenSkillUnlockPopup(skillId);
            }
        }
        else
        {
            Debug.Log($"[MotherBrain] 이미 해금된 스킬입니다: {skillId}");
        }
    }
    #endregion
}
