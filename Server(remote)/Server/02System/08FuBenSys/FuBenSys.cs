/*-----------------------------------------------------
    文件：FuBenSys.cs
	作者：Johnson
    日期：2023/8/5 16:57:30
	功能：副本战斗业务
------------------------------------------------------*/

using PEProtocol;

public class FuBenSys {
    private static FuBenSys instance = null;
    public static FuBenSys Instance {
        get {
            if (instance == null) {
                instance = new FuBenSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("FuBenSys Init Done");
    }

    public void ReqFBFight(MsgPack pack) {
        ReqFBFight data = pack.msg.reqFBFight;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspFBFight
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        int power = cfgSvc.GetMapCfg(data.fbid).power;

        //判断关卡是否符合进度
        if (pd.fuben < data.fbid) {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        else if(pd.power < power) {
            msg.err = (int)ErrorCode.LackPower;
        }
        else {
            pd.power -= power;
            if (cacheSvc.UpdatePlayerData(pd.id, pd)) {
                msg.rspFBFight = new RspFBFight {
                    fbid = data.fbid,
                    power = pd.power,
                };
            }
            else {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
        }
        pack.session.SendMsg(msg);
    }

    public void ReqFBFightEnd(MsgPack pack) {
        ReqFBFightEnd data = pack.msg.reqFBFightEnd;

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.RspFBFightEnd
        };

        //校验战斗是否合法
        if (data.win) {
            if (data.costtime > 0 && data.resthp > 0) {
                //根据副本ID获取相应的奖励
                MapCfg rd = cfgSvc.GetMapCfg(data.fbid);
                PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

                //任务进度数据更新
                TaskSys.Instance.CalcTaskPrgs(pd, 2);

                pd.coin += rd.coin;
                pd.crystal += rd.crystal;
                PECommon.CalcExp(pd, rd.exp);

                if(pd.fuben == data.fbid) {
                    pd.fuben += 1;
                }

                if (!cacheSvc.UpdatePlayerData(pd.id, pd)) {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else {
                    RspFBFightEnd rspFBFightEnd = new RspFBFightEnd {
                        win = data.win,
                        fbid = data.fbid,
                        resthp = data.resthp,
                        costtime = data.costtime,

                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp,
                        crystal = pd.crystal,
                        fuben = pd.fuben
                    };

                    msg.rspFBFightEnd = rspFBFightEnd;
                }
            }
        }
        else {
            msg.err = (int)ErrorCode.ClientDataError;
        }

        pack.session.SendMsg(msg);
    }
}
