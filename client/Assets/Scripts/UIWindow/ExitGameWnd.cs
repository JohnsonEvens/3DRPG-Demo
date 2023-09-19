/*-----------------------------------------------------
    文件：ExitGameWnd.cs
	作者：Johnson
    日期：2023/8/21 23:57:18
	功能：退出游戏窗口
------------------------------------------------------*/

using UnityEngine;

public class ExitGameWnd : WindowRoot {
    protected override void InitWnd() {
        base.InitWnd();

        Camera.main.GetComponent<CameraRotateAround>().isMouseControlEnabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Camera.main.GetComponent<CameraRotateAround>().isWndOpen = true;
    }

    public void ClickClose() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }

    public void ClickSureBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}