using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class introSequence : MonoBehaviour
{
    [TextArea(0, 30)]
    public string [] messages;

    public float duration;
    public float waitBetweenMessages;

    public TMP_Text textOut;

    private bool canStart;
    private float timer;
    private int currIndex;
    private int globalIndex;

    void Start()
    {
        globalIndex = 0;
        currIndex = 1;
        textOut.text = "";
    }


    void Update()
    {
        if (canStart)
        {
            if (Time.time - timer >= duration)
            {
                if (globalIndex <= messages.Length)
                    textOut.text = messages[globalIndex].Substring(0, currIndex++);

                if (currIndex >= messages[globalIndex].Length)
                {
                    currIndex = 0;
                    globalIndex++;
                }

                //reset timer
                timer = Time.time;
            }
        }
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void startAnimation()
    {
        timer = Time.time;
        canStart = true;
    }
}
