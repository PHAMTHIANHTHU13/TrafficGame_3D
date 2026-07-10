using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject quitPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowQuitPanel()
    {
        quitPanel.SetActive(true);
    }

    public void HideQuitPanel()
    {
        quitPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    // public void QuitGame()
    // {
    //     Debug.Log("da thoat game");
    //     Application.Quit();
    // }
}
