using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EnemyB : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed = 4;
    public int damage = 10;
    
    private float stoppingDistance = 1.5f;

    private Transform targetA;
    private Transform targetB;
    private string tagPlayerFollowed;

    public Transform GroundCheck;
    public LayerMask GroundLayer;
    public Rigidbody2D rb;

    private bool attack;
    private bool dead;
    private bool hit;
    private float hitTime;

    public Animator animator;
    private bool facingLeft;
    private bool isGrounded;

    private float damageCount;
    private float deadCount;

    public int EnemyHealth;
    private int timeDead;

    public AudioClip sound;

    void Start()
    {
        EnemyHealth = 60;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, 0.5f, GroundLayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("PlayerA") is not null)
        {
            targetA = GameObject.FindGameObjectWithTag("PlayerA").GetComponent<Transform>();
        }

        if (GameObject.FindGameObjectWithTag("PlayerB") is not null)
        {
            targetB = GameObject.FindGameObjectWithTag("PlayerB").GetComponent<Transform>();
        }

        if (isGrounded && !attack)
        {
            Vector3 targetVelocity = new Vector2(-speed, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref targetVelocity, .05f);
        }

        if (isGrounded == false)
        {
            Flip();
        }
        
        UpdateHitAnimation();
        UpdateAnimator();
        HealthCheck();
        
        if (targetA && targetB && isGrounded && !dead && !hit)
        {
            float distPlayerA = Vector2.Distance(targetA.position, transform.position);
            float distPlayerB = Vector2.Distance(targetB.position, transform.position);
            
            HandleIdleAndAttack(distPlayerA, distPlayerB);
        }
        else if (targetA && !targetB && isGrounded && !dead && !hit)
        {
            tagPlayerFollowed = "PlayerA";
            float distPlayerA = Vector2.Distance(targetA.position, transform.position);
            if (distPlayerA < stoppingDistance)
            {
                Attack();
            }
            else
            {
                attack = false;
            }
        }
    }

    private void HealthCheck()
    {
        if (photonView.IsMine && EnemyHealth <= 0)
        {
            dead = true;
            deadCount += Time.deltaTime;
            if (deadCount >= 1f)
            {
                photonView.RPC("Die", RpcTarget.AllBuffered);
                deadCount = 0f;
            }
        }
    }

    [PunRPC]
    private void Die()
    {
        AudioManager.instance.PlayClipAt(sound, transform.position);
        PhotonNetwork.Destroy(gameObject);
    }

    private void HandleIdleAndAttack(float distPlayerA, float distPlayerB)
    {
        if (distPlayerA < stoppingDistance)
        {
            Attack();
            tagPlayerFollowed = "PlayerA";
        }

        if (distPlayerB < stoppingDistance)
        {
            Attack();
            tagPlayerFollowed = "PlayerB";
        }
        
        attack = false;
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            EnemyHealth -= damage;
            hit = true;
        }
    }

    private void Attack()
    {
        if (photonView.IsMine)
        {
            attack = true;
            damageCount += Time.deltaTime;
            if (damageCount >= 0.7f)
            {
                photonView.RPC("DealDamage", RpcTarget.All, damage);
                PlayerHealth.instance.TakeDamage(damage, tagPlayerFollowed);
                damageCount = 0f;
            }   
        }
    }

    [PunRPC]
    private void DealDamage(int damage)
    {
        float distPlayerA = Vector2.Distance(targetA.position, transform.position);
        float distPlayerB = Vector2.Distance(targetB.position, transform.position);

        if (distPlayerA < distPlayerB)
        {
            PlayerHealth.instance.TakeDamage(damage, "PlayerA");
        }
        else
        {
            PlayerHealth.instance.TakeDamage(damage, "PlayerB");
        }
    }


    private void UpdateHitAnimation()
    {
        if (hit)
        {
            hitTime += Time.deltaTime;
            if (hitTime >= 0.5f)
            {
                hit = false;
                hitTime = 0f;
            }
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("Attack", attack);
        animator.SetBool("Dead", dead);
        animator.SetBool("Hit", hit);

        // Appel de la fonction RPC pour synchroniser les animations des joueurs
        photonView.RPC("UpdateAnimatorRPC", RpcTarget.Others, attack, dead, hit);
    }

    [PunRPC]
    private void UpdateAnimatorRPC(bool attack, bool idle, bool dead, bool hit)
    {
        animator.SetBool("Attack", attack);
        animator.SetBool("Dead", dead);
        animator.SetBool("Hit", hit);
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        speed *= -1;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(EnemyHealth);
            stream.SendNext(hit);
            stream.SendNext(dead);
            stream.SendNext(attack);
            stream.SendNext(damageCount);
            stream.SendNext(animator.GetBool("Attack"));
            stream.SendNext(animator.GetBool("Dead"));
            stream.SendNext(animator.GetBool("Hit"));
        }
        else
        {
            EnemyHealth = (int)stream.ReceiveNext();
            hit = (bool)stream.ReceiveNext();
            dead = (bool)stream.ReceiveNext();
            attack = (bool)stream.ReceiveNext();
            damageCount = (int)stream.ReceiveNext();
            animator.SetBool("Attack", (bool)stream.ReceiveNext());
            animator.SetBool("Dead", (bool)stream.ReceiveNext());
            animator.SetBool("Hit", (bool)stream.ReceiveNext());
        }
    }
}