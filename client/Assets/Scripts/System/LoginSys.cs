/*-----------------------------------------------------
    文件：LoginSys.cs
	作者：Johnson
    日期：2023/4/23 16:41:56
	功能：登陆注册业务系统
------------------------------------------------------*/

using PEProtocol;
using UnityEngine;

public class LoginSys : SystemRoot {
    public static LoginSys Instance = null;
    public LoginWnd loginWnd;
    public CreateWnd createWnd;
    public override void InitSys() {
        base.InitSys();

        Instance = this;
        PECommon.Log("Init LoginSys...");
    }


    /// <summary>
    /// 进入登陆场景
    /// </summary>
    public void EnterLogin() {

        //异步加载登陆场景
        //显示加载的进度
        //ResSvc.Instance.AsyncLoadScene(Constants.SceneLogin, OpenLoginWnd);
        resSvc.AsyncLoadScene(Constants.SceneLogin, () => {    //使用匿名函数
            //加载完成后再打开登录注册界面
            loginWnd.SetWndState(true);
            //播放背景音乐
            audioSvc.PlayBGMusic(Constants.BGDesertedVillage);
        });
    }

    public void RspLogin(GameMsg msg) {
        //GameRoot.AddTips("登陆成功");
        GameRoot.Instance.SetPlayerData(msg.rspLogin);

        if(msg.rspLogin.playerData.name == "") {
            //打开角色创建界面
            createWnd.SetWndState();
        }
        else {
            MainCitySys.Instance.EnterMainCity();
        }
        //关闭登陆界面
        loginWnd.SetWndState(false);
    }

    public void RspRename(GameMsg msg) {
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);

        //跳转场景进入主城
        MainCitySys.Instance.EnterMainCity();

        //关闭创建界面
        createWnd.SetWndState(false);
    }
}