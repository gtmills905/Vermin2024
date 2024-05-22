using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ConInputUpdated : MonoBehaviour
{
    public GameObject[] SelectionArray;
    private int currentSelected = 0;
    private MenuControls controls;
    private Vector2 navigationInput;
    private float inputCooldown = 0.5f; // Adjust the cooldown period if necessary
    private float lastInputTime = 0f;

    void Awake()
    {
        controls = new MenuControls();

        // Set up callback for Navigate action
        controls.UI.Navigate.performed += ctx => navigationInput = ctx.ReadValue<Vector2>();
        controls.UI.Navigate.canceled += ctx => navigationInput = Vector2.zero;
    }

    void OnEnable()
    {
        controls.UI.Enable();
    }

    void OnDisable()
    {
        controls.UI.Disable();
    }

    void Start()
    {
        if (SelectionArray.Length > 0)
        {
            EventSystem.current.SetSelectedGameObject(SelectionArray[currentSelected], null);
        }
    }

    void Update()
    {
        HandleNavigation();
    }

    private void HandleNavigation()
    {
        if (Time.time - lastInputTime < inputCooldown)
        {
            return;
        }

        if (navigationInput.y > 0.5f)
        {
            NavigateUp();
        }
        else if (navigationInput.y < -0.5f)
        {
            NavigateDown();
        }
    }

    private void NavigateUp()
    {
        lastInputTime = Time.time;
        currentSelected--;

        if (currentSelected < 0)
        {
            currentSelected = SelectionArray.Length - 1;
        }

        SetSelectedGameObject();
    }

    private void NavigateDown()
    {
        lastInputTime = Time.time;
        currentSelected++;

        if (currentSelected >= SelectionArray.Length)
        {
            currentSelected = 0;
        }

        SetSelectedGameObject();
    }

    private void SetSelectedGameObject()
    {
        EventSystem.current.SetSelectedGameObject(SelectionArray[currentSelected], null);
    }
}
