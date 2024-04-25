// WinSceneManager script
using UnityEngine;
using TMPro;

public class WinSceneManager : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    private void Start()
    {
        float currentScore = PlayerPrefs.GetFloat("CurrentScore");
        float highScore = PlayerPrefs.GetFloat("HighScore");

        currentScoreText.text = "Time: " + FormatTime(currentScore);

        if (highScore == Mathf.Infinity)
        {
            highScoreText.text = "Lowest Time: None";
        }
        else
        {
            highScoreText.text = "Lowest Time: " + FormatTime(highScore);
        }
    }

    private string FormatTime(float time)
    {
        if (time == Mathf.Infinity)
        {
            return "None";
        }
        else
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            int milliseconds = (int)((time * 1000) % 1000);
            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }
}
