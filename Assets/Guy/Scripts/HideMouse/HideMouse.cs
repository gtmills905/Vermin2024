using UnityEngine;

public class HideMouse : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = false; // Hide the mouse cursor
    }

    // Optionally, you can add functionality to toggle the cursor visibility back on
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Example key to toggle cursor visibility
        {
            Cursor.visible = !Cursor.visible; // Toggle the cursor visibility
        }
    }
}
