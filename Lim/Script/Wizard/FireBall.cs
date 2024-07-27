using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] GameObject Fire_Alert;
    IEnumerator StartFalling()
    {
        yield return new WaitForSeconds(4f);
        Destroy(Fire_Alert);
    }
    void Start()
    {
        StartCoroutine("StartFalling");
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
