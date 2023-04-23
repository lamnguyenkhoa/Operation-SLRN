using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public bool retry = true;
    public TextMeshProUGUI retryText;
    public TextMeshProUGUI highscoreText;

    void OnEnable()
    {
        RefreshRetryText();
        RefreshHighscoreText();
    }

    void Update()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            retry = !retry;
            RefreshRetryText();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (retry)
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
            }
            else
            {
                Application.Quit();
                Debug.Log("Return to main menu...");
            }
        }
    }

    public void RefreshRetryText()
    {
        if (retry)
        {
            retryText.text = "Retry? \n-YES- \nNO";
        }
        else
        {
            retryText.text = "Retry? \nYES \n-NO-";
        }
    }

    public void RefreshHighscoreText()
    {
        int highscore = PlayerPrefs.GetInt("Highscore");
        if (GameManager.instance.score > highscore)
        {
            PlayerPrefs.SetInt("Highscore", (GameManager.instance.score));
            highscoreText.text = "Highest score - " + GameManager.instance.score.ToString("000000000");
        }
        else
        {
            highscoreText.text = "Highest score - " + highscore.ToString("000000000");
        }

    }
}
