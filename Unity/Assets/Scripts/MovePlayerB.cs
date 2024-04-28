using System;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using UnityEngine.Timeline;

public class MovePlayerB : MonoBehaviourPunCallbacks, IPunObservable
{
    // Variables pour le déplacement
    public float speed = 5f;
    public float jumpForce = 11f;
    public LayerMask groundLayer;

    // Composants
    private Rigidbody2D rb;
    public Animator animator;

    // Variables pour la détection de collision avec le sol
    public bool isGrounded;
    public bool isJumping;

    public bool isDodging;
    public bool dodgeAnim;
    public bool b = true;

    private int jumpLeft;
    private int maxJump = 2;

    public Transform attackPoint;
    public float attackRange = 1.58f;
    public LayerMask enemyLayers;


    // Référence au GameObject "GroundCheck" qui est situé directement en dessous des pieds du joueur
    public Transform groundCheck;

    public static MovePlayerB instance;

    private float speedValue;

    private bool a;

    private void Awake()
    {
        // Récupération des composants Rigidbody2D et Animator
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        instance = this;
        jumpLeft = maxJump;

    }

    private void FixedUpdate()
    {
        // Vérification si le joueur est le joueur local
        if (!photonView.IsMine) return;

        // Déplacement horizontal
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (!isJumping)
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        //if (!isJumping && !isGrounded && moveInput != 0)
        //{
        //    isJumping = true;
        //    animator.SetBool("isJumping", true);
        //    speedValue = 0;
        //}

        //if (!isGrounded)
        //{
        //    speedValue = 0f;
        //}

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, groundLayer);
        bool canMoveHorizontally = true;
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Check"))
            {
                canMoveHorizontally = false;
                rb.velocity = new Vector2(0f, rb.velocity.y);
                break;
            }
        }

        if (canMoveHorizontally && !isJumping)
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }

        // Animation de déplacement
        speedValue = Mathf.Abs(rb.velocity.x);


        // Animation de déplacement
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Détection de la direction du mouvement horizontal
        Flip(moveInput);

        // Vérification si le joueur touche le sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);

        // Envoi des informations de mouvement aux autres joueurs sur le réseau
        photonView.RPC("UpdateMovement", RpcTarget.Others, rb.velocity, speedValue);

        // Si le joueur est en train de tomber, ajuster sa vitesse verticale pour éviter qu'il ne perde sa vitesse horizontale lorsqu'il touche le sol
        if (!isGrounded && rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime;
        }
    }


    private void Update()
    {
        // Vérification si le joueur est le joueur local
        if (!photonView.IsMine) return;

        if (dodgeAnim && b)
        {
            b = false;
            photonView.RPC("RPC_Dodge", RpcTarget.All);
        }

        if (isGrounded)
        {
            jumpLeft = maxJump;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("isJumping", true);
            if (jumpLeft == 2)
            {
                //rb.mass = 1f;
                rb.AddForce(new Vector2(0f, 4), ForceMode2D.Impulse);
                jumpLeft = 1;
            }
            if (jumpLeft == 1)
            {
                //rb.velocity=new Vector2(rb.velocity.x, 0);                
                //rb.mass = 1f;
                rb.AddForce(new Vector2(0f, 2.1f), ForceMode2D.Impulse);
                jumpLeft = 0;
                //Debug.Log(jumpLeft);  
            }
            //Envoi d'un appel de fonction RPC pour synchroniser le saut avec les autres joueurs sur le réseau
            photonView.RPC("Jump", RpcTarget.Others);
        }
        animator.SetBool("isJumping", false);

    }

    public void Attack()
    {
        if (!photonView.IsMine)
            return;

        animator.SetTrigger("Attack");
        photonView.RPC("TriggerAttackRPC", RpcTarget.Others);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (var enemy in hitEnemies)
        {
            if (enemy.gameObject.TryGetComponent<EnemyA>(out EnemyA enemyAHealth))
            {
                enemyAHealth.TakeDamage(15);
            }
            if (enemy.gameObject.TryGetComponent<EnemyB>(out EnemyB enemyBHealth))
            {
                enemyBHealth.TakeDamage(15);
            }
            if (enemy.gameObject.TryGetComponent<Boss>(out Boss bossHealth))
            {
                bossHealth.TakeDamage(15);
            }
            if (enemy.gameObject.TryGetComponent<EnemyB>(out EnemyB enemyHealth1))
            {
                enemyHealth1.TakeDamage(15);
            }
        }

        photonView.RPC("AttackRPC", RpcTarget.Others);
    }

    [PunRPC]
    public void TriggerAttackRPC()
    {
        animator.SetTrigger("Attack");
    }


    [PunRPC]
    public void AttackRPC()
    {
        Attack();
    }

    private void Flip(float moveInput)
    {
        // Récupération de l'échelle actuelle du transform
        Vector3 scale = transform.localScale;

        // Inversion de la direction de l'échelle horizontale en fonction de la direction du mouvement, si le personnage est au sol
        if (moveInput > 0 && isGrounded)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (moveInput < 0 && isGrounded)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        // Si le personnage est en l'air, on conserve sa vitesse horizontale
        else if (!isGrounded)
        {
            if (rb.velocity.x > 0)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else if (rb.velocity.x < 0)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
        }

        // Mise à jour de l'échelle du transform
        transform.localScale = scale;
        FlipRPC(moveInput);
    }

    [PunRPC]
    private void FlipRPC(float moveInput)
    {
        // Récupération de l'échelle actuelle du transform
        Vector3 scale = transform.localScale;

        // Inversion de la direction de l'échelle horizontale en fonction de la direction du mouvement, si le personnage est au sol
        if (moveInput > 0 && isGrounded)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (moveInput < 0 && isGrounded)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else if (!isGrounded)
        {
            if (rb.velocity.x > 0)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else if (rb.velocity.x < 0)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
        }

        // Affectation de la nouvelle échelle au transform
        transform.localScale = scale;

        // Envoi de la mise à jour de l'échelle aux autres joueurs sur le réseau
        photonView.RPC("UpdateFlip", RpcTarget.Others, scale);
    }

    private void OnDrawGizmosSelected() // dessine la range autour du attackPoint
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    [PunRPC]
    private void UpdateFlip(Vector3 scale)
    {
        // Mise à jour de l'échelle pour les autres joueurs sur le réseau
        transform.localScale = scale;
    }

    [PunRPC]
    private void RPC_Dodge()
    {
        MovePlayerB.instance.animator.SetTrigger("Dodge");
    }

    [PunRPC]
    private void UpdateMovement(Vector2 velocity, float speed)
    {
        // Mise à jour de la vitesse et de l'état de l'animation pour les joueurs sur le réseau
        rb.velocity = velocity;
        animator.SetFloat("Speed", speed);
    }

    [PunRPC]
    private void Jump()
    {
        // Appliquer une force de saut aux joueurs sur le réseau
        animator.SetTrigger("isJumping");
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    [PunRPC]
    private void UpdateJumpingRPC(bool jumping)
    {
        animator.SetBool("isJumping", jumping);
    }

    // Fonction appelée automatiquement pour synchroniser les mouvements des joueurs sur le réseau
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envoi des informations de mouvement aux autres joueurs sur le réseau
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));
            stream.SendNext(animator.GetBool("Attack"));
            stream.SendNext(animator.GetBool("isJumping"));
            stream.SendNext(animator.GetBool("Dodge"));
        }
        else
        {
            // Réception des informations de mouvement des autres joueurs sur le réseau
            transform.position = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("Attack", (bool)stream.ReceiveNext());
            animator.SetBool("isJumping", (bool)stream.ReceiveNext());
            animator.SetBool("Dodge", (bool)stream.ReceiveNext());
        }
    }
}
