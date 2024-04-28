using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownB : MonoBehaviour
{
    [SerializeField] float cooldownTime = 17f;
    private float next = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > next)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                next = Time.time + cooldownTime;
                MovePlayerB.instance.Attack();
            }
        }
    }
}
