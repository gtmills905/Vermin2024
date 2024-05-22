using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonCycler : MonoBehaviour
{
    public List<TMP_Text> buttons;
    private int currentIndex = 0;

    void Start()
    {
        if (buttons.Count > 0)
        {
            SelectButton(currentIndex);
        }
    }

    void Update()
    {
        // Check for input from the vertical axis of the left analog stick
        float verticalInput = Input.GetAxis("Vertical");

        // Threshold to avoid accidental inputs
        if (verticalInput > 0.5f)
        {
            currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
            SelectButton(currentIndex);
        }
        else if (verticalInput < -0.5f)
        {
            currentIndex = (currentIndex + 1) % buttons.Count;
            SelectButton(currentIndex);
        }

        // Check for input from the Submit button (e.g., Space or Enter key)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteEvents.Execute(buttons[currentIndex].gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    private void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }
}
