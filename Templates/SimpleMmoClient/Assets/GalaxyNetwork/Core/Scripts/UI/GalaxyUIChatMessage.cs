using System;
using GalaxyCoreCommon.InternalMessages;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyNetwork.Core.Scripts.UI
{
    public class GalaxyUIChatMessage : MonoBehaviour
    {
        [SerializeField] 
        private Text _time;
        [SerializeField] 
        private Text _name;
        [SerializeField] 
        private Text _text;
        [SerializeField] 
        private Button _button;

        private Action<ChatMessage> _onClickAction;

        private ChatMessage _message;
        public void SetMessage(ChatMessage message, Action<ChatMessage> OnClickAction)
        {
            _message = message;
            var time = UnixTimeToDateTime(message.Time);
            var stringTime = time.Hour + ":" + time.Minute;
            _time.text = stringTime;
            _name.text = message.SenderName;
            _text.text = message.Text;
            _button.onClick.AddListener(OnClick);
            _onClickAction = OnClickAction;
        }

        private DateTime UnixTimeToDateTime(long unixTime)
        {
            DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Unspecified);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
            return dtDateTime;
        }

        private void OnClick()
        {
            _onClickAction?.Invoke(_message);
        }
    }
}
