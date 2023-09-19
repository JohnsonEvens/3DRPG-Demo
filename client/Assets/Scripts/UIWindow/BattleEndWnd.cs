/*-----------------------------------------------------
    文件：BattleEndWnd.cs
	作者：Johnson
    日期：2023/8/16 22:57:13
	功能：战斗结算界面
------------------------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

public class BattleEndWnd : WindowRoot {
    #region UI Define
    public Transform rewardTrans;
    public Button btnClose;
    public Button btnExit;
    public Button btnSure;
    public Text txtTime;
    public Text txtRestHP;
    public Text txtReward;
    public Animation ani;
    #endregion

    private FBEndType endType = FBEndType.None;
    protected override void InitWnd() {
        base.InitWnd();

        Camera.main.GetComponent<CameraRotateAround>().isMouseControlEnabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Camera.main.GetComponent<CameraRotateAround>().isWndOpen = true;

        RefreshUI();
    }

    private void RefreshUI() {
        switch (endType) {
            case FBEndType.Pause:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;
            case FBEndType.Win:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject, false);
                SetActive(btnClose.gameObject, false);

                MapCfg cfg = resSvc.GetMapCfg(fbid);
                int min = costtime / 60;
                int sec = costtime % 60;
                int coin = cfg.coin;
                int exp = cfg.exp;
                int crystal = cfg.crystal;
                SetText(txtTime, "计时：" + min + ":" + sec);
                SetText(txtRestHP, "剩余血量：" + resthp);
                SetText(txtReward, "采集：" + coin + "金币 " + exp + "经验 " + crystal + "别针");

                timerSvc.AddTimeTask((int tid) => {
                    SetActive(rewardTrans);
                    ani.Play();
                }, 1000);
                break;
            case FBEndType.Lose:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject, false);
                audioSvc.PlayUIAudio(Constants.FBLose);
                break;
        }
    }

    public void ClickClose() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        BattleSys.Instance.battleMgr.isPauseGame = false;
        SetWndState(false);
    }

    public void ClickExitBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        //进入主城，销毁当前战斗
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
    }

    public void ClickSureBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        //进入主城
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        //打开副本界面
        //FubenSys.Instance.EnterFuben();
    }

    public void SetWndType(FBEndType endType) {
        this.endType = endType;
    }

    private int fbid;
    private int costtime;
    private int resthp;
    public void SetBattleEndData(int fbid, int costtime, int resthp) {
        this.fbid = fbid;
        this.costtime = costtime;
        this.resthp = resthp;
    }
}

public enum FBEndType {
    None,
    Pause,
    Win,
    Lose,
}