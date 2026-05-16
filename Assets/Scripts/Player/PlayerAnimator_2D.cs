using UnityEngine;

//================================================================================================
// [MonoBehaviour] 플레이어 애니메이터 제어 전담
//================================================================================================
[RequireComponent(typeof(Animator))]
public class PlayerAnimator_2D : MonoBehaviour
{
    [Header("애니메이션 컴포넌트")]
    [SerializeField] private Animator Animator_Player;

    private void Awake()
    {
        if (Animator_Player == null)
        {
            Animator_Player = GetComponent<Animator>();
        }
    }

    // 관제탑(Player2D)에서 상태가 변할 때마다 호출됨
    public void UpdateAnimation(bool isRun, bool isCharging, bool isGrounded)
    {
        if (Animator_Player == null) return;

        Animator_Player.SetBool("isRun", isRun);

        // 원본 코드에서 주석 처리되어 있던 부분도 필요하다면 아래 주석을 해제하여 사용하시면 됩니다.
        // Animator_Player.SetBool("isCharging", isCharging);
        // Animator_Player.SetBool("isGrounded", isGrounded);
    }
}