using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiClientItem : MonoBehaviour
{
    public Text id;
    public Text username;

    public void Set(int id, string username)
    {
        this.id.text = id.ToString();
        this.username.text = username;
    }
}
