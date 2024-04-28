using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu2 : MonoBehaviourPunCallbacks
{
    public InputField createinput;
    public InputField joinInput;
    public string sceneName;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createinput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
