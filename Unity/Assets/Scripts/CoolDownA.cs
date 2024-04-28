using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownA : MonoBehaviour
{
    [SerializeField] float cooldownTime = 0.5f;
    private float next = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > next)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                MovePlayerA.instance.Attack();
                next = Time.time + cooldownTime;
            }
        }
    }
}
