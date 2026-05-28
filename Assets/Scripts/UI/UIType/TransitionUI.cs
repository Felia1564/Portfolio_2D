using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TransitionUI : UIBase
{
    [SerializeField] private Animator Animator_Transition;

    private Action _onTransitionInComplete;
    private Action _onTransitionOutComplete;


    public void PlayTrasitionIn(Action onComplete)
    {
        _onTransitionInComplete = onComplete;
        Animator_Transition.Play("FadeIn");
    }

    public void PlayTrasitionOut(Action onComplete)
    {
        _onTransitionOutComplete = onComplete;
        Animator_Transition.Play("FadeOut");
    }


    //---------------------------------------------------------
    // [Animation Event] 애니메이션 클립 마지막 프레임에 달아줄 이벤트 함수
    //---------------------------------------------------------
    public void OnTransitionInCompleted()
    {
        _onTransitionInComplete?.Invoke();
    }

    public void OnTransitionOutCompleted()
    {
        _onTransitionOutComplete?.Invoke();
    }


    // =========================================================================
    // [꿀팁 추가] 에디터 이벤트 오작동을 원천 차단하는 정석 비동기 대기 함수
    // =========================================================================
    public async UniTask PlayTransitionInAsync()
    {
        // 1. 강제로 FadeIn 애니메이션 실행
        Animator_Transition.Play("FadeIn", 0, 0f);

        // 2. ⭐ [핵심 방어 코드] 애니메이터가 진짜로 'FadeIn' 상태에 진입할 때까지 대기합니다.
        // (이 코드가 없으면 이전 상태의 0초 길이를 가져와서 로딩이 바로 시작돼버립니다)
        await UniTask.WaitUntil(() => Animator_Transition.GetCurrentAnimatorStateInfo(0).IsName("FadeIn"));

        // 3. 이제 애니메이터가 확실하게 FadeIn 상태이므로, 진짜 클립의 길이(초)를 가져옵니다.
        float animDuration = Animator_Transition.GetCurrentAnimatorStateInfo(0).length;

        // 4. 현실 시간 기준으로 애니메이션 길이만큼 완벽하게 대기 (화면이 스르륵 가려지는 동안 락을 겁니다)
        await UniTask.Delay(TimeSpan.FromSeconds(animDuration), ignoreTimeScale: true);

        Debug.Log(">>> 가림막 100% 완료! 이제 맵을 로드합니다.");
    }

    public async UniTask PlayTransitionOutAsync()
    {
        // 1. 강제로 FadeOut 애니메이션 실행
        Animator_Transition.Play("FadeOut", 0, 0f);

        // 2. ⭐ 'FadeOut' 상태에 진입할 때까지 감시 및 대기
        await UniTask.WaitUntil(() => Animator_Transition.GetCurrentAnimatorStateInfo(0).IsName("FadeOut"));

        // 3. 실제 길이 측정
        float animDuration = Animator_Transition.GetCurrentAnimatorStateInfo(0).length;

        // 4. 화면이 다 밝아질 때까지 대기
        await UniTask.Delay(TimeSpan.FromSeconds(animDuration), ignoreTimeScale: true);

        Debug.Log(">>> 화면 밝히기 100% 완료! 조작을 복구합니다.");
    }
}

