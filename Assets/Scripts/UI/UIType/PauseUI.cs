using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : UIBase
{
    [SerializeField] private UIButton Button_Encyclopedia;
    [SerializeField] private UIButton Button_Achivement;
    [SerializeField] private UIButton Button_Title;
    [SerializeField] private UIButton Button_Quit;


    private void OnEnable()
    {
        Button_Encyclopedia.BindOnClickButtonEvent(OnClick_GameEncyclopedia);
        Button_Achivement.BindOnClickButtonEvent(OnClick_GameAchivement);
        Button_Title.BindOnClickButtonEvent(OnClick_GameTitle);
        Button_Quit.BindOnClickButtonEvent(OnClick_GameQuit);
    }


    public void OnClick_GameEncyclopedia()
    {
        Debug.Log("게임 도감 버튼 작동");
    }

    public void OnClick_GameAchivement()
    {
        Debug.Log("게임 도전과제 버튼 작동");
    }

    public void OnClick_GameTitle() // 나중에 강사님께 질문해보기
    {
        Debug.Log("게임 타이틀 버튼 작동");

        SceneManager.LoadScene("2D_Test");

        //MotherBrain.Instance.ChangeGameState(GameState.StartMenu);
        //UIManager.Instance.OpenStartUI();
    }

    public void OnClick_GameQuit()
    {
        Debug.Log("게임 종료 버튼 작동");
        MotherBrain.Instance.QuitGame();
    }

}
