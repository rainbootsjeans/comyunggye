using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 나 볼려고 적음 :
// 플레이어 이동 관련 로직 및 애니메이션 제어
// 변수는 기본적으로 그 변수를 사용하는 함수 위에서 선언
// 한 로직에서 Animator.Set~ 를 여러번 활용할 때엔, 해당 Controller의 Parameters 변수 순서대로 활용할 것. 

// 현재 발생하는 문제 :
// 플레이어 scale 이 2,2,n 이 아닐 때, raycast를 사용하는 로직에서 오류발생
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        lastY = transform.position.y;
    }
    void FixedUpdate()
    {
        Roll();
        Move();
        Jump();
        
    }
    void LateUpdate()
    {
        anim.SetFloat("movementSpeed", inputVec.magnitude);
        onGround();
        Flip();
        Fall();
    }


    void Flip()
    {
        if (inputVec.x != 0) {
            sprite.flipX = inputVec.x < 0 ;
        }
    }
    Vector2 inputVec;
    public float movementSpeed = 150f;
    void Move()
    {
        if (anim.GetBool("isRoll")||anim.GetBool("isAttack"))
            return;
        inputVec.x = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(inputVec.x * movementSpeed * Time.fixedDeltaTime, rigid.velocity.y);
    }
    float lastY;
    void Fall()
    {
        if ( lastY > transform.position.y)
        {
            anim.SetBool("isJump", false);
            anim.SetBool("isFall", true);
            lastY = transform.position.y; 
        }
        else if ( lastY <= transform.position.y)
        {
            anim.SetBool("isFall", false);
            lastY = transform.position.y;
        }
    }
    public float jumpPower = 8f;
    void Jump()
    {
        if (Input.GetKey(KeyCode.Space)&&isGround)
        {
            if (anim.GetBool("isRoll")||anim.GetBool("isAttack"))
                return;
            anim.SetBool("isJump", true);
            rigid.AddForce(Vector2.up*jumpPower,ForceMode2D.Impulse); 
        }
    }
    public bool isGround;
    void onGround()
    {
        Debug.DrawRay(rigid.position+new Vector2(-0.52f,-0.97f),new Vector3(1.04f,0,0),new Color(1,0,0));
        RaycastHit2D hit = Physics2D.Raycast(rigid.position+new Vector2(-0.52f,-0.97f),new Vector3(1.04f,0,0), 1, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            isGround = true;
            anim.SetBool("isGround", true);
            anim.SetBool("isJump", false);
        }
        else
        {
            isGround = false;
            anim.SetBool("isGround", false);
            anim.SetBool("isJump", false);
        }
    }
    public bool isRoll;
    public float rollPower = 4f;
    void Roll()
    {
        if (Input.GetKey(KeyCode.LeftShift)&&isGround)
        {
            if (anim.GetBool("isRoll")||anim.GetBool("isJump")||anim.GetBool("isAttack"))
                return;
            if (!sprite.flipX) 
            {
                anim.SetBool("isRoll",true);
                isRoll = true;
                rigid.AddForce(new Vector2(1,0)*rollPower,ForceMode2D.Impulse); 
            }
            else
            {
                anim.SetBool("isRoll",true);
                isRoll = true;
                rigid.AddForce(new Vector2(-1,0)*rollPower,ForceMode2D.Impulse);
            }
            
        }
    }
    public void SetRollBoolValue() // Roll 애니메이션 마지막 프레임에서 호출됨
    {
        anim.SetBool("isRoll",false);
        isRoll = false;
    }

}
