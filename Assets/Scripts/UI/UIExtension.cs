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
    T_GameOverUI
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
        uiManager.OpenLoadingUI();
        uiManager.OpenDialogueUI("dialogue_tutorial_1_1_001");
        // 게임 로비 UI를 여기서 오픈해주자 -> uiManager.
        // MainUI도
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
