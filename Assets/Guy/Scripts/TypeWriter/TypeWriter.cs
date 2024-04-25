using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class TypewWriter : MonoBehaviour
{
    public float delay = 0.1f; // Delay between each character
    private string fullText;
    private string currentText = "";
    private int textIndex = 0;
    private TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        fullText = textComponent.text;
        textComponent.text = currentText;
        StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
        while (textIndex < fullText.Length)
        {
            currentText += fullText[textIndex];
            textComponent.text = currentText;
            textIndex++;
            yield return new WaitForSeconds(delay);
        }
    }
}
