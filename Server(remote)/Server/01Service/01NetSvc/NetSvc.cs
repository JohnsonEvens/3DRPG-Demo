/*-----------------------------------------------------
    文件：NetSvc.cs
	作者：Johnson
    日期：2023/5/29 18:36:30
	功能：网络服务
------------------------------------------------------*/


using PENet;
using PEProtocol;
using System.Collections.Generic;
using System.Net;

public class MsgPack {
    public ServerSession session;
    public GameMsg msg;
    public MsgPack(ServerSession session, GameMsg msg) {
        this.session = session;
        this.msg = msg;
    }
}

public class NetSvc {
    private static NetSvc instance = null;
    public static NetSvc Instance {
        get {
            if (instance == null) {
                instance = new NetSvc();
            }
            return instance;
        }
    }
    public static readonly string obj = "lock";
    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

    public void Init() {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(IPAddress.Any.ToString(), SrvCfg.srvPort);

        PECommon.Log("NetSvc Init Done");  //会加上时间
    }

    public void AddMsgQue(ServerSession session, GameMsg msg) {
        //PESocket是异步的，存在多线程，对消息加入队列需要加锁
        lock (obj) {
            msgPackQue.Enqueue(new MsgPack(session, msg));
        }
    }

    public void Update() {
        if(msgPackQue.Count > 0) {
            /*PECommon.Log("PackCount:" + msgPackQue.Count);*/
            //逻辑处理可采用多线程，最后修改数据时加锁，性能相对高一点，但开发复杂度高很多，此处直接采用锁死
            lock (obj) {
                MsgPack pack = msgPackQue.Dequeue();
                HandOutMsg(pack);
            }
        }
    }

    private void HandOutMsg(MsgPack pack) {
        switch ((CMD)pack.msg.cmd) {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(pack);
                break;
            case CMD.ReqStrong:
                StrongSys.Instance.ReqStrong(pack);
                break;
            case CMD.SndChat:
                ChatSys.Instance.SndChat(pack);
                break;
            case CMD.ReqBuy:
                BuySys.Instance.ReqBuy(pack);
                break;
            case CMD.ReqTakeTaskReward:
                TaskSys.Instance.ReqTakeTaskReward(pack);
                break;
            case CMD.ReqFBFight:
                FuBenSys.Instance.ReqFBFight(pack);
                break;
            case CMD.ReqFBFightEnd:
                FuBenSys.Instance.ReqFBFightEnd(pack);
                break;
        }
    }
}

