using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealthBar : MonoBehaviour
{
    [Header("UI 연결")]
    [Tooltip("체력을 표시할 슬라이더 컴포넌트")]
    [SerializeField] private Slider _hpSlider;

    // 매 프레임 접근하기 위해 플레이어 모델을 캐싱(미리 저장)해 둡니다.
    private PlayerModel _playerModel;

    private void Start()
    {
        if (_hpSlider == null)
        {
            _hpSlider = GetComponent<Slider>();
        }

        // 1. MotherBrain에서 PlayerModel을 가져와 변수에 연결합니다.
        if (MotherBrain.Instance != null)
        {
            _playerModel = MotherBrain.Instance.PlayerModel;
        }
    }

    private void Update()
    {
        // 2. 이벤트(Action) 알람이 없으므로, 매 프레임(Update)마다 직접 체력을 확인하여 슬라이더를 갱신합니다.
        if (_playerModel != null && _hpSlider != null && _playerModel.MaxHp > 0)
        {
            // 슬라이더 값은 0 ~ 1 사이의 비율로 계산 (현재 체력 / 최대 체력)
            _hpSlider.value = (float)_playerModel.CurrentHp / _playerModel.MaxHp;
        }
    }
}