using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GalaxyNetwork.Core
{
    public class GalaxyDragWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

        public Transform MainObj;
        private Vector2 _offsetPos;
        private Outline _outline;
        public void OnBeginDrag(PointerEventData eventData)
        {
            _offsetPos = new Vector2(MainObj.position.x - eventData.pressPosition.x, MainObj.position.y - eventData.pressPosition.y);
            _outline = gameObject.AddComponent<Outline>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MainObj.position = new Vector2(eventData.position.x + _offsetPos.x, eventData.position.y + _offsetPos.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(_outline);
        }
    }
}
