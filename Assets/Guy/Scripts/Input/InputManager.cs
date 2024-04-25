using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Singleton instance
    private static InputManager instance;

    // Getter for singleton instance
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "InputManager";
                    instance = obj.AddComponent<InputManager>();
                }
            }
            return instance;
        }
    }

    // Dictionary to store axis names
    private Dictionary<string, string> axisNames = new Dictionary<string, string>();

    // Dictionary to store button names
    private Dictionary<string, string> buttonNames = new Dictionary<string, string>();

    // Method to set axis name
    public void SetAxisName(string axis, string name)
    {
        axisNames[axis] = name;
    }

    // Method to set button name
    public void SetButtonName(string button, string name)
    {
        buttonNames[button] = name;
    }

    // Method to get axis value
    public float GetAxis(string axis)
    {
        return Input.GetAxis(axisNames[axis]);
    }

    // Method to check if button is pressed
    public bool GetButtonDown(string button)
    {
        return Input.GetButtonDown(buttonNames[button]);
    }

    // Other input-related methods can be added as needed
}
