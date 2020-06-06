using UnityEngine;
using UnityEngine.UI;

public class CircleProgress : MonoBehaviour
{
    public Image fill;

    public void SetProgress(float value)
    {
        fill.fillAmount = Mathf.Clamp(value, 0f, 1f);
    }
}
