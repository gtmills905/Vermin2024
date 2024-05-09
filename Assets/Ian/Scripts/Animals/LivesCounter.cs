using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesCounter : MonoBehaviour
{
    public int LivesLeft;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LivesLeft >= 0) 
        {
            SceneManager.LoadScene("WinScene");// farmer wins
        }
    }
}
