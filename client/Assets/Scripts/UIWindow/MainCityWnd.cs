/*-----------------------------------------------------
    文件：MainCityWnd.cs
	作者：Johnson
    日期：2023/6/4 23:31:33
	功能：主城UI界面
------------------------------------------------------*/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MainCityWnd : WindowRoot {
    #region UIDefine
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;

    public Animation menuAni;
    public Button btnMenu;

    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtLevel;
    public Text txtExpPrg;

    public Transform expPrgTrans;

    public Button btnGuide;
    #endregion

    private bool menuState = true;
    private float pointDis; 
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos = Vector2.zero;
    private AutoGuideCfg curtTaskData;

    protected float targetDup;
    protected float targetDright;

    private void Update() {
        targetDup = (Input.GetKey(KeyCode.W) ? 1.0f : 0) - (Input.GetKey(KeyCode.S) ? 1.0f : 0);
        targetDright = (Input.GetKey(KeyCode.A) ? 1.0f : 0) - (Input.GetKey(KeyCode.D) ? 1.0f : 0);
        //PECommon.Log("X: " + Camera.main.transform.forward.x.ToString());
        MainCitySys.Instance.SetMoveDir(new Vector2(targetDup, targetDright));

        if (Input.GetKeyDown(KeyCode.Escape)) {
            MainCitySys.Instance.OpenExitWnd();
        }
    }

    #region MainFunctions
    protected override void InitWnd() {
        base.InitWnd();
        //窗口分辨率变化后，遥感移动距离会改变，所以需要依据比例做个计算
        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);

        //RegisterTouchEvts();

        RefreshUI();
    }

    public void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtPower, "体力:" + pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1.0f / PECommon.GetPowerLimit(pd.lv);
        SetText(txtLevel, pd.lv);

        #region Expprg
        int expPrgVal = (int)(pd.exp * 1.0f / PECommon.GetExpUpValByLv(pd.lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;

        GridLayoutGroup grid = expPrgTrans.GetComponent<GridLayoutGroup>();

        float globalRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float screenWidth = Screen.width * globalRate;
        float width = (screenWidth - 180) / 10;

        grid.cellSize = new Vector2(width, 7);

        for (int i = 0; i < expPrgTrans.childCount; i++) {
            Image img = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i < index) {
                img.fillAmount = 1;

            }
            else if (i == index) {
                img.fillAmount = expPrgVal % 10 * 1.0f / 10;
            }
            else {
                img.fillAmount = 0;
            }
        }
        #endregion

        //设置自动任务图标
        curtTaskData = resSvc.GetAutoGuideCfg(pd.guideid);
        /*if(curtTaskData != null) {
            SetGuideBtnIcon(curtTaskData.npcID);    
        }
        else {
            SetGuideBtnIcon(-1);
        }*/
    }

    private void SetGuideBtnIcon(int npcID) {
        string spPath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID) {
            case Constants.NPCWiseMan:
                spPath = PathDefine.WiseManHead;
                break;
            case Constants.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            default:
                spPath = PathDefine.TaskHead;
                break;
        }
        SetSprite(img, spPath);
    }
    #endregion

    #region ClickEvts
    public void ClickFubenBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.EnterFuben();
    }
    public void ClickBuyPowerBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(0);
    }
    public void ClickTaskBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenTaskRewardWnd();
    }
    public void ClickMKCoinBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyWnd(1);
    }
    public void ClickStrongBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenStrongWnd();
    }
    public void ClickGuideBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        if(curtTaskData != null) {
            MainCitySys.Instance.RunTask(curtTaskData);
        }
        else {
            GameRoot.AddTips("更多引导任务，正在开发中...");
        }
    }
    public void ClickMenuBtn() {
        audioSvc.PlayUIAudio(Constants.UIExtenBtn);

        menuState = !menuState;
        AnimationClip clip = null;
        if (menuState) {
            clip = menuAni.GetClip("MenuOpen");
        }
        else {
            clip = menuAni.GetClip("MenuClose");

        }
        menuAni.Play(clip.name);
    }
    public void ClickHeadBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenInfoWnd();
    }
    public void ClickChatBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenChatWnd();
    }

    /*public void RegisterTouchEvts() {
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) => {
            startPos = evt.position;
            SetActive(imgDirPoint);
            imgDirBg.transform.position = evt.position;
        });

        OnClickUp(imgTouch.gameObject, (PointerEventData evt) => {
            imgDirBg.transform.position = defaultPos;
            SetActive(imgDirPoint, false);
            imgDirPoint.transform.localPosition = Vector2.zero;
            //方向信息传递
            MainCitySys.Instance.SetMoveDir(Vector2.zero);

        });

        OnDrag(imgTouch.gameObject, (PointerEventData evt) => {
            Vector2 dir = evt.position - startPos;
            float len = dir.magnitude;  //返回向量长度
            if(len > pointDis) {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);  //把方向向量限制到这个长度
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else {
                imgDirPoint.transform.position = evt.position;
            }
            //方向信息传递
            MainCitySys.Instance.SetMoveDir(dir.normalized);
        });
    }*/
    #endregion
}