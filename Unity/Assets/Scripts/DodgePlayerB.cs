using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgePlayerB : MonoBehaviour
{
    public float dodgeTime = 1f;
    public float nextdodge = 0f;

    [SerializeField] public float animTime = 0.3f;
    public float nextAnim = 0f;

    public static DodgePlayerB instance;

    public void Update()
    {
        if (!MovePlayerB.instance.dodgeAnim && Input.GetKeyDown(KeyCode.J))
        {
            MovePlayerB.instance.isDodging = true;
            nextdodge = Time.time + dodgeTime;

            //MovePlayerB.instance.dodgeAnim = true;
            nextAnim = Time.time + animTime;
        }

        if (Time.time > nextdodge) MovePlayerB.instance.isDodging = false;
        if (Time.time > nextAnim)
        {
            //B.instance.animator.SetBool("Dodge", MovePlayerB.instance.dodgeAnim);
            MovePlayerB.instance.dodgeAnim = false;
            MovePlayerB.instance.b = true;
        }

        //Debug.Log("d = "+PlayerHealth.instance.isDoging);

    }
}
