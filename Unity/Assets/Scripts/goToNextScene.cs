using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class goToNextScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NextScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(38f);
        SceneManager.LoadScene("Menu");
    }
}
