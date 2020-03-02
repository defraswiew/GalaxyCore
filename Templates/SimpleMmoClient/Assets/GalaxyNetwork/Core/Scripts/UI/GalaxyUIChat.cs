using GalaxyCoreCommon;
using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GalaxyUIChat : MonoBehaviour
{
    public string sysName = "Система";
    public string instanseName = "Комната";
    public Color sysColor;
    public GameObject window;
    public GameObject mini;
    public Transform content;
    public InputField field;
    public Dropdown dropdown;
    public Text info;
    public GalaxyUIChatMessage messagePref;
    private Dictionary<string, ChenallItem> chenalls = new Dictionary<string, ChenallItem>();
    private ChenallItem currentChenall;

    void OnEnable()
    { 
        GalaxyApi.chat.OnChatMessage += OnChatMessage;
        GalaxyEvents.OnGalaxyEnterInInstance += OnGalaxyEnterInInstance;
        GalaxyEvents.OnGalaxyIncomingClient += OnGalaxyIncomingClient;
        GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;        
        dropdown.onValueChanged.AddListener(ChangeChenall);
        field.onValueChanged.AddListener(ChangeField);
    }
 
    private void AddCHenall(ChenallItem chenall)
    {
        chenalls.Add(chenall.name, chenall);
        dropdown.options.Add(new Dropdown.OptionData(chenall.name));
        field.interactable = true;
        if (chenalls.Count == 1) ChangeChenall(0);
    }

    private void ChangeField(string text)
    {
        if (currentChenall == null) field.interactable = false;
    }

    void OnDisable()
    {
        GalaxyApi.chat.OnChatMessage -= OnChatMessage;
        GalaxyEvents.OnGalaxyEnterInInstance -= OnGalaxyEnterInInstance;
        GalaxyEvents.OnGalaxyIncomingClient -= OnGalaxyIncomingClient;
        GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        dropdown.onValueChanged.RemoveListener(ChangeChenall);
        field.onValueChanged.RemoveListener(ChangeField);
    }

    void ChangeChenall(int id)
    {     
        currentChenall = chenalls[dropdown.options[id].text];
        info.text = "[" + currentChenall.name + "]";
    }

    private void OnGalaxyConnect(byte[] message)
    {
        Draw(false);
          ChenallItem chenall = new ChenallItem();
        chenall.name = "Мир";
        chenall.chatMessageType = ChatMessageType.all;
        AddCHenall(chenall);
    }
    public void Draw(bool visible)
    {
        window.SetActive(visible);
        mini.SetActive(!visible);
    }

    private void OnGalaxyIncomingClient(RemoteClient client)
    {
        SetItemInWondow("<color=#" + ColorUtility.ToHtmlStringRGB(sysColor) + "><b>[" + sysName + "]</b>: К нам присоеденился " + client.name + "</color>");
    }

    private void OnGalaxyEnterInInstance(InstanceInfo info)
    {       
        SetItemInWondow("<color=#"+ ColorUtility.ToHtmlStringRGB(sysColor) + "><b>[" + sysName + "]</b>: Вход в инстанс " + info.name + "</color>");
        if (chenalls.ContainsKey(instanseName)) return;
        ChenallItem chenall = new ChenallItem();
        chenall.name = instanseName;
        chenall.chatMessageType = ChatMessageType.instance;
        AddCHenall(chenall);

    }

    private void OnChatMessage(MessChat message)
    {
        switch (message.Type)
        {         
            case ChatMessageType.all:
                SetItemInWondow("<b>[Мир] " + message.name + "</b>:" + message.text);
                break;
            case ChatMessageType.privateMessage:
                break;
            case ChatMessageType.instance:
                SetItemInWondow("<b>[" + instanseName + "] "+ message.name+ "</b>:" + message.text);
                break;
            case ChatMessageType.group:
                break;
            default:
                break;
        }
      
       
    }

    private void SetItemInWondow(string text)
    {
        GalaxyUIChatMessage item = Instantiate(messagePref, content);
        item.SetMessage(text);
    }

    public void Send()
    {
        if (field.text.Length < 3) return;

        switch (currentChenall.chatMessageType)
        {
          
            case ChatMessageType.all:
                GalaxyApi.chat.SendAll(field.text);
                break;
            case ChatMessageType.privateMessage:
                GalaxyApi.chat.SendToClient(currentChenall.target,field.text);
                break;
            case ChatMessageType.instance:
                GalaxyApi.chat.SendToInstanse(field.text);
                break;
            case ChatMessageType.group:
                GalaxyApi.chat.SendGroup(currentChenall.target, field.text);
                break;
            default:
                break;
        }      
        field.text = "";
    }

}

public class ChenallItem
{    
    public ChatMessageType chatMessageType;
    public string name;
    public int target;
}
