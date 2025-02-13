using UnityEngine;
using UnityEngine.UI;


public class QuitButton : MonoBehaviour
{
    public Button quitButton;

    void Start()
    {
        // Add a listener to the button's click event.
        quitButton.onClick.AddListener(QuitGame);
    }

    public void QuitGame()
    {
        // Quit the application.
        Application.Quit();
    }
}
