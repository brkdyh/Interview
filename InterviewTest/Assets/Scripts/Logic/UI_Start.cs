using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MessageSystem;

public class UI_Start : UIBase ,IMessageSystemHandler
{
    public Button Btn_StartGame;

    string IMessageSystemHandler.getMessageUid => "UI_Start";


    void IMessageSystemHandler.initHandleMethodMap(Dictionary<string, MessageHandleMethod> HandleMethodMap)
    {
        HandleMethodMap.Add("handleGameStart", handleGameStart);
    }

    public void Awake()
    {
        Btn_StartGame = transform.Find("Button").GetComponent<Button>();
        Btn_StartGame.onClick.AddListener(clickStartGame);
        MessageCore.RegisterHandler(this);
    }

    private void OnDestroy()
    {
        MessageCore.UnregisterHandler(this);
    }

    public override void OnView()
    {
        base.OnView();
    }

    public override void OnDisView()
    {
        base.OnDisView();
    }

    void clickStartGame()
    {
        MessageCore.SendMessage("GameManager", "handleGameStart", "开始游戏！");
    }

    void handleGameStart(object[] msg_params)
    {
        CloseSelf(true);
    }
}
