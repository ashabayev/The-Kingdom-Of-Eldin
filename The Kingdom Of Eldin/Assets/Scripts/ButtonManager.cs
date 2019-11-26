using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public Button PlayButton;
    public string newGameSceneName;

    public void PlayGame()
    {
        SceneManager.LoadScene(newGameSceneName);
    }
}
