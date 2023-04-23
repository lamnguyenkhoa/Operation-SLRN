using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public bool retry = true;
    public TextMeshProUGUI retryText;
    void OnEnable()
    {
        RefreshRetryText();
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
}
