using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaUI : UIBase
{
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot;
    [SerializeField] private UIButton Button_ChooseCollective;
    [SerializeField] private UIButton Button_CloseSelf;


    private void OnEnable()
    {
        if (Button_CloseSelf != null)
            Button_CloseSelf.BindOnClickButtonEvent(Onclick_ClosePopup);

        SetEncyclopediaSlotsOnEnable();
    }


    private void OnDisable()
    {
        if (Button_CloseSelf != null)
            Button_CloseSelf.UnBindOnClickButtonEvent(Onclick_ClosePopup);
    }


    private void SetEncyclopediaSlotsOnEnable()
    {

    }


    private void CreateSlot(string achievementId, bool isCompleted)
    {

    }

    public void Onclick_ClosePopup()
    {
        UIManager.Instance.CloseContentUI(UIType.T_EncyclopediaUI);
    }
}
