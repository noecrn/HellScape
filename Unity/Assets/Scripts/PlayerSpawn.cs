using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Animator animator;
    private void Awake()
    {
        animator.SetTrigger("FadeIn");
        GameObject.FindGameObjectWithTag("PlayerA").transform.position = transform.position;
        GameObject.FindGameObjectWithTag("PlayerB").transform.position = transform.position;
    }
}
