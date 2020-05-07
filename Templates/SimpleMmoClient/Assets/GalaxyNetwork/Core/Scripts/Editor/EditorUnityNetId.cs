using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityNetEntity))]
public class EditorUnityNetId : Editor
{

    GUISkin skin;
    Rect mainBox;
    void OnEnable()
    {
        skin = Resources.Load<GUISkin>("GalaxyNetworkGUI");
        mainBox = new Rect(0, 0, 0, 0);
 
    }

    // OnInspector GUI
    public override void OnInspectorGUI() //2
    {
        mainBox.width = (Screen.width-5);
        mainBox.height = 200;

        UnityNetEntity item = (UnityNetEntity)target; //1
         
      //  GUILayout.BeginArea(mainBox, skin.window);
        GUILayout.Label("", skin.GetStyle("MiniLogo"));
        //3      
        GUILayout.Label("Galaxy Net Object", EditorStyles.boldLabel); //3      
        GUILayout.Label("Net ID: " + item.netEntity.netID, EditorStyles.largeLabel);
        if (item.netEntity.isInit)
        {
            if (item.netEntity.isMy)
            {
                GUILayout.Label("Owner: You", EditorStyles.largeLabel);
            } else
            {
                if(item.netEntity.ownerClientId == 0)
                {
                    GUILayout.Label("Owner: Server", EditorStyles.largeLabel);
                } else
                {
                    GUILayout.Label("Owner: client:" + item.netEntity.ownerClientId, EditorStyles.largeLabel);
                }
            }
        }
        if(item.netEntity.isInit) GUILayout.Label("Live time: " + (int)(Time.time - item.initTime), EditorStyles.largeLabel);


    //    GUILayout.EndArea();
    //    GUILayout.Space(200);
    }
}