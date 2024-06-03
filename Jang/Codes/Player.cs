using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    [SerializeField]    private float JumpCooldown = 0f;
    [SerializeField]    private float AttackRegenCooldown = 0f;
    [SerializeField]    private float AttackCooldown = 0f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        lastY = transform.position.y;
    }
    void Update()
    {
        Move();
        Jump();
        onGround();
        Cooldown();
        Attack();
    }

    void Cooldown(){
        if(JumpCooldown > 0){
            JumpCooldown = JumpCooldown - Time.fixedDeltaTime;
        }
        if(AttackRegenCooldown > 0){ //어택쿨
            AttackRegenCooldown = AttackRegenCooldown - Time.fixedDeltaTime;
        }
        if(AttackCooldown > 0){ //어택키쿨
            AttackCooldown = AttackCooldown - Time.fixedDeltaTime;
        }
        if(AttackRegenCooldown < 0){ //어택되돌리기
            isAttack =false;
        }

    
    }
    void LateUpdate()
    {
        anim.SetFloat("movementSpeed", inputVec.magnitude);
        Flip();
        Fall();
    }



    Vector2 inputVec;
    public float movementSpeed;
    void Move()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(inputVec.x * movementSpeed * Time.fixedDeltaTime, rigid.velocity.y);
    }
    void Flip()
    {
        if (inputVec.x != 0) {
            sprite.flipX = inputVec.x < 0 ;
        }
    }

    float lastY;
    void Fall()
    {
        if ( lastY > transform.position.y)
        {
            anim.SetBool("isFall", true);
            lastY = transform.position.y; 
        }
        else if ( lastY <= transform.position.y)
        {
            anim.SetBool("isFall", false);
            lastY = transform.position.y;
        }
    }

    public bool isGround;
    public float jumpPower;
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&isGround)
        {
            if (JumpCooldown <= 0 ){
                rigid.AddForce(Vector2.up*jumpPower,ForceMode2D.Impulse); 
                JumpCooldown = 2.5f;
            }
        }
    }
    [SerializeField] private bool isAttack;
    void Attack()
    {
       if (Input.GetKeyDown(KeyCode.K))
        {
            AttackRegenCooldown = 4;
            if (AttackCooldown <= 0){
                if (isAttack != true){
                    anim.SetBool("isAttack", true);
                    anim.SetTrigger("attack1");
                    isAttack = true;
                    AttackCooldown = 1;
                }
                else if (isAttack == true){
                    anim.SetTrigger("attack2");
                    isAttack = false;
                    AttackCooldown = 1;
                }
            }
        }
    }

    public float rayLength=0.397f; // Ray의 길이
    [SerializeField] private float x;
    [SerializeField] private float y;

    void onGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + x,transform.position.y + y),new Vector3(transform.position.x + x,transform.position.y + y)* Vector2.right, rayLength, LayerMask.GetMask("Ground"));
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector3(transform.position.x - x,transform.position.y + y),new Vector3(transform.position.x - x,transform.position.y + y)* Vector2.left, rayLength, LayerMask.GetMask("Ground"));
        if (hit.collider != null || hit2.collider != null)
        {
            isGround = true;
            anim.SetBool("isGround", true);
            anim.SetBool("isJump", false);
        }
        else
        {
            isGround = false;
            anim.SetBool("isJump", true);
            anim.SetBool("isGround", false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(transform.position.x + x,transform.position.y + y), new Vector3(transform.position.x + x,transform.position.y + y) + Vector3.right * rayLength);
    }
}
