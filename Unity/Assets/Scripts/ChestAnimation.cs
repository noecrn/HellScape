using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChestAnimation : MonoBehaviourPunCallbacks, IPunObservable
{
    public Animator animator; // Référence à l'animator du coffre
    [SerializeField] private bool open = false; // Variable booléenne pour contrôler l'état du coffre
    private bool syncOpen = false; // État du coffre synchronisé entre les clients


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
        open = syncOpen; // Utilise l'état synchronisé du coffre

        animator.SetBool("open", open); // Met à jour le paramètre "Open" de l'animator avec la valeur de la variable "open"

        if (open)
        {
            StartCoroutine(AnimationFinishedCoroutine()); // Démarre une coroutine pour détecter la fin de l'animation
        }
    }

    private IEnumerator AnimationFinishedCoroutine()
    {
        yield return new WaitForSeconds(1f); // Attendez une seconde (ajustez si nécessaire)

        isAnimationFinished = true; // Animation terminée
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
