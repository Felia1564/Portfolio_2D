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

    private PlayerModel _playerModel = new PlayerModel();


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

        _playerModel.ItemList.Add(newItem);
    }
}
