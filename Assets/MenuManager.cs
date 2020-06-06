using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void PlayOnePlayerGame()
    {
        Input.ResetInputAxes();
        GameConfig.Instance.useAI = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameplayScene");
    }

    public void PlayTwoPlayerGame()
    {
        Input.ResetInputAxes();
        GameConfig.Instance.useAI = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameplayScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
