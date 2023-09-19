/*-----------------------------------------------------
    文件：SystemRoot.cs
	作者：Johnson
    日期：2023/5/26 17:41:20
	功能：业务系统基类
------------------------------------------------------*/

using UnityEngine;

public class SystemRoot : MonoBehaviour {
    protected ResSvc resSvc;
    protected AudioSvc audioSvc;
    protected NetSvc netSvc;
    protected TimerSvc timerSvc;

    public virtual void InitSys() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }
}