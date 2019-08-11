using System;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestion : MonoBehaviour
{
    public Text questionText;
    public Image background;
    public AnswerInputSpinner spinner;
    public Sprite disabledBackground;

    public void Clear()
    {
        questionText.text = "-";
        spinner.disabled = true;
        background.overrideSprite = disabledBackground;
    }

    public void Load(Element e, IQuestion question)
    {
        // Setup the question text
        questionText.text = question.GetQuestionText(e);

        // Setup the input widget
        Vector2Int range = question.GetAnswerRange();
        spinner.disabled = false;
        spinner.minValue = range.x;
        spinner.maxValue = range.y;
        spinner.value = 1;

        // Clear the disabled background override
        background.overrideSprite = null;
    }
}
