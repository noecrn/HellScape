using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string levelToLoad;

    public GameObject settingsWindow;

    public void StartGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void SettingsButton()
    {
        settingsWindow.SetActive(true);
    }

    public void CloseSettingsWindow()
    {
        settingsWindow.SetActive(false);
    }

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
