using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "PlayerA" && gameObject.tag == "SpikeWhite")
        {
            PlayerHealth.instance.TakeDamage(1, "PlayerA");
        }  
        
        if (col.gameObject.tag == "PlayerB" && gameObject.tag == "SpikeRed")
        {
            PlayerHealth.instance.TakeDamage(1, "PlayerB");
        } 
    }
}

