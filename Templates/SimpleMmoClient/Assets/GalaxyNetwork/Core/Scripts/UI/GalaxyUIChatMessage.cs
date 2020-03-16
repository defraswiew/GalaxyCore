using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyUIChatMessage : MonoBehaviour
{
    private LayoutElement layout;
    private Text message;
    public void SetMessage(string text)
    {
        layout = GetComponent<LayoutElement>();
        message = GetComponentInChildren<Text>();
        message.text = text;
        layout.preferredHeight = message.preferredHeight+7;
        layout.preferredWidth = message.preferredWidth + 20;
    }
}
