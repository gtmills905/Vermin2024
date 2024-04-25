using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int player1Score;
    public int player2Score;

    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }
}
