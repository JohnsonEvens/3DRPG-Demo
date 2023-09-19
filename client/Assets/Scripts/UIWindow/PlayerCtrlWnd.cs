/*-----------------------------------------------------
    文件：PlayerCtrlWnd.cs
	作者：Johnson
    日期：2023/8/6 22:13:9
	功能：玩家控制界面
------------------------------------------------------*/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCtrlWnd : WindowRoot {
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;

    public Transform expPrgTrans;

    private float pointDis;
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos = Vector2.zero;

    public Vector2 currentDir;
    protected float targetDup;
    protected float targetDright;
    //float rotX, rotY;
    //public float sensitivity = 30.0f;

    #region Skill1
    public Image imgSk1CD;
    public Text txtSk1CD;
    private bool isSk1CD = false;
    private float sk1CDTime;
    private int sk1Num;
    private float sk1FillCount = 0;
    private float sk1NumCount = 0;
    #endregion

    #region Skill2
    public Image imgSk2CD;
    public Text txtSk2CD;
    private bool isSk2CD = false;
    private float sk2CDTime;
    private int sk2Num;
    private float sk2FillCount = 0;
    private float sk2NumCount = 0;
    #endregion

    #region Skill3
    public Image imgSk3CD;
    public Text txtSk3CD;
    private bool isSk3CD = false;
    private float sk3CDTime;
    private int sk3Num;
    private float sk3FillCount = 0;
    private float sk3NumCount = 0;
    #endregion

    public Text txtSelfHP;
    public Image imgSelfHP;

    private int HPSum;

    protected override void InitWnd() {
        base.InitWnd();

        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);

        HPSum = GameRoot.Instance.PlayerData.hp;
        SetText(txtSelfHP, HPSum + "/" + HPSum);
        imgSelfHP.fillAmount = 1;

        SetBossHPBarState(false);
        //RegisterTouchEvts();
        sk1CDTime = resSvc.GetSkillCfg(101).cdTime / 1000.0f;
        sk2CDTime = resSvc.GetSkillCfg(102).cdTime / 1000.0f;
        sk3CDTime = resSvc.GetSkillCfg(103).cdTime / 1000.0f;

        RefreshUI();
    }

    public void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtLevel, pd.lv);
        SetText(txtName, pd.name);

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
    }

    private void Update() {
        /*        rotX = Input.GetAxis("Mouse X") * sensitivity;
                rotY = Input.GetAxis("Mouse Y") * sensitivity;*/

        //CameraRotation(Camera.main.gameObject, rotX, rotY);

        targetDup = (Input.GetKey(KeyCode.W) ? 1.0f : 0) - (Input.GetKey(KeyCode.S) ? 1.0f : 0);
        targetDright = (Input.GetKey(KeyCode.A) ? 1.0f : 0) - (Input.GetKey(KeyCode.D) ? 1.0f : 0);
        //PECommon.Log("X: " + Camera.main.transform.forward.x.ToString());
        BattleSys.Instance.SetMoveDir(new Vector2(targetDup, targetDright));

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            ClickNormalAtk();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            ClickSkill1Atk();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            ClickSkill2Atk();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ClickSkill3Atk();
        }

        float delta = Time.deltaTime;
        #region Skill CD
        if (isSk1CD) {
            sk1FillCount += delta;
            if (sk1FillCount >= sk1CDTime) {
                isSk1CD = false;
                SetActive(imgSk1CD, false);
                sk1FillCount = 0;
            }
            else {
                imgSk1CD.fillAmount = 1 - sk1FillCount / sk1CDTime;
            }

            sk1NumCount += delta;
            if (sk1NumCount >= 1) {
                sk1NumCount -= 1;
                sk1Num -= 1;
                SetText(txtSk1CD, sk1Num);
            }
        }
        if (isSk2CD) {
            sk2FillCount += delta;
            if (sk2FillCount >= sk2CDTime) {
                isSk2CD = false;
                SetActive(imgSk2CD, false);
                sk2FillCount = 0;
            }
            else {
                imgSk2CD.fillAmount = 1 - sk2FillCount / sk2CDTime;
            }

            sk2NumCount += delta;
            if (sk2NumCount >= 1) {
                sk2NumCount -= 1;
                sk2Num -= 1;
                SetText(txtSk2CD, sk2Num);
            }
        }
        if (isSk3CD) {
            sk3FillCount += delta;
            if (sk3FillCount >= sk3CDTime) {
                isSk3CD = false;
                SetActive(imgSk3CD, false);
                sk3FillCount = 0;
            }
            else {
                imgSk3CD.fillAmount = 1 - sk3FillCount / sk3CDTime;
            }

            sk3NumCount += delta;
            if (sk3NumCount >= 1) {
                sk3NumCount -= 1;
                sk3Num -= 1;
                SetText(txtSk3CD, sk3Num);
            }
        }
        #endregion

        if (transBossHPBar.gameObject.activeSelf) {
            BlendBossHP();
            imgYellow.fillAmount = currentPrg;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
    }

    void CameraRotation(GameObject cam, float rotX, float rotY) {
        cam.transform.Rotate(-rotY * Time.deltaTime, rotX * Time.deltaTime, 0);
       //cam.transform.Rotate(-rotY * Time.deltaTime, 0, 0);
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
            currentDir = Vector2.zero;
            //方向信息传递
            BattleSys.Instance.SetMoveDir(currentDir);

        });

        OnDrag(imgTouch.gameObject, (PointerEventData evt) => {
            Vector2 dir = evt.position - startPos;
            float len = dir.magnitude;  //返回向量长度
            if (len > pointDis) {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);  //把方向向量限制到这个长度
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else {
                imgDirPoint.transform.position = evt.position;
            }
            currentDir = dir.normalized;
            //方向信息传递
            BattleSys.Instance.SetMoveDir(currentDir);
        });
    }*/

    public void ClickNormalAtk() {
        BattleSys.Instance.ReqReleaseSkill(0);
    }

    public void ClickSkill1Atk() {
        if (isSk1CD == false && GetCanRlsSkill()) {
            BattleSys.Instance.ReqReleaseSkill(1);
            isSk1CD = true;
            SetActive(imgSk1CD);
            imgSk1CD.fillAmount = 1;
            sk1Num = (int)sk1CDTime;
            SetText(txtSk1CD, sk1Num);
        }
    }
    public void ClickSkill2Atk() {
        if (isSk2CD == false && GetCanRlsSkill()) {
            BattleSys.Instance.ReqReleaseSkill(2);
            isSk2CD = true;
            SetActive(imgSk2CD);
            imgSk2CD.fillAmount = 1;
            sk2Num = (int)sk2CDTime;
            SetText(txtSk2CD, sk2Num);
        }
    }
    public void ClickSkill3Atk() {
        if (isSk3CD == false && GetCanRlsSkill()) {
            BattleSys.Instance.ReqReleaseSkill(3);
            isSk3CD = true;
            SetActive(imgSk3CD);
            imgSk3CD.fillAmount = 1;
            sk3Num = (int)sk3CDTime;
            SetText(txtSk3CD, sk3Num);
        }
    }

    //Test Reset Data
    /*public void ClickResetCfgs() {
        resSvc.ResetSkillCfgs();
    }*/

    public void PauseGame() {
        BattleSys.Instance.battleMgr.isPauseGame = true;
        BattleSys.Instance.SetBattleEndWndState(FBEndType.Pause);
    }

    public void SetSelfHPBarVal(int val) {
        SetText(txtSelfHP, val + "/" + HPSum);
        imgSelfHP.fillAmount = val * 1.0f / HPSum;
    }

    public bool GetCanRlsSkill() {
        return BattleSys.Instance.battleMgr.CanRlsSkill();
    }

    public Transform transBossHPBar;
    public Image imgRed;
    public Image imgYellow;
    private float currentPrg = 1f;
    private float targetPrg = 1f;
    public void SetBossHPBarVal(int oldVal,int newVal,int sumVal) {
        currentPrg = oldVal * 1.0f / sumVal;
        targetPrg = newVal * 1.0f / sumVal;
        imgRed.fillAmount = targetPrg;
    }
    private void BlendBossHP() {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.AccelerHPSpeed * Time.deltaTime) {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg) {
            currentPrg -= Constants.AccelerHPSpeed * Time.deltaTime;
        }
        else {
            currentPrg += Constants.AccelerHPSpeed * Time.deltaTime;
        }
    }

    public void SetBossHPBarState(bool state, float prg = 1) {
        SetActive(transBossHPBar, state);
        imgRed.fillAmount = prg;
        imgYellow.fillAmount = prg;
    }
}