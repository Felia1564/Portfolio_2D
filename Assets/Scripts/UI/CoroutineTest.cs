using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoroutineTest : MonoBehaviour
{
    [Header("UI 종류")]
    [SerializeField] private Image ThemeImg;
    [SerializeField] private TextMeshProUGUI Text;


    // 캐싱: 객체를 미리 생성해두기
    private WaitForSeconds _waitThreeSeconds = new WaitForSeconds(3f);

    // 현재 팝업이 띄워져 있는지 확인
    private bool _isPopupActive = false;


    void Start() // 처음엔 UI 꺼두기
    {
        SetUIActive(false);
    }


    void Update()
    {
        // Q키를 눌렀고, 현재 팝업이 실행 중이 아닐 때만 시작
        if (Input.GetKeyDown(KeyCode.Q) && !_isPopupActive)
        {
            StartCoroutine(ShowPopupRoutine());
        }
    }


    private System.Collections.IEnumerator ShowPopupRoutine()
    {
        _isPopupActive = true;

        // 1. UI 켜기
        SetUIActive(true);
        Debug.Log("팝업 열기");

        // 2. 미리 캐싱해둔 3초 대기 실행
        yield return _waitThreeSeconds;

        // 3. UI 끄기
        SetUIActive(false);
        Debug.Log("3초 경과, 팝업 닫기");

        _isPopupActive = false;
    }


    // UI 상태를 한꺼번에 조절하는 헬퍼 메서드
    private void SetUIActive(bool isActive)
    {
        if (ThemeImg != null) ThemeImg.gameObject.SetActive(isActive);
        if (Text != null) Text.gameObject.SetActive(isActive);
    }
}
