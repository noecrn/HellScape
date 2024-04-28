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
        rb.bodyType = RigidbodyType2D.Static; // Définir le bodyType sur Static au départ
    }

    private void Update()
    {
        if (playersOnPlatform.Count > 0)
        {
            fallDelay -= Time.deltaTime; // Réduire le délai de chute en fonction du temps écoulé

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
            playersOnPlatform.Add(collision.gameObject); // Ajouter le joueur à la liste des joueurs sur la plateforme
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerA") || collision.gameObject.CompareTag("PlayerB"))
        {
            playersOnPlatform.Remove(collision.gameObject); // Retirer le joueur de la liste des joueurs sur la plateforme
            fallDelay = 2f; // Réinitialiser le délai de chute
        }
    }

    private void Fall()
    {
        rb.bodyType = RigidbodyType2D.Dynamic; // Définir le bodyType sur Dynamic pour permettre à la plateforme de tomber

        // Déplacer tous les joueurs vers une position sûre
        foreach (GameObject player in playersOnPlatform)
        {
            player.transform.SetParent(null); // Dissocier le joueur de la plateforme
        }

        // Détruire la plateforme après un court délai
        Destroy(gameObject, 0.5f); // Supprime l'objet de la scène après 0.5 seconde (ajustez le délai selon vos besoins)
    }
}


