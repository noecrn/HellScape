using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CheckPoint : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform playerSpawn;

    private void Awake()
    {
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") || collision.CompareTag("PlayerB"))
        {
            playerSpawn.position = transform.position;
            photonView.RPC("PlayerRespawn", RpcTarget.AllBuffered, collision.gameObject.GetPhotonView().ViewID);
        }
    }
    [PunRPC]
    private void PlayerRespawn(int playerViewID)
    {
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        playerSpawn.position = transform.position;

        // Réactivez le GameObject du joueur et définissez sa position sur celle du checkpoint
        player.SetActive(true);
        player.transform.position = playerSpawn.position;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envoyer la position du checkpoint au réseau
            stream.SendNext(playerSpawn.position);
        }
        else
        {
            // Recevoir la position du checkpoint du réseau
            playerSpawn.position = (Vector3)stream.ReceiveNext();
        }
    }

}
