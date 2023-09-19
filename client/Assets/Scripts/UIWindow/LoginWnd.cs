/*-----------------------------------------------------
    文件：LoginWnd.cs
	作者：Johnson
    日期：2023/4/25 19:43:52
	功能：登录注册界面
------------------------------------------------------*/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class LoginWnd : WindowRoot {
    public InputField iptAcct;
    public InputField iptPass;
    public Button btnEnter;
    public Button btnNotice;

    protected override void InitWnd() {
        base.InitWnd();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //获取本地存储的账号密码
        if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pass")) {
            iptAcct.text = PlayerPrefs.GetString("Acct");
            iptPass.text = PlayerPrefs.GetString("Pass");
        }
        else {
            iptAcct.text = "";
            iptPass.text = "";
        }
    }

    /// <summary>
    /// 点击进入游戏
    /// </summary>
    public void ClickEnterBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        string _acct = iptAcct.text;
        string _pass = iptPass.text;
        if(_acct != "" && _pass != "") {
            //更新本地存储的账号密码
            PlayerPrefs.SetString("Acct", _acct);
            PlayerPrefs.SetString("Pass", _pass);

            //TODO 发送网络消息，请求登录
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin {
                    acct = _acct,
                    pass = _pass
                }
            };
            netSvc.SendMsg(msg);
        }
        else {
            GameRoot.AddTips("账号或密码为空");
        }
    }

    public void ClickNoticeBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        GameRoot.AddTips("功能正在开发中...");
    }
}