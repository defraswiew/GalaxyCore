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
            if(item.MainConnection == null)
                return;
            if (item.MainConnection.Api.MyId != 0) GUILayout.Label("Client ID: " + item.MainConnection.Api.MyId, EditorStyles.largeLabel);

            GUILayout.Space(10);
            if (item.MainConnection.Api.Transport != null)
            {
                if (item.MainConnection.Api.Transport.IsConnected)
                {
                    GUILayout.Label("Statictic", EditorStyles.boldLabel);


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Messages");
                    GUILayout.Label("In: " + item.MainConnection.Api.Transport.Statistic.InMessages);
                    GUILayout.Label("Out: " + item.MainConnection.Api.Transport.Statistic.OutMessages);
                    GUILayout.Label("           ");
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Traffic");
                    GUILayout.Label("In: " + item.MainConnection.Api.Transport.Statistic.InTraffic);
                    GUILayout.Label("Out: " + item.MainConnection.Api.Transport.Statistic.OutTraffic);
                    GUILayout.Label("           ");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Traffic");
                    GUILayout.Label("In: " + item.MainConnection.Api.Transport.Statistic.InTrafficInSecond);
                    GUILayout.Label("Out: " + item.MainConnection.Api.Transport.Statistic.OutTrafficInSecond);
                    GUILayout.Label("           ");
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Ping: " + item.MainConnection.Api.Transport.Statistic.Ping * 500);
                }
            }


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Отображать NetEntity");
            item.DrawLabels = GUILayout.Toggle(item.DrawLabels, "");

            GUILayout.EndHorizontal();

        }

    }
}