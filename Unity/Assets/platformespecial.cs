using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformespecial : MonoBehaviour
{

    public Transform posA, posB;
    public int Speed;
    Vector2 targetPos;
    private bool AreBothPlayerOn = false;
    // Start is called before the first frame update
    void Start()
    {
        targetPos = posB.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (AreBothPlayerOn)
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") || collision.CompareTag("PlayberB"))
        {
            collision.transform.SetParent(this.transform);
        }
        if (collision.CompareTag("PlayerA") && collision.CompareTag("PlayberB"))
        {
            AreBothPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") || collision.CompareTag("PlayberB"))
        {
            collision.transform.SetParent(null);
        }
        AreBothPlayerOn = false;
    }

}
