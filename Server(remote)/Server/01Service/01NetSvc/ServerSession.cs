/*-----------------------------------------------------
    文件：ServerSession.cs
	作者：Johnson
    日期：2023/5/29 18:58:30
	功能：网络会话连接
------------------------------------------------------*/

using PENet;
using PEProtocol;

public class ServerSession: PESession<GameMsg> {
    public int sessionID = 0;
    protected override void OnConnected() {
        sessionID = ServerRoot.Instance.GetSessionID();
        PECommon.Log("SessionID: " + sessionID + " Client Connect");
        
    }

    protected override void OnReciveMsg(GameMsg msg) {
        PECommon.Log("SessionID: " + sessionID + " RcvPack CMD: " + ((CMD)msg.cmd).ToString());
        //接收数据是多线程，为了数据安全，把它放到队列里，采用单线程处理信息
        NetSvc.Instance.AddMsgQue(this, msg);
    }

    protected override void OnDisConnected() {
        LoginSys.Instance.ClearOfflineData(this);
        PECommon.Log("SessionID: " + sessionID + " Client Offline");
    }
}
