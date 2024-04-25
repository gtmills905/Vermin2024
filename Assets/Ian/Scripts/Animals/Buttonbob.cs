using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBob : MonoBehaviour
{

    private bool CanRun = true;
    private float delayBetweenScales = 0.01f;
    Vector3 Scaleup = new Vector3(0.01f, 0.01f, 0.01f);

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerEnter()
    {
        CanRun = true;
        StartCoroutine(ScaleObject());
    }
    public void OnPointerExit()
    {
        CanRun = false;
        //Debug.Log("exit");
    }

    IEnumerator ScaleObject()
    {
        for (int i = 0; i < 27; i++)
        {
            transform.localScale += Scaleup;
            yield return new WaitForSeconds(delayBetweenScales);
            // Debug.Log("out");
        }

        for (int i = 0; i < 27; i++)
        {
            transform.localScale -= Scaleup;
            yield return new WaitForSeconds(delayBetweenScales);
            //Debug.Log("in");
        }
        if (CanRun == true)
        {
            StartCoroutine(ScaleObject());

        }
    }
}
