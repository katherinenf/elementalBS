using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnHider : MonoBehaviour
{
    public Text turnHiderTitle;
    public GameObject clickText;
    public CircleProgress circleProgress;
    private bool wasClicked;

    public IEnumerator ShowUntilClick(string text)
    {
        turnHiderTitle.text = text;
        gameObject.SetActive(true);
        clickText.SetActive(true);
        circleProgress.gameObject.SetActive(false);
        wasClicked = false;
        while (!wasClicked)
        {
            yield return null;
        }
        Input.ResetInputAxes();
        gameObject.SetActive(false);
    }

    public IEnumerator ShowForSeconds(string text, float seconds)
    {
        turnHiderTitle.text = text;
        gameObject.SetActive(true);
        clickText.SetActive(false);
        circleProgress.gameObject.SetActive(true);
        float remaining = seconds;
        while (remaining >= 0f)
        {
            circleProgress.SetProgress(remaining / seconds);
            remaining -= Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // called when player clicks turn hider before turn begins
    public void OnClick()
    {
        wasClicked = true;
    }
}
