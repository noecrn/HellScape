using UnityEngine;
using Photon.Pun;
using System.Collections;

public class CameraFollow : MonoBehaviourPun, IPunObservable
{
// La vitesse de mouvement des joueurs
    public float moveSpeed = 5f;

    // La distance maximale que les joueurs peuvent être l'un de l'autre (en pourcentage de la taille de l'écran)
    public float maxDistancePercent = 0.1f;

// Position du joueur
    private Vector3 playerPosition;

// Direction du joueur
    private Vector3 playerDirection;

// Vitesse du joueur
    private Vector3 playerVelocity;

// Référence au composant Rigidbody2D du joueur
    private Rigidbody2D rb;

// Initialisation
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (photonView.IsMine)
        {
            // Le joueur contrôle ce personnage
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            // Désactiver le personnage pour les autres joueurs
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }
    }

// Mise à jour
    void Update()
    {
        if (photonView.IsMine)
        {
            // Mouvement du joueur
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            playerDirection = new Vector3(x, 0f, z).normalized;

            // Déplacement du joueur
            playerVelocity = playerDirection * moveSpeed;

            // Limiter la distance entre les joueurs
            if (PhotonNetwork.PlayerList.Length > 1)
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player != photonView.Owner)
                    {
                        float maxDistance = Mathf.Min(Screen.width, Screen.height) * maxDistancePercent;
                        float distance = Vector3.Distance(playerPosition, transform.position);

                        if (distance > maxDistance)
                        {
                            Vector3 direction = (playerPosition - transform.position).normalized;
                            playerVelocity += direction * (distance - maxDistance);
                        }
                    }
                }
            }
        }
    }

// Synchronisation de la position
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            rb.MovePosition(rb.position + new Vector2(playerVelocity.x, playerVelocity.z) * Time.fixedDeltaTime);
        }
        else
        {
            rb.position = Vector3.Lerp(rb.position, playerPosition, 0.1f);
        }
    }

// Synchronisation de la position et de la vitesse
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(playerVelocity);
        }
        else
        {
            playerPosition = (Vector3)stream.ReceiveNext();
            playerVelocity = (Vector3)stream.ReceiveNext();
        }
    }
}

