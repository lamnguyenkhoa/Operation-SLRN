using TMPro;
using UnityEngine;

public class HighscoreText : MonoBehaviour
{
    void OnEnable()
    {
        RefreshHighscoreText();
    }

    public void RefreshHighscoreText()
    {
        TextMeshProUGUI highscoreText = GetComponent<TextMeshProUGUI>();
        int highscore = PlayerPrefs.GetInt("Highscore");
        if (GameManager.instance &&
            GameManager.instance.startedGame &&
            GameManager.instance.score > highscore)
        {
            PlayerPrefs.SetInt("Highscore", (GameManager.instance.score));
            highscoreText.text = "Personal highscore - " + GameManager.instance.score.ToString("000000000");
        }
        else
        {
            highscoreText.text = "Personal highscore - " + highscore.ToString("000000000");
        }
    }
}
