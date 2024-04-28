using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class KeyAndChestInteraction : MonoBehaviourPunCallbacks
{
    public GameObject[] mushroomChests; // Tableau des GameObjects des coffres Mushroom Chest
    public GameObject[] whiteChests; // Tableau des GameObjects des coffres White Chest
    public KeyCode interactKey = KeyCode.E; // Touche d'interaction
    public float coinActivationDistance = 5f; // Distance maximale pour activer les "Coins"
    private InventorySystem inventory; // Référence au script InventorySystem
    private InventoryRef inventoryRef;
    private GameObject[] coins;
    private GameObject[] apples;

    public GameObject[] mushroomKey;
    public GameObject[] whiteKey;
    private ChestAnimation chestAnimation;


    private void Start()
    {
        inventoryRef = InventoryRef.instance;
        inventory = FindObjectOfType<InventorySystem>(); // Trouver le script InventorySystem dans la scène
        mushroomChests = inventoryRef.mushroomChests;
        whiteChests = inventoryRef.whiteChests;
        coins = inventoryRef.Coins;
        mushroomKey = inventoryRef.mushroomKey;
        whiteKey = inventoryRef.whiteKey;
        apples = inventoryRef.Apple;
        chestAnimation = GetComponent<ChestAnimation>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey) && photonView.IsMine)
        {
            // Rechercher le coffre le plus proche
            GameObject closestChest = GetClosestChest();

            if (closestChest != null && HasCorrectKey(closestChest))
            {
                float distanceToChest = Vector3.Distance(transform.position, closestChest.transform.position);
                if (distanceToChest <= 2f)
                {
                    photonView.RPC("OpenChest", RpcTarget.All, GetChestType(closestChest));
                }
            }
        }
    }

    private GameObject GetClosestChest()
    {
        GameObject closestChest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject chest in mushroomChests.Concat(whiteChests))
        {
            float distance = Vector3.Distance(transform.position, chest.transform.position);
            if (distance < closestDistance)
            {
                closestChest = chest;
                closestDistance = distance;
            }
        }

        return closestChest;
    }

    private bool HasCorrectKey(GameObject chest)
    {
        if (chest.CompareTag("MushroomChest"))
        {
            return inventory.hasKey(mushroomKey);
        }
        else if (chest.CompareTag("WhiteChest"))
        {
            return inventory.hasKey(whiteKey);
        }

        return false;
    }

    private string GetChestType(GameObject chest)
    {
        if (chest.CompareTag("MushroomChest"))
        {
            return "MushroomChest";
        }
        else if (chest.CompareTag("WhiteChest"))
        {
            return "WhiteChest";
        }

        return "";
    }

    [PunRPC]
    void OpenChest(string chestType)
    {
        GameObject closestChest = GetClosestChest();
        chestAnimation = closestChest.GetComponent<ChestAnimation>(); // Récupérer le script ChestAnimation du coffre le plus proche

        if (chestType == "MushroomChest")
        {
            GameObject[] chests = mushroomChests;
            GameObject[] applesToActivate = apples.Where(apple => Vector3.Distance(apple.transform.position, chests[0].transform.position) <= coinActivationDistance).ToArray();

            chestAnimation.PlayChestAnimation(); // Appel de la fonction d'ouverture du coffre

            StartCoroutine(WaitForAnimationFinished(chestType));

            // Autres actions spécifiques au coffre Mushroom Chest
            Debug.Log("Mushroom Chest opened!");


            //ActivateApples(applesToActivate); // Activer les pommes lorsque l'animation est terminée

            if (mushroomKey.Length > 0)
            {
                GameObject keyToRemove = mushroomKey[0]; // Récupérer la première clé Mushroom du tableau
                inventory.RemoveItem(keyToRemove); // Appeler la méthode RemoveItem pour supprimer la clé de la liste items

                GameObject[] updatedMushroomKeys = new GameObject[mushroomKey.Length - 1];
                Array.Copy(mushroomKey, 1, updatedMushroomKeys, 0, mushroomKey.Length - 1);
                mushroomKey = updatedMushroomKeys;
            }
            //photonView.RPC("CloseChest", RpcTarget.All, chestType); 
        }
        else if (chestType == "WhiteChest")
        {
            GameObject[] chests = whiteChests;
            GameObject[] coinsToActivate = coins.Where(coin => Vector3.Distance(coin.transform.position, chests[0].transform.position) <= coinActivationDistance).ToArray();

            chestAnimation.PlayChestAnimation(); // Appel de la fonction d'ouverture du coffre

            StartCoroutine(WaitForAnimationFinished(chestType));

            // Autres actions spécifiques au coffre White Chest
            Debug.Log("White Chest opened!");

            if (whiteKey.Length > 0)
            {
                GameObject keyToRemove = whiteKey[0]; // Récupérer la première clé Mushroom du tableau
                inventory.RemoveItem(keyToRemove); // Appeler la méthode RemoveItem pour supprimer la clé de la liste items

                GameObject[] updatedWhiteKeys = new GameObject[whiteKey.Length - 1];
                Array.Copy(whiteKey, 1, updatedWhiteKeys, 0, whiteKey.Length - 1);
                whiteKey = updatedWhiteKeys;
            }
            //photonView.RPC("CloseChest", RpcTarget.All, chestType);
        }
    }


    [PunRPC]
    void CloseChest(string chestType)
    {
        if (chestType == "MushroomChest")
        {
            GameObject[] chests = mushroomChests;

            chests[0].GetComponent<ChestAnimation>().PlayChestAnimation(); // Appel de la fonction de fermeture du coffre
        }
        else if (chestType == "WhiteChest")
        {
            GameObject[] chests = whiteChests;

            chests[0].GetComponent<ChestAnimation>().PlayChestAnimation(); // Appel de la fonction de fermeture du coffre
        }
    }


    private void ActivateCoins(GameObject[] coinsToActivate)
    {
        Debug.Log("oui");
        foreach (GameObject coin in coinsToActivate)
        {
            coin.SetActive(true);
        }
    }

    private void ActivateApples(GameObject[] applesToActivate)
    {
        Debug.Log("oui");
        foreach (GameObject apple in applesToActivate)
        {
            apple.SetActive(true);
        }
    }

    private IEnumerator WaitForAnimationFinished(string chestType)
    {
        while (!chestAnimation.isAnimationFinished)
        {
            yield return null;
        }

        if (chestType == "MushroomChest")
        {
            GameObject[] chests = mushroomChests;
            GameObject[] applesToActivate = apples.Where(apple => Vector3.Distance(apple.transform.position, chests[0].transform.position) <= coinActivationDistance).ToArray();
            ActivateApples(applesToActivate); // Activer les pommes lorsque l'animation est terminée

        }
        else if (chestType == "WhiteChest")
        {
            GameObject[] chests = whiteChests;
            GameObject[] coinsToActivate = coins.Where(coin => Vector3.Distance(coin.transform.position, chests[0].transform.position) <= coinActivationDistance).ToArray();

            ActivateCoins(coinsToActivate); // Activer les pièces lorsque l'animation est terminée
        }
        chestAnimation.isAnimationFinished = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(mushroomChests);
            stream.SendNext(whiteChests);
            stream.SendNext(coins);
            stream.SendNext(mushroomKey);
            stream.SendNext(whiteKey);
        }
        else
        {
            mushroomChests = (GameObject[])stream.ReceiveNext();
            whiteChests = (GameObject[])stream.ReceiveNext();
            coins = (GameObject[])stream.ReceiveNext();
            mushroomKey = (GameObject[])stream.ReceiveNext();
            whiteKey = (GameObject[])stream.ReceiveNext();
        }
    }
}