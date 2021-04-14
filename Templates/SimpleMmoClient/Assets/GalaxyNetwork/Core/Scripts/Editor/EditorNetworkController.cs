using GalaxyCoreLib;
using UnityEditor;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts.Editor
{
    [CustomEditor(typeof(GalaxyNetworkController))]
    public class EditorNetworkController : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GalaxyNetworkController item = (GalaxyNetworkController)target;
            GUILayout.Label("Galaxy Network Controller", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Settings", GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Server IP: ");
            item.ServerIP = EditorGUILayout.TextField(item.ServerIP);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Server port: ");
            item.ServerPort = EditorGUILayout.IntField(item.ServerPort);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (GalaxyApi.MyId != 0) GUILayout.Label("Client ID: " + GalaxyApi.MyId, EditorStyles.largeLabel);

            GUILayout.Space(10);
            if (GalaxyApi.Connection != null)
            {
                if (GalaxyApi.Connection.IsConnected)
                {
                    GUILayout.Label("Statictic", EditorStyles.boldLabel);


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Messages");
                    GUILayout.Label("In: " + GalaxyApi.Connection.Statistic.InMessages);
                    GUILayout.Label("Out: " + GalaxyApi.Connection.Statistic.OutMessages);
                    GUILayout.Label("           ");
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Traffic");
                    GUILayout.Label("In: " + GalaxyApi.Connection.Statistic.InTraffic);
                    GUILayout.Label("Out: " + GalaxyApi.Connection.Statistic.OutTraffic);
                    GUILayout.Label("           ");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Traffic");
                    GUILayout.Label("In: " + GalaxyApi.Connection.Statistic.InTrafficInSecond);
                    GUILayout.Label("Out: " + GalaxyApi.Connection.Statistic.OutTrafficInSecond);
                    GUILayout.Label("           ");
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Ping: " + GalaxyApi.Connection.Statistic.Ping * 500);
                }
            }


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Отображать NetEntity");
            item.DrawLabels = GUILayout.Toggle(item.DrawLabels, "");

            GUILayout.EndHorizontal();

        }

    }
}