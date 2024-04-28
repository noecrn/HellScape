using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class risingLava : MonoBehaviour
{
    public float lavaSpeed = 2f;
    public float maxHeight = 10f;
    private Vector3 initialPosition;
    private Tilemap tilemap;
    private float elapsedTime = 0f;
    
    private void Start()
    {
        initialPosition = transform.position;
        tilemap = GetComponent<Tilemap>();
    }

    private void Update()
    {
        if (PlayerHealth.instance.currentHealth <= 0)
        {
            SceneManager.LoadScene("Level04");
        }
        
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= (1f / lavaSpeed))
        {
            tilemap.RefreshAllTiles();
            elapsedTime = 0f;
        }

        transform.Translate(Vector3.up * lavaSpeed * Time.deltaTime);

        if (transform.position.y >= initialPosition.y + maxHeight)
        {
            transform.position = initialPosition;
        }
        //Debug.Log(PlayerHealth.instance.currentHealth);
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "PlayerA")
        {
            PlayerHealth.instance.TakeDamage(100, "PlayerA");
        }  
        
        if (col.gameObject.tag == "PlayerB")
        {
            PlayerHealth.instance.TakeDamage(100, "PlayerB");
        }
    }
    
}
