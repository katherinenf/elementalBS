using System;
using UnityEngine;
using UnityEngine.UI;

public class AnswerInputSpinner : MonoBehaviour
{
    public InputField inputField;
    public Button incrementBtn;
    public Button decrementBtn;
    public int minValue;
    public int maxValue;

    protected bool _disabled;

    public int value
    {
        get {
            if (inputField != null)
            {
                try
                {
                    return Int32.Parse(inputField.text);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            return 0;
        }

        set {
            if (inputField != null)
            {
                inputField.text = value.ToString();
            }
        }
    }

    public bool disabled
    {
        get { return _disabled; }
        set {
            _disabled = value;
            if (_disabled)
            {
                if (inputField != null)
                {
                    inputField.text = "";
                    inputField.interactable = false;
                }
                if (incrementBtn != null)
                {
                    incrementBtn.interactable = false;
                }
                if (decrementBtn != null)
                {
                    decrementBtn.interactable = false;
                }
            }
            else
            {
                if (inputField != null)
                {
                    inputField.text = minValue.ToString();
                    inputField.interactable = true;
                }
                if (incrementBtn != null)
                {
                    incrementBtn.interactable = true;
                }
                if (decrementBtn != null)
                {
                    decrementBtn.interactable = true;
                }
            }
        }
    }

    public void Update()
    {
        if (inputField.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("TAB");
            Selectable next = inputField.FindSelectableOnDown();
            if (next != null)
            {
                Debug.Log("next");
                next.Select();
            }
        }
    }

    public void Increment()
    {
        if (!disabled && inputField != null)
        {
            inputField.text = Math.Min(Math.Max(value + 1, minValue), maxValue).ToString();
        }
    }

    public void Decrement()
    {
        if (!disabled && inputField != null)
        {
            inputField.text = Math.Min(Math.Max(value - 1, minValue), maxValue).ToString();
        }
    }

    public void InputValueChangeCheck()
    {
        if (!_disabled)
        {
            inputField.text = Math.Min(Math.Max(value, minValue), maxValue).ToString();
        }
    }
}
