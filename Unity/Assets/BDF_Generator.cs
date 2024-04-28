using UnityEngine;

public class BDF_Generator : MonoBehaviour
{
    public GameObject bouleDeFeuPrefab;  // Préfabriqué de la boule de feu
    public float delaiMin = 0.1f;  // Délai minimum entre chaque génération
    public float delaiMax = 0.3f;  // Délai maximum entre chaque génération

    private float tempsEcoule = 0f;
    private float tempsAttente = 0f;
    public int nombreBoulesDeFeu = 10;

    void Update()
    {
   
        tempsEcoule += Time.deltaTime;

        // Générer une nouvelle boule de feu lorsque le temps d'attente est écoulé
        if (tempsEcoule >= tempsAttente)
        {
            for (int i = 0; i < nombreBoulesDeFeu; i++)
            {
                GenererBouleDeFeu();
            }

            // Réinitialiser le temps écoulé et le temps d'attente
            tempsEcoule = 0f;
            tempsAttente = Random.Range(delaiMin, delaiMax);
        }
    }

    void GenererBouleDeFeu()
    {
        // Instancier une nouvelle boule de feu à une position aléatoire
        float posX = Random.Range(15f, 190f);
        Vector3 position = new Vector3(posX, transform.position.y, 0f);
        Instantiate(bouleDeFeuPrefab, position, Quaternion.identity);
    }
}
