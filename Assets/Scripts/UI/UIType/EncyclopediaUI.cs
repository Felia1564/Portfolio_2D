//using System.Collections.Generic;
//using UnityEngine;

//public class EncyclopediaUI : UIBase
//{
//    [SerializeField] private GameObject Prefab_Slot;
//    [SerializeField] private Transform Transform_UISlotRoot;
//    [SerializeField] private UIButton Button_ChooseCollective;
//    [SerializeField] private UIButton Button_CloseSelf;


//    private void OnEnable()
//    {
//        if (Button_CloseSelf != null)
//            Button_CloseSelf.BindOnClickButtonEvent(Onclick_ClosePopup);

//        SetEncyclopediaSlotsOnEnable();
//    }


//    private void OnDisable()
//    {
//        if (Button_CloseSelf != null)
//            Button_CloseSelf.UnBindOnClickButtonEvent(Onclick_ClosePopup);
//    }


//    private void SetEncyclopediaSlotsOnEnable()
//    {

//    }


//    private void CreateSlot(string achievementId, bool isCompleted)
//    {

//    }

//    public void Onclick_ClosePopup()
//    {
//        UIManager.Instance.CloseContentUI(UIType.T_EncyclopediaUI);
//    }
//}


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaUI : UIBase
{
    [Header("좌측 스크롤 리스트 셋업")]
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot;
    [SerializeField] private UIButton Button_ChooseCollective;
    [SerializeField] private UIButton Button_CloseSelf;

    [Header("우측 상세 정보 패널 (인스펙터 연결 필수)")]
    [SerializeField] private TextMeshProUGUI Text_CollectionName;
    [SerializeField] private TextMeshProUGUI Text_CollectionDescription;

    // 런타임에 동적 생성된 슬롯 오브젝트들을 청소하기 위해 추적하는 바구니
    private List<GameObject> _spawnedSlots = new List<GameObject>();

    private void OnEnable()
    {
        if (Button_CloseSelf != null)
            Button_CloseSelf.BindOnClickButtonEvent(Onclick_ClosePopup);

        // UI 활성화 타이밍에 리스트 구축
        RefreshEncyclopediaUI();
    }

    private void OnDisable()
    {
        if (Button_CloseSelf != null)
            Button_CloseSelf.UnBindOnClickButtonEvent(Onclick_ClosePopup);

        ClearOldSlots();
    }

    // 💡 전체 수집품 리스트를 새로고침하여 화면에 그리는 함수
    public void RefreshEncyclopediaUI()
    {
        ClearOldSlots();

        if (GameDataManager.Instance == null || MotherBrain.Instance == null) return;

        // 우측 상세 패널 기본 안내 세팅
        Text_CollectionName.text = "수집품 선택";
        Text_CollectionDescription.text = "좌측 리스트에서 보석을 클릭하면 상세 설명이 표시됩니다.";

        // GameDataManager에 파싱되어 보관중인 수집용 마스터 데이터 딕셔너리를 전체 순회
        // (json 구조 래핑 클래스 이름이 ItemCollectionData 형태라고 상정)
        foreach (var metaData in GameDataManager.Instance.CollectionDataList.Values)
        {
            // 1. 모델 레이어에 이 보석 ID를 먹은 적이 있는지 정밀 대조 판별 ⭐
            bool isCollected = MotherBrain.Instance.PlayerModel.HasItem(metaData.Id);

            // 2. 슬롯 껍데기 프리팹 동적 생성 (기수 UI 생성 규칙 준수)
            GameObject slotGO = Instantiate(Prefab_Slot, Transform_UISlotRoot, false);
            _spawnedSlots.Add(slotGO);

            // 3. 알맹이 컴포넌트를 찾아 데이터, 획득여부, 클릭이벤트 콜백(OnSelectSlot)을 안전하게 주입(Dependency Injection)
            var slotScript = slotGO.GetComponent<EncyclopediaSlotUI>();
            if (slotScript != null)
            {
                slotScript.InitSlotElement(metaData, isCollected, OnSelectSlot);
            }
        }
    }

    // 💡 좌측 슬롯 스크립트가 버튼 클릭을 감지하면 콜백받아 우측 텍스트를 채우는 타겟 함수
    private void OnSelectSlot(CollectionData selectedGemData)
    {
        if (selectedGemData == null) return;

        // 기획 규칙: 미획득 상태라도 이름과 정보는 정상 표시되도록 세팅
        Text_CollectionName.text = selectedGemData.Name;
        Text_CollectionDescription.text = selectedGemData.Description;
    }

    private void ClearOldSlots()
    {
        foreach (var slot in _spawnedSlots)
        {
            if (slot != null) Destroy(slot);
        }
        _spawnedSlots.Clear();
    }

    public void Onclick_ClosePopup()
    {
        UIManager.Instance.CloseContentUI(UIType.T_EncyclopediaUI);
    }
}