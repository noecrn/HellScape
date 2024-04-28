using UnityEngine;

public class BDF_Generator : MonoBehaviour
{
    public GameObject bouleDeFeuPrefab;  // Pr�fabriqu� de la boule de feu
    public float delaiMin = 0.1f;  // D�lai minimum entre chaque g�n�ration
    public float delaiMax = 0.3f;  // D�lai maximum entre chaque g�n�ration

    private float tempsEcoule = 0f;
    private float tempsAttente = 0f;
    public int nombreBoulesDeFeu = 10;

    void Update()
    {
   
        tempsEcoule += Time.deltaTime;

        // G�n�rer une nouvelle boule de feu lorsque le temps d'attente est �coul�
        if (tempsEcoule >= tempsAttente)
        {
            for (int i = 0; i < nombreBoulesDeFeu; i++)
            {
                GenererBouleDeFeu();
            }

            // R�initialiser le temps �coul� et le temps d'attente
            tempsEcoule = 0f;
            tempsAttente = Random.Range(delaiMin, delaiMax);
        }
    }

    void GenererBouleDeFeu()
    {
        // Instancier une nouvelle boule de feu � une position al�atoire
        float posX = Random.Range(15f, 190f);
        Vector3 position = new Vector3(posX, transform.position.y, 0f);
        Instantiate(bouleDeFeuPrefab, position, Quaternion.identity);
    }
}
