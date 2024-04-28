using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgePlayerA : MonoBehaviour
{
    public float dodgeTime = 1f;
    public float nextdodge = 0f;

    [SerializeField] public float animTime = 0.3f;
    public float nextAnim = 0f;
    
    public static DodgePlayerA instance;
    
    public void Update()
    {
        if (!MovePlayerA.instance.dodgeAnim && Input.GetKeyDown(KeyCode.J))
        {
            MovePlayerA.instance.isDoging = true;
            nextdodge = Time.time + dodgeTime;

            //MovePlayerA.instance.dodgeAnim = true;
            nextAnim = Time.time + animTime;
        }

        if (Time.time > nextdodge) MovePlayerA.instance.isDoging = false;
        if (Time.time > nextAnim)
        {
            //MovePlayerA.instance.animator.SetBool("Dodge", MovePlayerA.instance.dodgeAnim);
            MovePlayerA.instance.dodgeAnim = false;
            MovePlayerA.instance.b = true;
        }
        
        //Debug.Log("d = "+PlayerHealth.instance.isDoging);
        
    }
}
