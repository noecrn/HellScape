using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject hostPlayerPrefab; // Préfab pour l'hôte
    public Transform startPoint;

    private GameObject playerInstance; // Instance du joueur actuel


    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // si le joueur est l'hôte
        {
            // Instancier l'objet sur le réseau
            playerInstance = PhotonNetwork.Instantiate(hostPlayerPrefab.name, startPoint.position, Quaternion.identity);
        }
        else // si le joueur est un client
        {
            // Instancier l'objet sur le réseau
            playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, startPoint.position, Quaternion.identity);
        }
        // Ajouter la cible à la caméra
        if (playerInstance != null)
        {
            PAcameraFollow cameraFollowScript = Camera.main.GetComponent<PAcameraFollow>();
            cameraFollowScript.AddTarget(playerInstance.transform);

            // Récupérer le script PlayerHealth pour le joueur local
            PlayerHealth playerHealthScript = playerInstance.GetComponent<PlayerHealth>();

            // Mettre à jour la barre de vie pour tous les joueurs
            playerHealthScript.SetMaxHealth(playerHealthScript.maxHealth);

            // Synchroniser les changements de barre de vie pour tous les joueurs
            photonView.RPC("SetMaxHealth", RpcTarget.OthersBuffered, playerHealthScript.maxHealth);

        }
    }
}
