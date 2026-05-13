using System.Collections.Generic;
using UnityEngine;


//public enum GameState
//{
//    StartMenu,
//    InGame,
//    PauseGame,
//    GameOver
//}


public class MotherBrain : MonoBehaviour
{
    public static MotherBrain Instance { get; set; }


    //public GameState currentState = GameState.StartMenu;


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
}
