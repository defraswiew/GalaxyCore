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


    public override void OnInspectorGUI()
    {
        mainBox.width = (Screen.width - 5);
        mainBox.height = 200;

        UnityNetEntity item = (UnityNetEntity)target;
        GUILayout.Label("", skin.GetStyle("MiniLogo"));
        GUILayout.Label("Galaxy Net Object", EditorStyles.boldLabel);
        GUILayout.Label("Net ID: " + item.netEntity.netID, EditorStyles.largeLabel);
        if (item.netEntity.isInit)
        {
            if (item.netEntity.isMy)
            {
                GUILayout.Label("Owner: You", EditorStyles.largeLabel);
            }
            else
            {
                if (item.netEntity.ownerClientId == 0)
                {
                    GUILayout.Label("Owner: Server", EditorStyles.largeLabel);
                }
                else
                {
                    GUILayout.Label("Owner: client:" + item.netEntity.ownerClientId, EditorStyles.largeLabel);
                }
            }
        }
        if (item.netEntity.isInit) GUILayout.Label("Live time: " + (int)(Time.time - item.initTime), EditorStyles.largeLabel);
    }
}