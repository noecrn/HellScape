using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Timeline;

public class MovePlayerA : MonoBehaviourPunCallbacks, IPunObservable
{
    // Variables pour le déplacement
    public float speed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;

    // Composants
    private Rigidbody2D rb;
    public Animator animator;

    // Variables pour la détection de collision avec le sol
    public bool isGrounded;
    public bool isJumping;

    public Transform attackPoint;
    public float attackRange = 1.58f;
    public LayerMask enemyLayers;

    public bool isDoging;
    public bool dodgeAnim;
    public bool b = true;

    private float speedValue;


    // private int jumpLeft;
    // private int maxJump = 2;


    // Référence au GameObject "GroundCheck" qui est situé directement en dessous des pieds du joueur
    [SerializeField] private Transform groundCheck;

    public static MovePlayerA instance;

    private void Awake()
    {
        // Récupération des composants Rigidbody2D et Animator
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        instance = this;
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

        speedValue = Mathf.Abs(rb.velocity.x);


        // Animation de déplacement
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Détection de la direction du mouvement horizontal
        Flip(moveInput);

        // Vérification si le joueur touche le sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

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
            MovePlayerA.instance.animator.SetTrigger("DodgeTr");
        }

        //MovePlayerA.instance.animator.SetBool("Dodge", MovePlayerA.instance.dodgeAnim);

        // Saut
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
            animator.SetBool("isJumping", true);
            // Envoi d'un appel de fonction RPC pour synchroniser le saut avec les autres joueurs sur le réseau
            photonView.RPC("Jump", RpcTarget.Others);
        }

        // Prevent jumping while in the air
        if (!isGrounded && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Désactiver le flag de saut si le joueur est en train de tomber
        if (isJumping && rb.velocity.y <= 0)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
            photonView.RPC("UpdateJumpingRPC", RpcTarget.Others, false);
        }
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

    private void OnDrawGizmosSelected() // dessine la range autour du attackPoint
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void Flip(float moveInput)
    {
        // Récupération de la rotation actuelle du transform
        Vector3 rotation = transform.rotation.eulerAngles;

        // Rotation sur l'axe x en fonction de la direction du mouvement, si le personnage est au sol
        if (moveInput > 0 && isGrounded)
        {
            rotation.y = 0f;  // Remettre la rotation sur l'axe y à 0
        }
        else if (moveInput < 0 && isGrounded)
        {
            rotation.y = 180f;  // Effectuer une rotation de 180 degrés sur l'axe y
        }

        // Appliquer la nouvelle rotation au transform
        transform.rotation = Quaternion.Euler(rotation);

        // Appel de la fonction RPC pour synchroniser la rotation sur tous les clients
        photonView.RPC("FlipRPC", RpcTarget.All, moveInput);
    }


    [PunRPC]
    private void FlipRPC(float moveInput)
    {
        // Récupération de la rotation actuelle du transform
        Vector3 rotation = transform.rotation.eulerAngles;

        // Rotation sur l'axe x en fonction de la direction du mouvement, si le personnage est au sol
        if (moveInput > 0 && isGrounded)
        {
            rotation.y = 0f;  // Remettre la rotation sur l'axe y à 0
        }
        else if (moveInput < 0 && isGrounded)
        {
            rotation.y = 180f;  // Effectuer une rotation de 180 degrés sur l'axe y
        }

        // Appliquer la nouvelle rotation au transform
        transform.rotation = Quaternion.Euler(rotation);
    }


    [PunRPC]
    private void UpdateMovement(Vector2 velocity, float speed)
    {
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
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));
            stream.SendNext(animator.GetBool("Attack"));
            stream.SendNext(animator.GetBool("isJumping"));
        }
        else
        {
            // Réception des informations de mouvement des autres joueurs sur le réseau
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("Attack", (bool)stream.ReceiveNext());
            animator.SetBool("isJumping", (bool)stream.ReceiveNext());
        }
    }

}