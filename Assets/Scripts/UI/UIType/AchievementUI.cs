using System.Collections.Generic;
using UnityEngine;

public class AchievementUI : UIBase
{
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot;
    [SerializeField] private UIButton Button_CloseSelf;

    // 슬롯들을 관리할 딕셔너리 (도전과제는 데이터 ID인 string을 키로 사용하는 것이 관리하기 좋습니다)
    //private Dictionary<string, AchievementSlotUI> _achievementSlotDict = new Dictionary<string, AchievementSlotUI>();


    private void OnEnable()
    {
        if (Button_CloseSelf != null)
            Button_CloseSelf.BindOnClickButtonEvent(Onclick_ClosePopup);

        SetAchievementSlotsOnEnable();
    }


    private void OnDisable()
    {
        if (Button_CloseSelf != null)
            Button_CloseSelf.UnBindOnClickButtonEvent(Onclick_ClosePopup);
    }


    private void SetAchievementSlotsOnEnable()
    {

    }


    private void CreateSlot(string achievementId, bool isCompleted)
    {

    }


    public void Onclick_ClosePopup()
    {
        UIManager.Instance.CloseContentUI(UIType.T_AchievementUI);
    }

}
