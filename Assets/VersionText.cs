using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
    void Start()
    {
        Text uiText = GetComponent<Text>();
        if (uiText != null)
        {
            uiText.text = "v" + Application.version;
        }
    }
}
