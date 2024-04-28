using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthBar : MonoBehaviourPunCallbacks, IPunObservable {

    public Slider healthSlider;    // Référence au slider de la barre de vie
    public Vector3 offset;        // Décalage de la position de la barre de vie

    private Gradient gradient;     // Gradient pour la couleur de la barre de vie
    private GradientColorKey[] colorKeys;     // Tableau des couleurs pour le gradient
    private GradientAlphaKey[] alphaKeys;     // Tableau des alphas pour le gradient

    private void Awake()
    {
        // Initialiser le gradient
        gradient = new Gradient();
        colorKeys = new GradientColorKey[2];
        colorKeys[0].color = Color.green;
        colorKeys[0].time = 0f;
        colorKeys[1].color = Color.red;
        colorKeys[1].time = 1f;

        alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 1f;
        alphaKeys[1].time = 1f;

        gradient.SetKeys(colorKeys, alphaKeys);
    }

    // Initialiser la barre de vie
    public void SetMaxHealth(float maxHealth) {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    // Mettre à jour la barre de vie
    public void SetHealth(float health) {
        healthSlider.value = health;

        // Mettre à jour la couleur de la barre de vie
        healthSlider.fillRect.GetComponentInChildren<Image>().color = gradient.Evaluate(1f - (health / healthSlider.maxValue));
    }

    // Synchroniser la position et la rotation de la barre de vie sur tous les clients
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else {
            transform.position = (Vector3)stream.ReceiveNext() + offset;
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
