
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "PlayerA")
        {
            PlayerHealth.instance.TakeDamage(100, "PlayerA");
            Debug.Log("ok");
        }  
        
        if (col.gameObject.tag == "PlayerB")
        {
            PlayerHealth.instance.TakeDamage(100, "PlayerB");
        } 
        
    }


}
