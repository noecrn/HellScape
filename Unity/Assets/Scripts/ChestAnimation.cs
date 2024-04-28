using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChestAnimation : MonoBehaviourPunCallbacks, IPunObservable
{
    public Animator animator; // R�f�rence � l'animator du coffre
    [SerializeField] private bool open = false; // Variable bool�enne pour contr�ler l'�tat du coffre
    private bool syncOpen = false; // �tat du coffre synchronis� entre les clients


    public bool isAnimationFinished = false;


    private void Start()
    {
        if (PhotonNetwork.IsConnected)
            photonView.ObservedComponents.Add(this);
    }

    public void PlayChestAnimation()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("SyncChestState", RpcTarget.AllBuffered, !syncOpen);
        }
        else
        {
            ToggleChestState();
        }
    }

    [PunRPC]
    private void SyncChestState(bool newState)
    {
        syncOpen = newState;
        ToggleChestState();
    }

    private void ToggleChestState()
    {
        open = syncOpen; // Utilise l'�tat synchronis� du coffre

        animator.SetBool("open", open); // Met � jour le param�tre "Open" de l'animator avec la valeur de la variable "open"

        if (open)
        {
            StartCoroutine(AnimationFinishedCoroutine()); // D�marre une coroutine pour d�tecter la fin de l'animation
        }
    }

    private IEnumerator AnimationFinishedCoroutine()
    {
        yield return new WaitForSeconds(1f); // Attendez une seconde (ajustez si n�cessaire)

        isAnimationFinished = true; // Animation termin�e
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(syncOpen);
        }
        else
        {
            syncOpen = (bool)stream.ReceiveNext();
            ToggleChestState();
        }
    }
}
