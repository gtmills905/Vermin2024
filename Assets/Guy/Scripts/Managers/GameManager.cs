using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Static instance field

    public TextMeshProUGUI birdScoreText;
    public TextMeshProUGUI BirdLivesText;
    public TextMeshProUGUI timerText;

    public AudioSource givemebackmypigAudioSource;
    public AudioClip givemebackmypigSoundClip;

    public int birdScore = 0;
    public int BirdLives = 0;

    public DropOffSystem[] dropOffSystem;

    public int depositedAnimalCount = 0;

    public float timer = 300f; // 5 minutes for the first mode

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Another instance of GameManager already exists. Destroying this one.");
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        UpdateScoreText();
        UpdateTimerText();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        UpdateTimerText();
        UpdateScoreText();
        if(birdScore == 10)
        {
            SceneManager.LoadScene("Birds Win");
        }
        if(BirdLives == 10)
        {
            SceneManager.LoadScene("Farmer Win");
        }


        // Update timer
        if (timer <= 0f)
        {
            // Check if birdScore and BirdLives are complementary out of 10
            if (birdScore + BirdLives == 10)
            {
                SceneManager.LoadScene("Tie");
            }
            else
            {
                // If timer runs out (5 minutes elapsed), determine the winner
                if (birdScore > BirdLives)
                {
                    SceneManager.LoadScene("Birds Win 1");
                    // You can add more logic here, such as displaying a win screen or triggering other game events.
                }
                else if (BirdLives > birdScore)
                {
                    SceneManager.LoadScene("Farmer Win 1");
                    // You can add more logic here, such as displaying a win screen or triggering other game events.
                }
                else if (BirdLives <= 0)
                {
                    SceneManager.LoadScene("Farmer Win 1");
                    // You can add more logic here, such as displaying a win screen or triggering other game events.
                }
                else
                {
                    SceneManager.LoadScene("Tie 1");
                    // You can add more logic here, such as displaying a win screen or triggering other game events.
                }
            }
        }


    }

    // Call this method when a bird deposits an object
    public void DepositObject(int points)
    {
        birdScore += points;
        UpdateScoreText();
    }


    // Call this method when the farmer kills a bird
    public void FarmerKill(int points)
    {
        BirdLives += points;  // Increment Farmer's score or kills
        UpdateScoreText();  // Update the UI with the new score
    }



    void UpdateScoreText()
    {
        birdScoreText.text = "     Score: " + birdScore.ToString();
        BirdLivesText.text = "Lives Taken: " + BirdLives.ToString();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public int GetNumPlayers()
    {
        return 4;
    }
}