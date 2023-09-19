/*-----------------------------------------------------
    文件：GameRoot.cs
	作者：Johnson
    日期：2023/4/23 16:38:52
	功能：游戏启动入口
------------------------------------------------------*/

using PEProtocol;
using UnityEngine;

public class GameRoot : MonoBehaviour {
    public static GameRoot Instance = null;

    public LoadingWnd loadingWnd;
    public DynamicWnd dynamicWnd;
    private void Start() {
        Instance = this;

        //GameRoot在场景切换时或加载时不应该被销毁
        DontDestroyOnLoad(this);
        PECommon.Log("Game Start...");

        ClearUIRoot();

        Init();
    }
    
    /// <summary>
    /// 初始化UI的显示和隐藏
    /// </summary>
    private void ClearUIRoot() {
        Transform canvas = transform.Find("Canvas");
        for(int i = 0; i < canvas.childCount; ++i) {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Init() {
        //服务模块初始化
        NetSvc net = GetComponent<NetSvc>();
        net.InitSvc();
        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();
        AudioSvc audio = GetComponent<AudioSvc>();
        audio.InitSvc();
        TimerSvc timer = GetComponent<TimerSvc>();
        timer.InitSvc();

        //业务系统初始化
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();
        MainCitySys maincitySys = GetComponent<MainCitySys>();
        maincitySys.InitSys();
        FubenSys fuben = GetComponent<FubenSys>();
        fuben.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();


        dynamicWnd.SetWndState();
        //进入登陆场景并加载相应UI
        login.EnterLogin();

    }

    public static void AddTips(string tips) {
        Instance.dynamicWnd.AddTips(tips);
    }

    private PlayerData playerData = null;
    public PlayerData PlayerData {
        get {
            return playerData;
        }
    }
    public void SetPlayerData(RspLogin data) {
        playerData = data.playerData;
    }

    public void SetPlayerName(string name) {
        playerData.name = name;
    }

    public void SetPlayerDataGuide(RspGuide data) {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.guideid = data.guideid;
    }

    public void SetPlayerDataByStrong(RspStrong data) {
        playerData.coin = data.coin;
        playerData.crystal = data.crystal;
        playerData.hp = data.hp;
        playerData.ad = data.ad;
        playerData.ap = data.ap;
        playerData.addef = data.addef;
        playerData.apdef = data.apdef;

        playerData.strongArr = data.strongArr;
    }

    public void SetPlayerDataByBuy(RspBuy data) {
        playerData.diamond = data.diamond;
        playerData.coin = data.coin;
        playerData.power = data.power;
    }

    public void SetPlayerDataByPower(PshPower data) {
        playerData.power = data.power;
    }

    public void SetPlayerDataByTask(RspTakeTaskReward data) {
        playerData.coin = data.coin;
        playerData.lv = data.lv;
        playerData.exp = data.exp;
        playerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByTaskPsh(PshTaskPrgs data) {
        playerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByFBStart(RspFBFight data) {
        playerData.power = data.power;
    }

    public void SetPlayerDataByFBEnd(RspFBFightEnd data) {
        playerData.coin = data.coin;
        playerData.lv = data.lv;
        playerData.exp = data.exp;
        playerData.crystal = data.crystal;
        playerData.fuben = data.fuben;
    }
}