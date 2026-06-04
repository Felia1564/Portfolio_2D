using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks; // UniTask 사용 시

public class EncyclopediaSlotUI : MonoBehaviour
{
    [SerializeField] private Image Image_Icon; // 도화지 (인스펙터 연결)
    [SerializeField] private UIButton Button_Slot; // 버튼 (인스펙터 연결)

    private CollectionData _myGemData;
    private System.Action<CollectionData> _onSlotClickedCallback;

    // 부모(EncyclopediaUI)가 슬롯을 켜줄 때 이 함수를 부릅니다.
    public void InitSlotElement(CollectionData gemData, bool isCollected, System.Action<CollectionData> onClickCallback)
    {
        _myGemData = gemData;
        _onSlotClickedCallback = onClickCallback;

        // 1. JSON에 적힌 경로를 통해 어드레서블에서 진짜 '그림'을 가져옵니다.
        LoadAndSetIconSprite(gemData.IconPath).Forget();

        // 2. 획득 여부에 따른 알파값(반투명) 설정
        SetSlotState(isCollected);

        // 3. 버튼 클릭 이벤트 바인딩
        if (Button_Slot != null)
        {
            Button_Slot.BindOnClickButtonEvent(OnSlotClicked);
        }
    }

    // 어드레서블을 이용해 이미지를 동적으로 불러와 씌우는 함수
    private async UniTaskVoid LoadAndSetIconSprite(string iconPath)
    {
        if (string.IsNullOrEmpty(iconPath)) return;

        // ResourceManager(어드레서블)에게 해당 경로의 Sprite(그림)를 찾아오라고 시킵니다.
        Sprite loadedSprite = await ResourceManager.Inst.LoadAsset<Sprite>(iconPath);

        // 그림을 무사히 가져왔다면 도화지(Image_Icon)에 입힙니다!
        if (loadedSprite != null && Image_Icon != null)
        {
            Image_Icon.sprite = loadedSprite;
        }
    }

    // 획득 상태에 따른 투명도 제어
    private void SetSlotState(bool isCollected)
    {
        if (Image_Icon == null) return;

        Color curColor = Image_Icon.color;
        curColor.a = isCollected ? 1.0f : 0.3f; // 획득시 선명하게(100%), 미획득시 흐리게(30%)
        Image_Icon.color = curColor;
    }

    private void OnSlotClicked()
    {
        _onSlotClickedCallback?.Invoke(_myGemData);
    }
}