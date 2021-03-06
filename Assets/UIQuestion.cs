using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIQuestion : MonoBehaviour
{
    public Text questionText;
    public Image background;
    public AnswerInputSpinner spinner;
    public Sprite disabledBackground;
    public GameObject correctIcon;
    public GameObject wrongIcon;

    public int value
    {
        get { return spinner.value; }
    }

    public void Clear()
    {
        // Clear text
        questionText.text = "-";

        // Prevent input
        spinner.disabled = true;

        // Gray out background
        background.overrideSprite = disabledBackground;

        // Hide icons
        correctIcon.SetActive(false);
        wrongIcon.SetActive(false);
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

    public void ShowAnswer(bool userCorrect)
    {
        if (userCorrect)
        {
            correctIcon.SetActive(true);
        }
        else
        {
            wrongIcon.SetActive(true);
        }
    }

    // Make the "selected" input field (focus)
    public void Select()
    {
        //if (!spinner.inputField.isFocused)
        //{
            spinner.inputField.Select();
            spinner.inputField.ActivateInputField();
        //}
    }
}
