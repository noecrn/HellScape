using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Ladder : MonoBehaviourPunCallbacks, IPunObservable
{
    private float vertical;
    private float speed = 4f;
    public bool isLadder;
    public bool isClimbing;

    public Rigidbody2D rb;

    public Animator animator;

    private CompositeCollider2D[] collider;

    private Text interactUI;

    [SerializeField] private Transform groundCheck;


    private void Awake()
{
    animator = GetComponent<Animator>();
    interactUI = GameObject.FindGameObjectWithTag("InteractUI").GetComponent<Text>();
    collider = FindObjectsOfType<CompositeCollider2D>();
}

    private void Update()
    {
        if (!photonView.IsMine) return; // On ne contrôle pas cet objet

        if (!isLadder)
        {
            enabled = false; // désactiver le script Ladder si IsLadder est faux
            return;
        }

        if (isLadder)
        { 
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                isClimbing = true;
                rb.gravityScale = 0f;
                for(int i = 0; i < collider.Length; i++)
                {
                    collider[i].isTrigger = true;
                }
                photonView.RPC("SetClimbing", RpcTarget.Others, true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                isClimbing = false;
                rb.gravityScale = 1f;
                for (int i = 0; i < collider.Length; i++)
                {
                    collider[i].isTrigger = false;
                }
                photonView.RPC("SetClimbing", RpcTarget.Others, false);
            }

            if (isClimbing)
            {
                vertical = Input.GetAxis("Vertical");
                rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                animator.SetBool("IsClimbing", true);
                photonView.RPC("SetClimbingAnimation", RpcTarget.Others, true);
            }
            else
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                animator.SetBool("IsClimbing", false);
                photonView.RPC("SetClimbingAnimation", RpcTarget.Others, false);
            }
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None;
            animator.SetBool("IsClimbing", false);
        }

        if (!isClimbing && !isLadder)
        {
            rb.gravityScale = 1f;
        }
        else if (isLadder && !isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }

        if (isLadder && isClimbing && Mathf.Approximately(vertical, 0))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else if (!isLadder)
        {
            rb.constraints = RigidbodyConstraints2D.None;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    [PunRPC]
    public void SetClimbingAnimation(bool isClimbing)
    {
        animator.SetBool("IsClimbing", isClimbing);
    }


    private void OnTriggerStay2D(Collider2D col)
{
    if (col.CompareTag("Echelle"))
        {
        isLadder = true;
        if (photonView.IsMine)
        {
            photonView.RPC("SetLadder", RpcTarget.Others, true);
            interactUI.enabled = true;
        }

        if (!enabled)
            {
                enabled = true;
            }
        }
}

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Echelle"))
        {
            isLadder = false;
            isClimbing = false;
            animator.SetBool("IsClimbing", false);
            for (int i = 0; i < collider.Length; i++)
            {
                collider[i].isTrigger = false;
            }

            if (photonView.IsMine)
            {
                photonView.RPC("SetLadder", RpcTarget.Others, false);
                photonView.RPC("SetClimbing", RpcTarget.Others, false);
                interactUI.enabled = false;
            }

            if (!isClimbing && !isLadder)
            {
                rb.gravityScale = 1f;
            }
        }
    }


    [PunRPC]
void SetLadder(bool value)
{
    isLadder = value;
    if (!isLadder)
    {
        isClimbing = false;
        animator.SetBool("IsClimbing", false);
            for (int i = 0; i < collider.Length; i++)
            {
                collider[i].isTrigger = false;
            }
        }
}

[PunRPC]
public void SetClimbing(bool value)
{
    isClimbing = value;
    rb.gravityScale = value ? 0f : 1f;
        for (int i = 0; i < collider.Length; i++)
        {
            collider[i].isTrigger = value;
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(vertical);
            stream.SendNext(speed);
            stream.SendNext(isLadder);
            stream.SendNext(isClimbing);
        }
        else
        {
            vertical = (float)stream.ReceiveNext();
            speed = (float)stream.ReceiveNext();
            isLadder = (bool)stream.ReceiveNext();
            isClimbing = (bool)stream.ReceiveNext();
            animator.SetBool("IsClimbing", isClimbing);
        }
    }

    public void OnPhotonDeserializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(vertical);
            stream.SendNext(speed);
            stream.SendNext(isLadder);
            stream.SendNext(isClimbing);
        }
        else
        {
            vertical = (float)stream.ReceiveNext();
            speed = (float)stream.ReceiveNext();
            isLadder = (bool)stream.ReceiveNext();
            isClimbing = (bool)stream.ReceiveNext();
            animator.SetBool("IsClimbing", isClimbing);
        }
    }

}
