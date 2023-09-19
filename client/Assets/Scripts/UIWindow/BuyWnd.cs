/*-----------------------------------------------------
    文件：BuyWnd.cs
	作者：Johnson
    日期：2023/7/28 16:0:10
	功能：购买交易窗口
------------------------------------------------------*/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class BuyWnd : WindowRoot {
    public Text txtInfo;
    public Button btnSure;

    private int buyType;    //0:体力 1:金币

    public void SetBuyType(int type) {
        this.buyType = type;
    }

    protected override void InitWnd() {
        base.InitWnd();

        Camera.main.GetComponent<CameraRotateAround>().isMouseControlEnabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Camera.main.GetComponent<CameraRotateAround>().isWndOpen = true;

        btnSure.interactable = true;
        RefreshUI();
    }

    private void RefreshUI() {
        switch (buyType) {
            case 0:
                //体力
                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("100体力", TxtColor.Green) + "?";
                break;
            case 1:
                //金币
                txtInfo.text = "是否花费" + "10别针" + "购买" + "1000金币" + "?";
                break;
        }
    }

    public void ClickSureBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        //发送网络消息
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.ReqBuy,
            reqBuy = new ReqBuy {
                type = buyType,
                cost = 10,
            }
        };

        netSvc.SendMsg(msg);
        btnSure.interactable = false;   //关闭交互，防止网络延迟导致重复购买
    }

    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }
}