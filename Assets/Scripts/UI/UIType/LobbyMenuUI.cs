using UnityEngine;

public class LobbyMenuUI : UIBase
{
    [SerializeField] private UIButton Button_GameStart;
    [SerializeField] private UIButton Button_GameQuit;


    private void OnEnable()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_GameStart);
        Button_GameQuit.BindOnClickButtonEvent(OnClick_GameQuit);
    }


    public void OnClick_GameStart()
    {
        Debug.Log("게임 시작 버튼 작동");
    }

    public void OnClick_GameQuit()
    {
        Debug.Log("게임 종료 버튼 작동");
        MotherBrain.Instance.QuitGame();
    }
}
