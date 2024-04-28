using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    private Ladder ladder;
    private CompositeCollider2D collider;

    private void Awake()
    {
        collider = FindObjectOfType<CompositeCollider2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") && ladder.isClimbing)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") && ladder.isClimbing)
        {
            collider.isTrigger = false;
        }
    }
}
