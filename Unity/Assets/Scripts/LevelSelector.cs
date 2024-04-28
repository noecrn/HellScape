using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;

    public void LoadLevelPassed(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
