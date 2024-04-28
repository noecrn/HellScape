using UnityEngine;

public class BDF : MonoBehaviour
{
    public int degats = 15;  // D�g�ts inflig�s au joueur

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Position de d�part al�atoire
        float posX = Random.Range(15f, 190f);
        transform.position = new Vector3(posX, 21f, 0f);

        // Vitesse de chute al�atoire
        //float vitesseChute = Random.Range(0.2f, 0.4f);
        //rb.velocity = new Vector2(0f, -vitesseChute);
    }

    void Update()
    {
        // Si la boule de feu sort de l'�cran, la d�truire
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // V�rifier si la collision est avec un objet que vous souhaitez
        // d�truire la boule de feu (par exemple, le joueur)
        if (collision.CompareTag("Fondz"))
            if (collision.CompareTag("Fondz") || collision.CompareTag("PlayerA") || collision.CompareTag("PlayerB"))
            {
                // D�truire la boule de feu
                Destroy(gameObject);
            }

        if (collision.gameObject.tag == "PlayerA")
        {
            PlayerHealth.instance.TakeDamage(degats, "PlayerA");
        }

        if (collision.gameObject.tag == "PlayerB")
        {
            PlayerHealth.instance.TakeDamage(degats, "PlayerB");
        }
    }

}