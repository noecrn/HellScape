using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public Transform startPoint;
    public GameObject hostPlayer; // GameObject pour l'hôte
    // public GameObject myPrefab;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // si le joueur est l'hôte
        {
            PhotonNetwork.Instantiate(hostPlayer.name, startPoint.position, Quaternion.identity);
        }
        else // si le joueur est un client
        {
            PhotonNetwork.Instantiate(player.name, startPoint.position, Quaternion.identity);
        }
        // if (PhotonNetwork.IsConnected)
        // {
        //     PhotonNetwork.Instantiate(myPrefab.name, Vector3.zero, Quaternion.identity);
        // }
    }
}






























