using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private Transform playerSpawn;
    public AudioClip sound;

    private void Awake()
    {
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerA") || collision.CompareTag("PlayerB"))
        {
            AudioManager.instance.PlayClipAt(sound, transform.position);
            collision.transform.position = playerSpawn.position;
            
        }
    }
}