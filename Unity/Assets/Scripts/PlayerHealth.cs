using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    public int maxHealth = 100;
    public int currentHealth;
    public static PlayerHealth instance;

    private HealthBar healthBarPrefab;
    private HealthBar healthBarInstance;

    public AudioClip sound;

    private Transform playerSpawn;
    private Animator fadeSystem;

    
    
    

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerHealth dans la scène");
            return;
        }

        instance = this;
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (photonView.IsMine)
        {
            // Instancier la HealthBar sur le joueur local
            healthBarInstance = Instantiate(healthBarPrefab);
            healthBarInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
            healthBarInstance.SetMaxHealth(maxHealth);
        }
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        currentHealth = health;

        if (healthBarInstance != null) // Check if healthBarInstance is not null
        {
            healthBarInstance.SetMaxHealth(health);
        }
    }

    void Update()
    {
        //Debug.Log(loadNextLevel.nextSceneName);

        if (instance.currentHealth <= 0)
        {
            Debug.Log("Pas Ok");
            AudioManager.instance.PlayClipAt(sound, transform.position);
            SetMaxHealth(maxHealth); 
            transform.position = playerSpawn.position;
            
            //fadeSystem.SetTrigger("playerHealth");
        }

        if (instance.currentHealth <= 0)
        {
            Debug.Log("Ok");
            SetMaxHealth(maxHealth);
            SceneManager.LoadScene("Level04");
        }
        

    
        fadeSystem.SetFloat("playerHealth", currentHealth);
        
    }

    public void HealPlayer(int amount)
    {
        if ((currentHealth + amount) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }

        photonView.RPC("UpdateHealth", RpcTarget.AllBuffered, (float)currentHealth);
    }

    public void TakeDamage(int damage, string PlayerTag)
    {
        if (PlayerTag == "PlayerA")
        {
            
            if (!MovePlayerA.instance.isDoging)
            {
                if (photonView.IsMine && gameObject.CompareTag(PlayerTag))
                {
                    currentHealth -= damage;
                    healthBarInstance.SetHealth(currentHealth);
                    // Appeler la méthode TakeDamage sur tous les clients connectés
                    photonView.RPC("UpdateHealth", RpcTarget.AllBuffered, currentHealth);
                }
            }
            else
            {
                MovePlayerA.instance.dodgeAnim = true;
            }
        }
        
        if (PlayerTag == "PlayerB")
        {
            if (!MovePlayerB.instance.isDodging)
            {
                if (photonView.IsMine && gameObject.CompareTag(PlayerTag))
                {
                    currentHealth -= damage;
                    healthBarInstance.SetHealth(currentHealth);
                    // Appeler la méthode TakeDamage sur tous les clients connectés
                    photonView.RPC("UpdateHealth", RpcTarget.AllBuffered, currentHealth);
                }
            }
            else
            {
                MovePlayerA.instance.dodgeAnim = true;
            }
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        // Charger le prefab de la HealthBar depuis le dossier Resources
        healthBarPrefab = Resources.Load<HealthBar>("HealthBar");
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            healthBarInstance.SetHealth(currentHealth);
        }
    }

    [PunRPC]
    void UpdateHealth(float health)
    {
        if (photonView.IsMine)
        {
            currentHealth = (int)health;
            healthBarInstance.SetHealth(currentHealth);
        }
    }
}
