using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 나 볼려고 적음 :
// 플레이어 상호작용 관련 로직 및 애니메이션 제어

public class PlayerInteraction : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        searchObject();
    }

    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;
    void searchObject()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        float diff = 100;

        if (targets.GetLength(0)==0)
        {
            nearestTarget = null;
        }

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if(curDiff < diff)
            {
                diff = curDiff;
                nearestTarget = target.transform;
            }
        }
    }
}
