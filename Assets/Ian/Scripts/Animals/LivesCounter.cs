using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LivesCounter : MonoBehaviour
{
    public float LivesLeft;
    public TMP_Text currentLives;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentLives.text = "Lives: " + LivesLeft;
    
        if (LivesLeft >= 0) 
        {
            SceneManager.LoadScene("WinScene");
        }
    }
}
