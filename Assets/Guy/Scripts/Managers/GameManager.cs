using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Static instance field

    public TextMeshProUGUI birdScoreText;
    public TextMeshProUGUI farmerScoreText;
    public TextMeshProUGUI timerText;

    public int birdScore = 0;
    public int FarmerScore = 0;

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
        UpdateScoreText();

        // Update timer
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerText();


        }

        else
        {
            // If timer runs out (5 minutes elapsed), determine the winner

            if (birdScore > FarmerScore)
            {
                Debug.Log("Birds Win!");
                // You can add more logic here, such as displaying a win screen or triggering other game events.
            }
            else if (FarmerScore > birdScore)
            {
                Debug.Log("Farmers Win!");
                // You can add more logic here, such as displaying a win screen or triggering other game events.
            }
            else
            {
                Debug.Log("It's a tie!");
                // You can add more logic here for handling a tie scenario.
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
        FarmerScore += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        birdScoreText.text = "     Score: " + birdScore.ToString();
        farmerScoreText.text = "Score: " + FarmerScore.ToString();
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