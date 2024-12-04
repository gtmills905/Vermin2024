using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public TMP_Text overheatedMessage;
    public Slider weaponTempSlider;

    public static UiController instance;

    private void Awake()
    {
        instance = this;
    }

}
