using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public bool retry = true;
    public TextMeshProUGUI retryText;
    public GameObject[] cracks;
    void OnEnable()
    {
        RefreshRetryText();
    }

    void Update()
    {
        // Maybe I should just delete this script and put GameOverMenu control
        // inside GameManager too.
        if (!GameManager.instance.isInLeaderboard)
        {
            if (Input.GetButtonDown("Vertical"))
            {
                retry = !retry;
                RefreshRetryText();
            }

            if (Input.GetKeyDown(KeyCode.E) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.Space))
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
    }

    public void RefreshRetryText()
    {
        if (retry)
            retryText.text = "Retry? \n-YES- \nNO";
        else
            retryText.text = "Retry? \nYES \n-NO-";
    }

    public void HideCracks()
    {
        foreach (GameObject crack in cracks)
            crack.SetActive(false);
    }
}
