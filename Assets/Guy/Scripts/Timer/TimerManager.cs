// TimeManager script
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    private float currentTime;
    private float lowestTime = Mathf.Infinity;
    private bool gameEnded = false;

    private void Start()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            lowestTime = PlayerPrefs.GetFloat("HighScore");
            if (lowestTime == Mathf.Infinity)
            {
                highScoreText.text = "Lowest Time: None";
            }
            else
            {
                highScoreText.text = "Lowest Time: " + FormatTime(lowestTime);
            }
        }
        else
        {
            highScoreText.text = "Lowest Time: None";
        }
    }

    private void Update()
    {
        if (!gameEnded)
        {
            currentTime += Time.deltaTime;
            currentScoreText.text = "Current Time: " + FormatTime(currentTime);
        }
    }

    public void EndGame()
    {
        gameEnded = true;
        if (currentTime < lowestTime)
        {
            lowestTime = currentTime;
            highScoreText.text = "Lowest Time: " + FormatTime(lowestTime);
            SaveLowestTime();
        }
    }

    private void SaveLowestTime()
    {
        PlayerPrefs.SetFloat("HighScore", lowestTime);
        PlayerPrefs.SetFloat("CurrentScore", currentTime);
        PlayerPrefs.Save();
    }

    public void LoadWinScene()
    {
        SaveLowestTime();
        SceneManager.LoadScene("WinScene");
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
