using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRef : MonoBehaviour
{
    public static InventoryRef instance;

    [Header("UI Items Section")]
    public GameObject ui_Window;
    public Image[] items_images;
    [Header("UI Item Description")]
    public GameObject ui_Description_Window;
    public Image description_Image;
    public Text description_Title;
    public Text description_Text;

    public GameObject[] mushroomChests;
    public GameObject[] whiteChests;
    public GameObject[] Coins;
    public GameObject[] Apple;

    public GameObject[] mushroomKey;
    public GameObject[] whiteKey;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
