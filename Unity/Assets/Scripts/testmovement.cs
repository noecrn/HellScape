using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmovement : MonoBehaviour
{

    private float horizontal;
    public int speed;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            horizontal = -1f;
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            horizontal = 1f;
        }
        else
        {
            horizontal = 0;
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }
}
