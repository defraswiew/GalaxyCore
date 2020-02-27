using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityNetEntity))]
public class EditorUnityNetId : Editor
{

  

    // OnInspector GUI
    public override void OnInspectorGUI() //2
    {

        UnityNetEntity item = (UnityNetEntity)target; //1
        
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
        GUILayout.Label("Live time: " + (int)(Time.time - item.initTime), EditorStyles.largeLabel);
       
    }
}