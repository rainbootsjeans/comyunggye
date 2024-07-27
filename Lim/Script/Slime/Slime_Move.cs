using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Move : MonoBehaviour
{

    [SerializeField] Transform Target;//플레이어
    [SerializeField] GameObject Child;//자식 오브젝트
    Animator Anim;
    SpriteRenderer Sp;
    Rigidbody2D rigid;

    public int Max_HP=3;
    int cur_HP;
    public float E_speed = 0.1f;
    bool isdie = false;//사망시 움직임 방지
    bool isHit = false;

    public float knockbackForce = 1f; // 넉백 힘
    public float knockbackDuration = 0.5f; // 넉백 지속 시간
    public bool isKnockedBack = false; // 넉백 상태 여부를 나타내는 변수
    private float knockbackTimer = 0f; // 넉백 지속 시간을 계산하는 타이머

    public Vector3 moveVelocity;

    void Start()
    {
        cur_HP = Max_HP;
        rigid=GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        Sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
            }
        }
        if (!isdie&&!isKnockedBack&!isHit)
        {
            Move();
            transform.position += moveVelocity * 6 * E_speed * Time.deltaTime;
        }
        if (isdie)
        {
            StartCoroutine("Death");
        }
    }
    IEnumerator Death()
    {
        Anim.SetTrigger("Die");
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
    void Move()
    {
        if (Mathf.Abs(Target.position.x - this.transform.position.x)>=0.5f)
        {
            Anim.SetBool("IsMove", true);
            Anim.SetBool("IsIdle", false);
            if (Target.position.x < this.transform.position.x)
            {
                StartCoroutine(TurningDelay(Vector3.left));
            }
            else if(Target.position.x > this.transform.position.x)
            {
                StartCoroutine(TurningDelay(Vector3.right));
            }
        }
        else
        {
            StartCoroutine(TurningDelay(Vector3.zero));
            Anim.SetBool("IsMove", false);
            Anim.SetBool("IsIdle", true);
        }

    }
    IEnumerator AfterHit()
    {
        yield return new WaitForSeconds(1f);
        isHit = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "P_Attack"&&!isHit)
        {

            cur_HP -= 1;
            isHit = true;
            Anim.SetTrigger("Hurt");
            Vector2 KnockbackVelocity = Vector2.zero;//넉백 발생
            if (collision.gameObject.transform.position.x > this.transform.position.x)
            {
                KnockbackVelocity = new Vector2(-1, 0);
            }
            else if (collision.gameObject.transform.position.x < this.transform.position.x)
            {
                KnockbackVelocity = new Vector2(1, 0);
            }

            if (cur_HP <= 0)//죽음
            {
                gameObject.layer = 9;
                Child.layer = 9;
                isdie = true;
                Anim.SetBool("IsDie", true);
            }
            else
            {
                StartCoroutine("AfterHit");
                Knockback(KnockbackVelocity);
            }
        }
        else if (collision.gameObject.tag == "Player" &&collision.name!="ItemRange")//플레이어와 접촉시 플레이어에게 넉백 발생
        {
            bool isPlayerKnockback=collision.GetComponent<PlayerMove>().ReturnKnockback();
            if (!isPlayerKnockback)
            {
                collision.GetComponent<Animator>().SetTrigger("Hit");
                Debug.Log(collision.gameObject.name);
                Vector2 PKnockbackVelocity = Vector2.zero;
                if (collision.gameObject.transform.position.x < this.transform.position.x)
                {
                    PKnockbackVelocity = new Vector2(-1, 1);
                }
                else if (collision.gameObject.transform.position.x > this.transform.position.x)
                {
                    PKnockbackVelocity = new Vector2(1, 1);
                }

                collision.GetComponent<PlayerMove>().Knockback(PKnockbackVelocity);
            }
        }
    }
    IEnumerator TurningDelay(Vector3 direction)
    {
        yield return new WaitForSeconds(0.5f);
        moveVelocity = direction;
        if (direction == Vector3.right)
        {
            Sp.flipX = true;
        }
        else if (direction == Vector3.left)
        {
            Sp.flipX = false;
        }
    }
    // Start is called before the first frame update
    public void Knockback(Vector2 KnockbackVelo)
    {
        rigid.velocity = Vector2.zero; // 현재 속도를 초기화
        rigid.AddForce(KnockbackVelo.normalized * 10 * knockbackForce, ForceMode2D.Impulse); // 넉백 방향으로 힘을 가함

        // 넉백 상태 설정 및 타이머 초기화
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }
}
