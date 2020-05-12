using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyUiClientItem : MonoBehaviour
{
    public Text label;

    public void Init(int id, string login)
    {
        label.text = "(Id:" + id + ") " + login;
    }
}
