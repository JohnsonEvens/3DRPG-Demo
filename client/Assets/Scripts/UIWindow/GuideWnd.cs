/*-----------------------------------------------------
    文件：GuideWnd.cs
	作者：Johnson
    日期：2023/7/13 22:7:13
	功能：引导对话界面
------------------------------------------------------*/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class GuideWnd : WindowRoot {
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;

    private PlayerData pd;
    private AutoGuideCfg curtTaskData;
    private string[] dialogArr;
    private int index;  //对话索引号

    protected override void InitWnd() {
        base.InitWnd();

        Camera.main.GetComponent<CameraRotateAround>().isMouseControlEnabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Camera.main.GetComponent<CameraRotateAround>().isWndOpen = true;

        pd = GameRoot.Instance.PlayerData;
        curtTaskData = MainCitySys.Instance.GetCurtTaskData();
        dialogArr = curtTaskData.dilogArr.Split('#');
        index = 1;

        SetTalk();
    }

    private void SetTalk() {
        string[] talkArr = dialogArr[index].Split('|');
        if(talkArr[0] == "0") {
            //自己
            SetSprite(imgIcon, PathDefine.SelfIcon);
            SetText(txtName, pd.name);
        }
        else {
            //对话npc
            switch (curtTaskData.npcID) {
                case 0:
                    SetSprite(imgIcon, PathDefine.OthersIcon);
                    SetText(txtName, "幸存者1");
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.OthersIcon);
                    SetText(txtName, "幸存者2");
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.OthersIcon);
                    SetText(txtName, "幸存者3");
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.OthersIcon);
                    SetText(txtName, "幸存者4");
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.OthersIcon);
                    SetText(txtName, "幸存者5");
                    break;
            }
        }

        //imgIcon.SetNativeSize();
        SetText(txtTalk, talkArr[1].Replace("$name", pd.name));
    }

    public void ClickNextBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        index += 1;
        if(index == dialogArr.Length) {
            //TODO 发送任务引导完成信息
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqGuide,
                reqGuide = new ReqGuide {
                    guideid = curtTaskData.ID
                }
            };
            netSvc.SendMsg(msg);
            SetWndState(false);
        }
        else {
            SetTalk();
        }
    }
}