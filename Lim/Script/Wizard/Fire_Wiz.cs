using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_Wiz : MonoBehaviour
{
    public enum Wiz_State
    {
        Idle,
        Move,
        Attack,
        Death
    }

    public Wiz_State Cur_State = Wiz_State.Idle;
    Wiz_State prev_State;

    public int WIz_HP = 5;
    int cur_HP;

    SpriteRenderer WSR;
    Animator WAnim;

    [SerializeField] GameObject TPeffect;//텔레포트 사용시 드러남
    [SerializeField] Transform Player;//행동 타겟팅 대상
    [SerializeField] GameObject SpawnPrefab;//화염비 패턴 소환 대상
    [SerializeField] GameObject FireRange;
    [SerializeField] GameObject DamageRange;//피해 판정
    /*float FR_basic_Offset = 3;
    float x_MaxDistance= 10f;
    float y_MaxDistance = 5f;*/
    
    public bool IsHit = false;
    public bool IsDie = false;

    int AttackChance = 0;
    int MoveChance = 0;
    int FireBallCount = 3;
    // Start is called before the first frame update
    void Start()
    {
        WSR = GetComponent<SpriteRenderer>();
        WAnim = GetComponent<Animator>();
        TPeffect.SetActive(false) ;
        cur_HP = WIz_HP;
        FireRange.SetActive(false);
        Cur_State = Wiz_State.Idle;
        StartCoroutine("StateMachine");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsDie)
        {
            Face_Player();
        }
    }
    void Face_Player()//화염방사 패턴/사망 제외 모든 상태에서 사용
    {

        if (Player.position.x < this.transform.position.x)
        {
            WSR.flipX = true;
            FireRange.transform.rotation = new Quaternion(0, 0, 180, 0);

        }
        else if (Player.position.x > this.transform.position.x)
        {
            WSR.flipX = false;
            FireRange.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }



    IEnumerator StateMachine()
    {
        while (cur_HP > 0)
        {
            //Debug.Log(Cur_State);
            yield return StartCoroutine(Cur_State.ToString());
        }//반복 해제=체력이 0이하
        Cur_State = Wiz_State.Death;
        IsDie = true;
        yield return StartCoroutine("Death");

    }


    IEnumerator Idle()//무행동,다음 행동을 결정하는 랜덤값을 불러옴.
    {
        yield return new WaitForSeconds(1f);
        int Next = Random.Range(0,101);

        if (Next <= 50 - MoveChance) { //&& transform.position.y < Player.transform.position.y + 7f
            Cur_State = Wiz_State.Move;//0->Move
            MoveChance =+ 5;//이동패턴이 많으면 공격확률 증가
        }
        else
        {
            Cur_State = Wiz_State.Attack;//0->Attack
            MoveChance = 0;//공격패턴 발생시 확률 리셋
        }
        prev_State = Wiz_State.Idle;
        yield return 0;
    }
    IEnumerator Attack()//Idle 혹은 Move 후 실행, 공격패턴 2가지 중 하나를 선택해 실행. 사용하지 않은 패턴은 다음 Attack에서 확률 증가
    {
        //AttackChance 값이 높을수록 2번째 공격의 확률이 증가.
        int Next = Random.Range(1, 101);
        if (Next <= 50-AttackChance) {
            yield return StartCoroutine("Fire_thrower");//패턴 1:화염방사
            AttackChance += 5;
        }
        else
        {
            yield return StartCoroutine("Fire_Rain");//패턴 2:화염비
            AttackChance -= 5;
        }
        yield return new WaitForSeconds(1f);
        if (prev_State == Wiz_State.Idle)//기본->공격이라면 이동 실행
        {
            Cur_State = Wiz_State.Move;
            prev_State = Wiz_State.Attack;
        }
        else
        {
            Cur_State = Wiz_State.Idle;
        }

        yield return 0;
    }
    IEnumerator Move()//Idle 혹은 Attack 후 실행, 
    {
        //Move행동 
        if (transform.position.y < Player.transform.position.y + 8f)
        {
            yield return StartCoroutine("Teleport");
        }
        yield return new WaitForSeconds(1f);
        /*if (prev_State == Wiz_State.Idle)//기본->이동이라면 공격 실행
        {
            Cur_State = Wiz_State.Attack;
            prev_State = Wiz_State.Move;
        }
        else
        {
            Cur_State = Wiz_State.Idle;
        }*/
        Cur_State = Wiz_State.Idle;//화염방사 2연타 같은 부조리 방지용 (이동->공격->기본->공격)

        yield return 0;
    }
    IEnumerator Death()//체력이 0이 될시 실행
    {
        WAnim.SetBool("IsDie", true);
        WAnim.SetTrigger("Death");
        
        yield return new WaitForSeconds(1f);
        TPeffect.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        TPeffect.SetActive(true);
        yield return StartCoroutine("TPEffection1");
        WSR.enabled = false;
        yield return StartCoroutine("TPEffection2");
        Destroy(this.gameObject);
    }
    IEnumerator Teleport()
    {
        TPeffect.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        WAnim.SetBool("IsAttack", false);
        FireRange.SetActive(false);
        TPeffect.SetActive(true);

        yield return StartCoroutine("TPEffection1");
        WSR.enabled = false;//마법사 이미지가 사라지기 전까진 공격으로 캔슬 가능
        DamageRange.SetActive(false);
        yield return StartCoroutine("TPEffection2");
        while (transform.position.y <= Player.transform.position.y + 8f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.15f, 0);
            yield return new WaitForSeconds(0.03f);
        }
        yield return StartCoroutine("TPEffection1");
        WSR.enabled = true;

        DamageRange.SetActive(true);
        TPeffect.SetActive(false);
        yield return 0;
    }
    IEnumerator TPEffection1()
    {
        while (TPeffect.GetComponent<Transform>().localScale.x < 3f)
        {
            TPeffect.transform.localScale = new Vector3(TPeffect.transform.localScale.x + 0.1f
                                                            , TPeffect.transform.localScale.y + 0.1f
                                                            , 0);
            yield return new WaitForSeconds(0.03f);
        }
    }
    IEnumerator TPEffection2()
    {
        while (TPeffect.GetComponent<Transform>().localScale.x > 0.5f)
        {
            TPeffect.transform.localScale = new Vector3(TPeffect.transform.localScale.x - 0.2f
                                                            , TPeffect.transform.localScale.y - 0.2f
                                                            , 0);
            yield return new WaitForSeconds(0.03f);
        }
    }
    public void Hit()
    {
        WSR.enabled = true;
        DamageRange.SetActive(true);
        TPeffect.SetActive(false);
        TPeffect.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        cur_HP -= 1;
        IsHit = true;
        WAnim.SetBool("IsAttack", false);
        FireRange.SetActive(false);
        WAnim.SetTrigger("Hit");
        StopAllCoroutines();
        StartCoroutine("AfterHit");
    }
    IEnumerator AfterHit()
    {
        yield return new WaitForSeconds(1f);
        Cur_State = Wiz_State.Idle;
        IsHit = false;
        StartCoroutine("StateMachine");
        
    }

    IEnumerator Fire_thrower()//화염방사형 패턴,방향 지정후 그방향으로 직진(코루틴 Attack_Move)
    {
        WAnim.SetBool("IsAttack",true);
        FireRange.SetActive(true);
        yield return StartCoroutine("Attack_Move");
    }

    IEnumerator Attack_Move()
    {
        Vector3 direction;
        float time = 0.0f;

        while (time < 1.0f)
        {
            direction = Player.position - transform.position;
            time += Time.deltaTime / 5f;

            transform.position += direction.normalized * 2.5f * Time.deltaTime;
            yield return null;
        }
        WAnim.SetBool("IsAttack", false);
        FireRange.SetActive(false);
    }

    IEnumerator Fire_Rain()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < FireBallCount; i++)//화염생성 지역(0~2)
        {
            Instantiate(SpawnPrefab, Player.position,Quaternion.identity);//경고범위 생성->이후 Start 함수에서 알아서 실행
            yield return new WaitForSeconds(1f) ;
        }
        yield return 0;
    }
}
