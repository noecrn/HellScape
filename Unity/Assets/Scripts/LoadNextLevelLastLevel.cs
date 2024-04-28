using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadNextLevelLastLevel : MonoBehaviour
{
    public string nextSceneName;
    public int numCoin;
    private bool isTexted;
    public Text interactUI;

    private void Awake()
    {
        interactUI = GameObject.FindGameObjectWithTag("warningCoins").GetComponent<Text>();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.CompareTag("PlayerA") || col.CompareTag("PlayerB")) && Boss.instance.EnemyHealth <= 0)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        if ((col.CompareTag("PlayerA") || col.CompareTag("PlayerB")) && Inventory.instance.coinCount < numCoin)
        {
            interactUI.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        interactUI.enabled = false;
    }
}
