using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GalaxyDragWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Transform MainObj;
    Vector2 offsetPos;
    Outline ou;
    public void OnBeginDrag(PointerEventData eventData)
    {
        offsetPos = new Vector2(MainObj.position.x - eventData.pressPosition.x, MainObj.position.y - eventData.pressPosition.y);
        ou = this.gameObject.AddComponent<Outline>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MainObj.position = new Vector2(eventData.position.x + offsetPos.x, eventData.position.y + offsetPos.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(ou);
    }

}
