using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageSystem;

public class GameManager : MonoSingleton<GameManager>, IMessageSystemHandler
{
    string IMessageSystemHandler.getMessageUid => "GameManager";

    void IMessageSystemHandler.initHandleMethodMap(Dictionary<string, MessageHandleMethod> HandleMethodMap)
    {
        HandleMethodMap.Add("handleGameStart", handleGameStart);
    }

    public override void Awake()
    {
        base.Awake();
        MessageCore.RegisterHandler(this);
    }

    private void OnDestroy()
    {
        MessageCore.UnregisterHandler(this);
    }

    void Start()
    {
        UIMgr.Instance.OpenUI<UI_Start>();
    }

    void handleGameStart(object[] msg_params)
    {
        Debug.Log(msg_params[0] as string);
    }
}
