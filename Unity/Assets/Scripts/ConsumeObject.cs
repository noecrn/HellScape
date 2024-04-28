using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeObject : MonoBehaviour
{
    private PlayerHealth health;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    public void Consume(GameObject obj)
    {
        if (obj.CompareTag("Apple"))
        {
            health.HealPlayer(20);
        }
    }
}
