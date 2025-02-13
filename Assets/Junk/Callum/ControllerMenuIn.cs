using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ControllerMenuIn : MonoBehaviour
{

    public GameObject[] SelectionArray;
    int currentSelected = 0;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(SelectionArray[currentSelected], null);
    }

    void Update()
    {
        Debug.Log("CurrentSelect " + currentSelected + "Array is " + SelectionArray[currentSelected]);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelected--;

            if (currentSelected < 0)
            {
                currentSelected = 0;
            }
            EventSystem.current.SetSelectedGameObject(SelectionArray[currentSelected], null);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelected++;

            if (currentSelected >= SelectionArray.Length - 1)
            {
                currentSelected = SelectionArray.Length - 1;
            }
            EventSystem.current.SetSelectedGameObject(SelectionArray[currentSelected], null);
        }
    }
}

