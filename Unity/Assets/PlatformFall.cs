using UnityEngine;
using System.Collections.Generic;

public class PlatformFall : MonoBehaviour
{
    public float fallDelay = 2f; // Temps avant que la plateforme ne tombe
    private Rigidbody2D rb;
    private List<GameObject> playersOnPlatform = new List<GameObject>();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static; // D�finir le bodyType sur Static au d�part
    }

    private void Update()
    {
        if (playersOnPlatform.Count > 0)
        {
            fallDelay -= Time.deltaTime; // R�duire le d�lai de chute en fonction du temps �coul�

            if (fallDelay <= 0)
            {
                Fall();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerA") || collision.gameObject.CompareTag("PlayerB"))
        {
            playersOnPlatform.Add(collision.gameObject); // Ajouter le joueur � la liste des joueurs sur la plateforme
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerA") || collision.gameObject.CompareTag("PlayerB"))
        {
            playersOnPlatform.Remove(collision.gameObject); // Retirer le joueur de la liste des joueurs sur la plateforme
            fallDelay = 2f; // R�initialiser le d�lai de chute
        }
    }

    private void Fall()
    {
        rb.bodyType = RigidbodyType2D.Dynamic; // D�finir le bodyType sur Dynamic pour permettre � la plateforme de tomber

        // D�placer tous les joueurs vers une position s�re
        foreach (GameObject player in playersOnPlatform)
        {
            player.transform.SetParent(null); // Dissocier le joueur de la plateforme
        }

        // D�truire la plateforme apr�s un court d�lai
        Destroy(gameObject, 0.5f); // Supprime l'objet de la sc�ne apr�s 0.5 seconde (ajustez le d�lai selon vos besoins)
    }
}


