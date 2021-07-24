using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string mGameSceneName = "GameScene";

    public void PlayGame()
    {
        SceneManager.LoadScene(mGameSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
