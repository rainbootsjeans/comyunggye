using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 나 볼려고 적음 :
// 오브젝트 상호작용 관련 로직 및 애니메이션 제어
// 플레이어가 이 파일의 interaction()과 showInteractionKey()을 직접 호출
// 기본적으로 상호작용 가능한 오브젝트의 첫번째 자식오브젝트는 상호작용키여야함
// 매우 중요 :
// 이 스크립트를 쓰는 오브젝트에는 다음 항목들이 필수적으로 들어가야함
// 상호작용 했을 때 실행될 interaction이 있는 스크립트를 인스펙터에서 할당
// 해당 스크립트 class 선언에서,  … MonoBehaviour, IInteractable 처럼 선언되어야 함
public class Interaction : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    Transform playerTarget; // 지금 플레이어의 상호작용 타겟
    public MonoBehaviour interactableScript; // 상호작용 했을 때, 실행될 interaction()이 있는 스크립트

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playerTarget = GetComponent<PlayerInteraction>().result;
    }
    public interface IInteractable
    {
        void interaction();
    }

    public void interaction()
    {
        IInteractable interactable = interactableScript as IInteractable;
        if (interactable != null)
        {
            interactable.interaction();
        }
    }
    public void showInteractionKey()
    {
        if (playerTarget == this.transform)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
