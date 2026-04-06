using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;
    public GameObject gameOverUI;
    public TextMeshProUGUI statsText;
    public Button buttonPlayAgain;

    public


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        gameOverUI.SetActive(false);
        buttonPlayAgain.onClick.AddListener(RestartGame);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        string stats = ViolationStatistics.Instance.GetStatisticsText();
        statsText.text = stats;
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        Scene currentScence = SceneManager.GetActiveScene();
        int scenceIndex = currentScence.buildIndex;
        SceneManager.LoadScene(scenceIndex);

    }
}
