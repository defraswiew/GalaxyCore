using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEditor;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts.Editor
{
    [CustomEditor(typeof(UnityNetEntity))]
    public class EditorUnityNetId : UnityEditor.Editor
    {
        private GUISkin _skin;
        private Rect _mainBox;
        void OnEnable()
        {
            _skin = Resources.Load<GUISkin>("GalaxyNetworkGUI");
            _mainBox = new Rect(0, 0, 0, 0);
        }


        public override void OnInspectorGUI()
        {
            _mainBox.width = (Screen.width - 5);
            _mainBox.height = 200;

            UnityNetEntity item = (UnityNetEntity)target;
            GUILayout.Label("", _skin.GetStyle("MiniLogo"));
            GUILayout.Label("Galaxy Net Object", EditorStyles.boldLabel);
            GUILayout.Label("Net ID: " + item.NetEntity.NetID, EditorStyles.largeLabel);
            if (item.NetEntity.IsInit)
            {
                if (item.NetEntity.IsMy)
                {
                    GUILayout.Label("Owner: You", EditorStyles.largeLabel);
                }
                else
                {
                    if (item.NetEntity.OwnerClientId == 0)
                    {
                        GUILayout.Label("Owner: Server", EditorStyles.largeLabel);
                    }
                    else
                    {
                        GUILayout.Label("Owner: client:" + item.NetEntity.OwnerClientId, EditorStyles.largeLabel);
                    }
                }
            }
            if (item.NetEntity.IsInit) GUILayout.Label("Live time: " + (int)(Time.time - item.InitTime), EditorStyles.largeLabel);
        }
    }
}