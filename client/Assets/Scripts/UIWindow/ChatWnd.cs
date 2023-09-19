/*-----------------------------------------------------
    文件：ChatWnd.cs
	作者：Johnson
    日期：2023/7/25 23:36:25
	功能：聊天界面
------------------------------------------------------*/

using PEProtocol;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatWnd : WindowRoot {
    public InputField iptChat;
    public Text txtChat;
    public Image imgWorld;
    public Image imgGuild;
    public Image imgFriend;

    private int chatType;
    private List<string> chatLst = new List<string>();

    protected override void InitWnd() {
        base.InitWnd();

        Camera.main.GetComponent<CameraRotateAround>().isMouseControlEnabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Camera.main.GetComponent<CameraRotateAround>().isWndOpen = true;

        chatType = 0;
        
        RefreshUI();

    }

    public void AddChatMsg(string name, string chat) {
        chatLst.Add(name + "：" + chat);
        if(chatLst.Count > 16) {
            chatLst.RemoveAt(0);
        }
        if (GetWndState()) {
            RefreshUI();    //聊天窗口可能还没打开过，没有初始化
        }
    }

    private void RefreshUI() {
        if(chatType == 0) {
            string chatMsg = "";
            for (int i = 0; i < chatLst.Count; i++) {
                chatMsg += chatLst[i] + "\n";
            }
            SetText(txtChat, chatMsg);

/*            SetSprite(imgWorld, "ResImages/btntype1");
            SetSprite(imgGuild, "ResImages/btntype2");
            SetSprite(imgFriend, "ResImages/btntype2");*/
        }
        else if(chatType == 1) {
            SetText(txtChat, "还没有加入公会");

/*            SetSprite(imgWorld, "ResImages/btntype2");
            SetSprite(imgGuild, "ResImages/btntype1");
            SetSprite(imgFriend, "ResImages/btntype2");*/
        }
        else if(chatType == 2) {
            SetText(txtChat, "暂无好友信息");

/*            SetSprite(imgWorld, "ResImages/btntype2");
            SetSprite(imgGuild, "ResImages/btntype2");
            SetSprite(imgFriend, "ResImages/btntype1");*/
        }
    }

    private bool canSend = true;
    public void ClickSendBtn() {
        if (!canSend) {
            GameRoot.AddTips("聊天消息每条需间隔5秒");
            return;
        }
        if(iptChat.text != null && iptChat.text != "" && iptChat.text != " ") {
            if(iptChat.text.Length > 12) {
                GameRoot.AddTips("输入信息不能超过12个字");
            }
            else {
                //发送网络消息到服务器
                GameMsg msg = new GameMsg {
                    cmd = (int)CMD.SndChat,
                    sndChat = new SndChat {
                        chat = iptChat.text
                    }
                };
                iptChat.text = "";
                netSvc.SendMsg(msg);
                canSend = false;

                /*//开启携程
                StartCoroutine(MsgTimer());*/
                timerSvc.AddTimeTask((int tid) => {
                    canSend = true;
                }, 5, PETimeUnit.Second);
            }
        }
        else {
            GameRoot.AddTips("尚未输入聊天信息");
        }
    }
    /*IEnumerator MsgTimer() {
        yield return new WaitForSeconds(5.0f);
        canSend = true;
    }*/

    public void ClickWorldBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 0;
        RefreshUI();
    }
    public void ClickGuildBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 1;
        RefreshUI();
    }
    public void ClickFriendBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 2;
        RefreshUI();
    }
    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 0;
        SetWndState(false);
    }
}