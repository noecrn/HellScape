using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviourPunCallbacks
{
    public static RespawnManager Instance;

    public Transform playerSpawn; // Position de respawn par d�faut

    public bool isRespawnPositionsSynced = false;

    private void Awake()
    {
        Instance = this;
    }

    public void SyncRespawnPositions()
    {
        // Envoyer les positions de respawn aux joueurs connect�s
        if (PhotonNetwork.IsMasterClient)
        {
            // R�cup�rer les positions de respawn de tous les checkpoints dans une liste
            CheckPoint[] checkpoints = FindObjectsOfType<CheckPoint>();
            List<Vector3> respawnPositions = new List<Vector3>();
            foreach (CheckPoint checkpoint in checkpoints)
            {
                respawnPositions.Add(checkpoint.playerSpawn.position);
            }

            // Envoyer les positions de respawn aux autres joueurs
            photonView.RPC("ReceiveRespawnPositions", RpcTarget.OthersBuffered, respawnPositions.ToArray());
        }
    }

    [PunRPC]
    private void ReceiveRespawnPositions(Vector3[] positions)
    {
        // Recevoir les positions de respawn des autres joueurs
        CheckPoint[] checkpoints = FindObjectsOfType<CheckPoint>();

        // V�rifier si le nombre de positions re�ues correspond au nombre de checkpoints
        if (positions.Length != checkpoints.Length)
        {
            Debug.LogError("Number of received respawn positions does not match the number of checkpoints.");
            return;
        }

        // Mettre � jour les positions de respawn locales avec les positions re�ues
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].playerSpawn.position = positions[i];
        }
    }


}
