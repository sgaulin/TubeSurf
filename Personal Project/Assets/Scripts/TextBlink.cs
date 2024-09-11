using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBlink : MonoBehaviour
{
    [SerializeField] private float delay;
    public bool isActive = false;

    private TextMeshProUGUI text;


    void Start()
    {
        text= GetComponent<TextMeshProUGUI>();
        StartBlinking();

    }

    IEnumerator Blink()
    {
        while(isActive) 
        {
            switch (text.color.a.ToString()) 
            {
                case "0":
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
                    yield return new WaitForSeconds(delay);
                    break;
                case "1":
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
                    yield return new WaitForSeconds(delay);
                    break;

            }
        }
    }

    private void StartBlinking() 
    {
        StopCoroutine("Blink");
        StartCoroutine("Blink");

    }

    private void StopBlinking() 
    {
        StopCoroutine("Blink");

    }
}
