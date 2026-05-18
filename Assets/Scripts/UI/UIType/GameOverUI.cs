using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameOverUI: UIBase
{
    [Header("UI 참조")]
    [SerializeField] private Button Button_Restart;

    private void Awake()
    {
        InitUIButton();
        // 시작할 때는 UI가 보이지 않도록 꺼둠
        this.gameObject.SetActive(false);
    }

    private void InitUIButton()
    {
        if (Button_Restart != null)
        {
            // 버튼 클릭 이벤트 바인딩
            Button_Restart.onClick.AddListener(OnClickRestartGame);
        }
    }

    // UIManager나 GameManager에서 이 UI를 띄울 때 호출할 함수
    public void ShowVictoryUI()
    {
        this.gameObject.SetActive(true);

        // 필요하다면 게임의 시간을 멈춤 (배경 움직임, 적 움직임 정지)
        Time.timeScale = 0f;
    }

    // 버튼 클릭 시 호출되는 함수 (동사로 시작)
    private void OnClickRestartGame()
    {
        // 정지했던 시간을 다시 원래대로 돌려놓음 (중요!)
        Time.timeScale = 1f;

        // 현재 활성화된 씬의 이름을 가져와서 다시 로드 = 게임 재시작
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void OnDestroy()
    {
        // 이벤트 해제 (메모리 누수 방지)
        if (Button_Restart != null)
        {
            Button_Restart.onClick.RemoveListener(OnClickRestartGame);
        }
    }
}
