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


    //public GameState currentState = GameState.StartMenu;


    #region
    private void Awake()
    {
        if (Instance == null) Instance = this;

        Debug.Log("게임 매니저 Awake");
    }


    private void OnEnable()
    {
        Debug.Log("게임 매니저 OnEnable");
    }


    private void OnDIsable()
    {

    }


    private void Start()
    {
        //currentState = GameState.StartMenu;

        Debug.Log("게임 매니저 Start");

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
        
    }

    void LateUpdate()
    {

    }
    #endregion


    public void AddItem(string itemDataId, int addItemCount)
    {
        // 저장할때 고유값 ID를 부여하기 위해 사용
        long uniqueId = GameUtil_Data.GenerateUniqueId();

        // TODO : 우선 쉽게 사용할 수 있도록 중복 처리는 빼두었다. 습득할때마다 아이템이 하나씩 추가되도록 해두고
        // 추후에 중복값은 StackCount가 다 찰때까지 누적해줄 수 있도록 로직을 추가하자
        var newItem = new ItemModel();
        newItem.ItemUniqueId = uniqueId;
        newItem.ItemDataId = itemDataId;
        newItem.ItemStackCount = addItemCount;

        PlayerModel.ItemList.Add(newItem);
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
}
