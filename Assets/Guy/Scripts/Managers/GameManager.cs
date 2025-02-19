using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun; // For PUN 2
using Photon.Realtime; // Optional for room/lobby management


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

        // Send updated scores to MatchManager via Photon
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            MatchManager.Instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 0, birdScore); // Bird score as kills
            MatchManager.Instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, BirdLives); // Farmer score as deaths
        }

        
    }

    // Call this method when a bird deposits an object
    public void DepositObject(int points)
    {
        birdScore += points;
        MatchManager.Instance.UpdateDepositsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1);
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
