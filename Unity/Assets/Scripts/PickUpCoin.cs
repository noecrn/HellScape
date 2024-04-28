using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCoin : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA")||collision.CompareTag("PlayerB"))
        {
            AudioManager.instance.PlayClipAt(sound, transform.position);
            Inventory.instance.AddCoins(1);
            Destroy(gameObject);
        }
    }
}
