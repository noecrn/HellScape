using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("General Fields")]
    //List of items picked up
    public List<GameObject> items = new List<GameObject>();
    //flag indicates if the inventory is open or not
    public bool isOpen;
    [Header("UI Items Section")]
    //Inventory System Window
    public GameObject ui_Window;
    public Image[] items_images;
    [Header("UI Item Description")]
    public GameObject ui_Description_Window;
    public Image description_Image;
    public Text description_Title;
    public Text description_Text;
    public PlayerHealth health;

    private InventoryRef inventoryRef;

    private void Awake()
    {
        inventoryRef = InventoryRef.instance;
        health = GetComponent<PlayerHealth>();

        // Assign GameObjects from the GameManager
        ui_Window = inventoryRef.ui_Window;
        items_images = inventoryRef.items_images;
        description_Image = inventoryRef.description_Image;
        description_Title = inventoryRef.description_Title;
        description_Text = inventoryRef.description_Text;

        AddPointerEventsToImages();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        ui_Window.SetActive(isOpen);

        Update_UI();
    }

    public void PickUp(GameObject item)
    {
        // Add the item to the inventory of the local player
        items.Add(item);
        item.SetActive(false);

        // Call RPC function to pick up item for all clients except the local player
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("PickUpItem", RpcTarget.Others, item.GetComponent<PhotonView>().ViewID);
        }

        Update_UI();
    }

    public bool hasKey(GameObject[] keys)
    {
        foreach (GameObject key in keys)
        {
            if (items.Contains(key))
            {
                return true;
            }
        }
        return false;
    }

    //Refresh the UI elements in the inventory window    
    void Update_UI()
    {
        HideAll();
        //For each item in the "items" list 
        //Show it in the respective slot in the "items_images"
        for (int i = 0; i < items.Count; i++)
        {
            items_images[i].sprite = items[i].GetComponent<SpriteRenderer>().sprite;
            items_images[i].gameObject.SetActive(true);
        }
    }

    public void AddPointerEventsToImages()
    {
        for (int i = 0; i < items_images.Length; i++)
        {
            Image image = items_images[i];
            EventTrigger trigger = image.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = image.gameObject.AddComponent<EventTrigger>();
            }

            int index = i; // Sauvegardez la valeur de l'index pour l'utiliser dans les callbacks

            // Créer un nouvel événement PointerEnter et ajouter la méthode ShowDescription en tant qu'action
            EventTrigger.Entry enterEvent = new EventTrigger.Entry();
            enterEvent.eventID = EventTriggerType.PointerEnter;
            enterEvent.callback.AddListener((eventData) => { ShowDescription(index); });
            trigger.triggers.Add(enterEvent);

            // Créer un nouvel événement PointerExit et ajouter la méthode HideDescription en tant qu'action
            EventTrigger.Entry exitEvent = new EventTrigger.Entry();
            exitEvent.eventID = EventTriggerType.PointerExit;
            exitEvent.callback.AddListener((eventData) => { HideDescription(); });
            trigger.triggers.Add(exitEvent);

            // Créer un nouvel événement PointerClick et ajouter la méthode Consume en tant qu'action
            EventTrigger.Entry clickEvent = new EventTrigger.Entry();
            clickEvent.eventID = EventTriggerType.PointerClick;
            clickEvent.callback.AddListener((eventData) => { Consume(index); });
            trigger.triggers.Add(clickEvent);
        }
    }

    //Hide all the items ui images
    void HideAll()
    {
        foreach (var i in items_images) { i.gameObject.SetActive(false); }

        HideDescription();
    }

    public void ShowDescription(int id)
    {
        if (photonView.IsMine)
        {
            //Set the Image
            description_Image.sprite = items_images[id].sprite;
            //Set the Title
            description_Title.text = items[id].name;
            //Show the description
            description_Text.text = items[id].GetComponent<Item>().descriptionText;
            //Show the elements
            description_Image.gameObject.SetActive(true);
            description_Title.gameObject.SetActive(true);
            description_Text.gameObject.SetActive(true);
        }
    }

    public void HideDescription()
    {
        description_Image.gameObject.SetActive(false);
        description_Title.gameObject.SetActive(false);
        description_Text.gameObject.SetActive(false);
    }

    public void Consume(int id)
    {
        if (items[id].GetComponent<Item>().type == Item.ItemType.Consumables)
        {
            Debug.Log($"CONSUMED {items[id].name}");
            // Call the local ConsumeItem method directly
            ConsumeItem(id, items[id].GetComponent<PhotonView>().ViewID);
        }
    }

    void ConsumeItem(int id, int viewID)
    {
        health.HealPlayer(20);

        // Call the RPC to consume the item for all clients
        photonView.RPC("ConsumeItemRPC", RpcTarget.All, id, viewID);
    }

    [PunRPC]
    void ConsumeItemRPC(int id, int viewID)
    {
        // Find item with the given viewID and destroy it for all clients
        PhotonNetwork.Destroy(PhotonView.Find(viewID));

        // Remove the item from the items list for all clients
        items.RemoveAt(id);

        // Update UI for all clients
        Update_UI();
    }

    public void RemoveItem(GameObject item)
    {
        if (items.Contains(item))
        {
            int index = items.IndexOf(item); // Trouver l'index de l'élément dans la liste
            items.RemoveAt(index); // Supprimer l'élément à cet index
        }
    }

    [PunRPC]
    void PickUpItem(int viewID)
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            // Find the item with the given viewID and disable it
            GameObject item = PhotonView.Find(viewID).gameObject;
            items.Remove(item);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the current state of the items list to other clients
            stream.SendNext(items.Count);
            foreach (var item in items)
            {
                stream.SendNext(item.GetComponent<PhotonView>().ViewID);
            }
        }
        else
        {
            // Receive the state of the items list from the server and update it
            int itemCount = (int)stream.ReceiveNext();
            items.Clear();
            for (int i = 0; i < itemCount; i++)
            {
                int viewID = (int)stream.ReceiveNext();
                GameObject item = PhotonView.Find(viewID).gameObject;
                items.Add(item);
            }
            // Update the UI to reflect the new state of the items list
            Update_UI();
        }
    }
}
