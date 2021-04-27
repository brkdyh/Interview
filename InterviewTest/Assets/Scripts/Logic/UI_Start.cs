using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MessageSystem;

public class UI_Start : UIBase
{
    public Button Btn_StartGame;

    public void Awake()
    {
        Btn_StartGame.onClick.AddListener(clickStartGame);
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
        CloseSelf(true);
        MessageCore.SendMessage("GameManager", "handleGameStart", "开始游戏！");
    }
}
