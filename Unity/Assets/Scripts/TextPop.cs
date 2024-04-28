using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPop : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] string[] sentences;
    int index;
    bool isOndial, canDial;
    private Text interactUI;

    HUDManager manager => HUDManager.instance;

    private void Awake()
    {
        interactUI = GameObject.FindGameObjectWithTag("InteractUI").GetComponent<Text>();
    }

    private void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.LeftShift) && canDial)
        {
            photonView.RPC("StartDialogue", RpcTarget.All);
            manager.continueButton.GetComponent<Button>().onClick.RemoveAllListeners();
            manager.continueButton.GetComponent<Button>().onClick.AddListener(delegate { photonView.RPC("NextLine", RpcTarget.All); });
        }
    }

    [PunRPC]
    public void StartDialogue()
    {
        manager.dialogueHolder.SetActive(true);
        isOndial = true;
        TypingText(sentences);
    }

    void TypingText(string[] sentence)
    {
        manager.textDisplay.text = "";
        manager.textDisplay.text = sentence[index];
        if (manager.textDisplay.text == sentence[index])
            manager.continueButton.SetActive(true);
    }

    [PunRPC]
    public void NextLine()
    {
        manager.continueButton.SetActive(false);
        if (isOndial && index < sentences.Length - 1)
        {
            index++;
            manager.textDisplay.text = "";
            TypingText(sentences);
        }
        else if (isOndial && index == sentences.Length - 1)
        {
            isOndial = false;
            index = 0;
            manager.textDisplay.text = "";
            manager.dialogueHolder.SetActive(false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(index);
            stream.SendNext(isOndial);
            stream.SendNext(canDial);
        }
        else
        {
            index = (int)stream.ReceiveNext();
            isOndial = (bool)stream.ReceiveNext();
            canDial = (bool)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (photonView.IsMine && (col.tag == "PlayerA" || col.tag == "PlayerB"))
        {
            canDial = true;
            interactUI.enabled = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (photonView.IsMine && (other.tag == "PlayerA" || other.tag == "PlayerB"))
        {
            canDial = false;
            interactUI.enabled = false;
        }
    }

}
