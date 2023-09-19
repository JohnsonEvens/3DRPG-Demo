/*-----------------------------------------------------
    文件：LoadingWnd.cs
	作者：Johnson
    日期：2023/4/23 21:10:24
	功能：加载进度界面
------------------------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot {
    public Text txtTips;
    public Image imgFG;
    public Image imgPoint;
    public Text txtPrg;

    private float fgWidth;

    protected override void InitWnd() {
        base.InitWnd(); //执行父类的初始化

        fgWidth = imgFG.GetComponent<RectTransform>().sizeDelta.x;

        SetText(txtTips, "按住Tab唤出鼠标");
        SetText(txtPrg, "0%");
        imgFG.fillAmount = 0;   //进度条水平压缩比例
        //进度光标初始化为进度为0的位置，坐标为进度条长度的负一半位置
        imgPoint.transform.localPosition = new Vector3(-fgWidth / 2, 0, 0);
    }

    public void SetProgress(float prg) {
        SetText(txtPrg, (int)(prg * 100) + "%");
        imgFG.fillAmount = prg;

        float posX = prg * fgWidth - fgWidth / 2;
        imgPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }
}