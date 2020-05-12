using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyUiRoomInfo : MonoBehaviour
    {
        public GalaxyInstanceInfoScriptable myInfo;
    Image img;
        public void Click()
        {
            GalaxyUIRoomCreate.api.SelectedInfo(myInfo);
        }
    public void Init(GalaxyInstanceInfoScriptable info)
    {
        myInfo = info;
        img = GetComponent<Image>();
        img.sprite = info.img;
    }
    }
 
