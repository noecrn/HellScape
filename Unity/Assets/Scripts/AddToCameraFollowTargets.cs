using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToCameraFollowTargets : MonoBehaviour
{
    public void Start()
    {
        PAcameraFollow cameraFollowScript = Camera.main.GetComponent<PAcameraFollow>();
        if (cameraFollowScript != null)
        {
            cameraFollowScript.AddTarget(transform);
        }
    }
}