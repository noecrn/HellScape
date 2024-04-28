using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public InputField createinput;
    public InputField joinInput;
    public string sceneName;

    void Start()
    {
        // ...
        // ensure that all players play on the same map
        PhotonNetwork.AutomaticallySyncScene = true;
    }

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