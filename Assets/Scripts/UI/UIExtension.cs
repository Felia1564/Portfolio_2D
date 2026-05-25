using UnityEngine;


public enum UIRootType
{
    None = 0,
    BackgroundUI,
    StartUI,
    ContentUI,
    PopupUI,
    VeryFrontUI
}


public enum UIType
{
    T_Popup,
    T_StartUI,
    T_PauseUI,
    T_LoadingUI,
    T_DialogueUI,
    T_GameOverUI,
    T_TransitionUI,
    T_AchievementUI,
    T_EncyclopediaUI
}



public static class UIExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty; // "" == string.Empty

        // 신규UI추가 2) Resources.Load를 할 경로를 직접 명시한다
        // 해당 경로는 프로젝트창에서 Resources/Prefabs/UI폴더 내에 있는 RootType 폴더명과 UIType 프리팹 이름과 동일해야 한다! (ex. ContentUI/DNMyProfilePopup)
        path = $"Prefabs/UI/{uiRootType}/{uiType}";
        return path;
    }


    public static void ShowStartupUIOnGameStart(this UIManager uiManager)
    {
        // 게임 로비 UI를 여기서 오픈해주자 -> uiManager.
        // MainUI도

        uiManager.OpenLoadingUI();

        uiManager.OpenStartUI();
    }


    public static void OpenLoadingUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenUI(UIRootType.VeryFrontUI, UIType.T_LoadingUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void OpenStartUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenUI(UIRootType.StartUI, UIType.T_StartUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static bool TogglePauseUI(this UIManager uiManager)
    {
        // 1. 본체(UIManager)에게 현재 일시정지 UI가 켜져있는지 물어봅니다.
        bool isCurrentlyOpen = uiManager.IsUIOpened(UIType.T_PauseUI);

        if (isCurrentlyOpen)
        {
            // 2. 켜져있다면 닫습니다. (CloseUI 함수가 UIManager에 있다고 가정)
            uiManager.CloseUI(UIRootType.ContentUI, UIType.T_PauseUI);

            MotherBrain.Instance.ChangeGameState(GameState.InGame);

            return false; // 이제 꺼졌음을 MotherBrain에 반환
        }

        else
        {
            // 3. 꺼져있다면 엽니다.
            var uiBase = uiManager.OpenUI(UIRootType.ContentUI, UIType.T_PauseUI);
            if (uiBase == null)
            {
                Debug.LogWarning($"UI가 생성되지 않았습니다");
                // 실패했으니 열리지 않았음(false)을 MotherBrain에 알리고 여기서 함수를 즉시 탈출합니다!
                return false;
            }
            MotherBrain.Instance.ChangeGameState(GameState.PauseGame);

            return true; // 이제 켜졌음을 MotherBrain에 반환
        }
    }

    public static void CloseLoadingUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.VeryFrontUI, UIType.T_LoadingUI);
    }


    public static void OpenDialogueUI(this UIManager uiManager, string startDialogueId)
    {
        var uiBase = uiManager.OpenContentUI(UIType.T_DialogueUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }

        MotherBrain.Instance.ChangeGameState(GameState.PauseGame);

        if (uiBase is DialogueUI dialogueUi)
        {
            dialogueUi.StartDialogue(startDialogueId);
        }
    }


    public static void OpenGameOverUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.T_GameOverUI);

        if (uiBase == null) return;
    }
}
