using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 나 볼려고 적음 :
// 플레이어 공격 관련 로직 및 애니메이션 제어
// Attack은 Attack 애니메이션 특정 프레임에서 함수 호출하여서 처리함

// 현재 발생하는 문제 :
// Raycast 아직 안함
public class PlayerBattle : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator anim;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        Attack();
    }
    void LateUpdate()
    {
        AttackCooldown();
    }


    void Attack()
    {
        if(Input.GetMouseButton(0)&&anim.GetBool("isGround"))
        {
            anim.SetBool("isAttack",true);
        }
    }
    float attackCooldown;
    void AttackCooldown()
    {
        if (anim.GetInteger("combo")>0&&!anim.GetBool("isAttack"))
        {
            attackCooldown =+ Time.fixedDeltaTime;
            if (attackCooldown > 0.7f)
            {
                anim.SetInteger("combo",0);
            }
        }
    }


    void SetCombo() // Attack 첫 프레임에서 호출됨
    {
        if (anim.GetInteger("combo") < 1)
        {
            anim.SetInteger("combo",1);
        }
        else
        {
            anim.SetInteger("combo",0);
        }
    }
    void SetIsAttackFalse() // Attack 마지막 프레임에서 호출됨
    {
        anim.SetBool("isAttack",false);
    }
    void callHit() // 아마 Attack 애니메이션 프레임 중간에서 호출되어 몬스터에게 피격 판정을 가해줄 함수
    {

    }
}
