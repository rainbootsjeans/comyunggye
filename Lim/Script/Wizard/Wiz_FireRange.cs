using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiz_FireRange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.name != "ItemRange")
        {
            bool isPlayerKnockback = collision.GetComponent<PlayerMove>().ReturnKnockback();
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
}
