/********************************************************************
	File: 	GuideSys.cs
	Author:	groundhog
	Time:	2025/2/24  11:31
	Description: 任务引导业务系统
*********************************************************************/

using PEProtocol;

public class GuideSys
{
    private static GuideSys instance = null;
    public static GuideSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GuideSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("GuideSys Init Done.");
    }

    public void ReqGuide(MsgPack pack)
    {
        ReqGuide data = pack.msg.reqGuide;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspGuide
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        GuideCfg gc = cfgSvc.GetAutoGuideData(data.guideid);
        
        // 更新引导ID, 如果错误就是客户端和服务器信息不同步
        if (pd.guideID == data.guideid)
        {
            // 检测是否为智者点播任务
            if (pd.guideID == 1001)
            {
                TaskSys.Instance.CalcTaskPrgs(pd, 1);
            }
            pd.guideID += 1;

            // 更新玩家数据
            gc.coin += gc.coin;
            PECommon.CalcExp(ref pd, gc.exp);

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspGuide = new RspGuide
                {
                    guideid = pd.guideID,
                    coin = pd.coin,
                    lv = pd.lv,
                    exp = pd.exp,
                };
            }
        }
        else
        {
            msg.err = (int)ErrorCode.SeverDataError;
        }
        pack.session.SendMsg(msg);
    }

}
