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
}
