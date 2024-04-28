using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviourPunCallbacks, IPunObservable
{
    private float speed = 2;
    private float stoppingDistance = 7f;
    private float agroRange = 20;

    private Transform targetA;
    private Transform targetB;
    private string tagPlayerFollowed;

    public Transform GroundCheck;
    public LayerMask GroundLayer;

    private bool attack;
    private bool idle = true;
    private bool dead;
    private bool hit;
    private float hitTime;

    public Animator animator;
    private bool facingLeft;
    private bool isGrounded;
    
    public static Boss instance;

    private Transform enemyFollowed;
    private float damageCount;

    public int EnemyHealth;
    private int timeDead;

    public AudioClip sound;

    void Start()
    {
        EnemyHealth = 100;
    }

    private void Awake()
    {
        instance = this;
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

        if (targetA is null && targetB is null)
        {
            // Les deux joueurs ne sont pas présents sur la scène, on ne fait rien
            return;
        }

        if (targetA && targetB && !isGrounded && !dead && !hit)
        {
            FollowClosestPlayer();
        }
        else if (targetA && !targetB && !isGrounded && !dead && !hit)
        {
            FollowSinglePlayer(targetA);
            tagPlayerFollowed = "PlayerA";
        }

        if (!isGrounded == false)
        {
            idle = true;
        }

        UpdateFacingDirection();
        UpdateHitAnimation();
        UpdateAnimator();
        HealthCheck();
    }

    private void HealthCheck()
    {
        if (photonView.IsMine && EnemyHealth <= 0)
        {
            dead = true;
            photonView.RPC("Die", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void Die()
    {
        AudioManager.instance.PlayClipAt(sound, transform.position);
        Destroy(gameObject);
    }

    private void FollowClosestPlayer()
    {
        float distPlayerA = Vector2.Distance(targetA.position, transform.position);
        float distPlayerB = Vector2.Distance(targetB.position, transform.position);
        if (distPlayerA < distPlayerB && distPlayerA > stoppingDistance && distPlayerA < agroRange)
        {
            FollowPlayer(targetA);
            tagPlayerFollowed = "PlayerA";
        }
        else if (distPlayerB < distPlayerA && distPlayerB > stoppingDistance && distPlayerB < agroRange)
        {
            FollowPlayer(targetB);
            tagPlayerFollowed = "PlayerB";
        }
        else
        {
            HandleIdleAndAttack(distPlayerA, distPlayerB);
        }
    }

    private void FollowSinglePlayer(Transform player)
    {
        float distPlayerA = Vector2.Distance(player.position, transform.position);
        if (distPlayerA > stoppingDistance && distPlayerA < agroRange)
        {
            FollowPlayer(player);
        }
        else
        {
            HandleIdleAndAttack(distPlayerA, float.MaxValue);
        }
    }

    private void FollowPlayer(Transform player)
    {
        idle = false;
        attack = false;
        enemyFollowed = player;
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    private void HandleIdleAndAttack(float distPlayerA, float distPlayerB)
    {
        if (distPlayerA < stoppingDistance || distPlayerB < stoppingDistance)
        {
            Attack();
        }
        else if (distPlayerA > agroRange || distPlayerB > agroRange)
        {
            Idle();
        }
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            EnemyHealth -= damage;
            hit = true;
        }
    }
    private void Idle()
    {
        idle = true;
        attack = false;
    }

    private void Attack()
    {
        if (photonView.IsMine)
        {
            attack = true;
            idle = false;
            damageCount += Time.deltaTime;
            if (damageCount >= 1f)
            {
                // photonView.RPC("DealDamage", RpcTarget.All, damage);
                PlayerHealth.instance.TakeDamage(50, tagPlayerFollowed);
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

    private void UpdateFacingDirection()
    {
        Vector3 scale = transform.localScale;

        if (transform.position.x < enemyFollowed.position.x)
        {
            if (!facingLeft && !animator.GetBool("Attack"))
            {
                facingLeft = true;
                scale.x = -Mathf.Abs(scale.x);
            }
        }
        else
        {
            if (facingLeft && !animator.GetBool("Attack"))
            {
                facingLeft = false;
                scale.x = Mathf.Abs(scale.x);
            }
        }
        transform.localScale = scale;
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
        animator.SetBool("Idle", idle);
        animator.SetBool("Dead", dead);
        animator.SetBool("Hit", hit);

        // Appel de la fonction RPC pour synchroniser les animations des joueurs
        photonView.RPC("UpdateAnimatorRPC", RpcTarget.Others, attack, idle, dead, hit);
    }

    [PunRPC]
    private void UpdateAnimatorRPC(bool attack, bool idle, bool dead, bool hit)
    {
        animator.SetBool("Attack", attack);
        animator.SetBool("Idle", idle);
        animator.SetBool("Dead", dead);
        animator.SetBool("Hit", hit);
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(EnemyHealth);
            stream.SendNext(hit);
            stream.SendNext(dead);
            stream.SendNext(attack);
            stream.SendNext(idle);
            stream.SendNext(damageCount);
            stream.SendNext(animator.GetBool("Attack"));
            stream.SendNext(animator.GetBool("Idle"));
            stream.SendNext(animator.GetBool("Dead"));
            stream.SendNext(animator.GetBool("Hit"));
        }
        else
        {
            EnemyHealth = (int)stream.ReceiveNext();
            hit = (bool)stream.ReceiveNext();
            dead = (bool)stream.ReceiveNext();
            attack = (bool)stream.ReceiveNext();
            idle = (bool)stream.ReceiveNext();
            damageCount = (int)stream.ReceiveNext();
            animator.SetBool("Attack", (bool)stream.ReceiveNext());
            animator.SetBool("Idle", (bool)stream.ReceiveNext());
            animator.SetBool("Dead", (bool)stream.ReceiveNext());
            animator.SetBool("Hit", (bool)stream.ReceiveNext());
        }
    }
}