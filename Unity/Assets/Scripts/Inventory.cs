using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int coinCount;
    public Text CoinCountText;

    public static Inventory instance;

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("attention plusieurs instances inventaires");
            return;
        }
        instance = this;
    }

    public void AddCoins(int count)
    {
        coinCount += count;
        CoinCountText.text = coinCount.ToString();
    }
}
