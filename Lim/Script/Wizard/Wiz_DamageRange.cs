using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiz_DamageRange : MonoBehaviour
{
    [SerializeField] Fire_Wiz FW;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "P_Attack" && !FW.IsHit && !FW.IsDie)
        {
            FW.Hit();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
