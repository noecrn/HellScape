using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu1 : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public string defaultSceneName; // la scène par défaut si aucune scène n'est spécifiée dans MainMenu
    private bool joinDefaultScene; // drapeau indiquant si le client doit rejoindre la scène par défaut

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    // Appelé lorsque le client rejoint une salle
    public override void OnJoinedRoom()
    {
        // Vérifie si le client doit rejoindre la scène par défaut
        if (joinDefaultScene)
        {
            PhotonNetwork.LoadLevel(defaultSceneName);
        }
        else if (SceneManager.GetActiveScene().name != PhotonNetwork.CurrentRoom.Name)
        {
            // Si le client a choisi une scène différente de celle de l'hôte, il rejoint la scène de l'hôte
            PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.Name);
        }
    }

    // Appelé lorsque le client rejoint le lobby
    public override void OnJoinedLobby()
    {
        // Si le client n'a pas choisi de scène spécifique, il rejoint la scène par défaut
        joinDefaultScene = true;
    }

    // Appelé lorsque le client quitte la salle
    public override void OnLeftRoom()
    {
        // Réinitialise le drapeau pour le prochain client qui rejoint
        joinDefaultScene = false;
    }
}
