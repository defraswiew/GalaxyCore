using UnityEditor;
using UnityEngine;
using GalaxyCoreLib;

[CustomEditor(typeof(GalaxyNetworkController))]
public class EditorNetworkController : Editor
{
    public override void OnInspectorGUI()
    {
        GalaxyNetworkController item = (GalaxyNetworkController)target;
        GUILayout.Label("Galaxy Network Controller", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Settings", GUILayout.ExpandWidth(true));

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Server IP: ");
        item.serverIP = EditorGUILayout.TextField(item.serverIP);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Server port: ");
        item.serverPort = EditorGUILayout.IntField(item.serverPort);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        if (GalaxyApi.myId != 0) GUILayout.Label("Client ID: " + GalaxyApi.myId, EditorStyles.largeLabel);

        GUILayout.Space(10);
        if (GalaxyApi.connection != null)
        {
            if (GalaxyApi.connection.isConnected)
            {
                GUILayout.Label("Statictic", EditorStyles.boldLabel);


                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Messages");
                GUILayout.Label("In: " + GalaxyApi.connection.statistic.inMessages);
                GUILayout.Label("Out: " + GalaxyApi.connection.statistic.outMessages);
                GUILayout.Label("           ");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Traffic");
                GUILayout.Label("In: " + GalaxyApi.connection.statistic.inTraffic);
                GUILayout.Label("Out: " + GalaxyApi.connection.statistic.outTraffic);
                GUILayout.Label("           ");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Traffic");
                GUILayout.Label("In: " + GalaxyApi.connection.statistic.inTrafficInSecond);
                GUILayout.Label("Out: " + GalaxyApi.connection.statistic.outTrafficInSecond);
                GUILayout.Label("           ");
                GUILayout.EndHorizontal();

                GUILayout.Label("Ping: " + GalaxyApi.connection.statistic.ping * 500);
            }
        }


        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Отображать NetEntity");
        item.drawLables = GUILayout.Toggle(item.drawLables, "");

        GUILayout.EndHorizontal();

    }

}