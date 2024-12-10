using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public TMP_Text overheatedMessage;
    public Slider weaponTempSlider;

    public GameObject deathScreen;
    public TMP_Text deathText;


    public Slider healthSlider;
    public static UiController instance;

    private void Awake()
    {
        instance = this;
    }

}
