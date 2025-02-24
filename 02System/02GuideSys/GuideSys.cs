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
            pd.guideID += 1;

            // 更新玩家数据
            gc.coin += gc.coin;
            CalcExp(ref pd, gc.exp);

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

    /// <summary>
    /// 获得经验值后，计算经验和等级
    /// </summary>
    /// <param name="pd">玩家数据</param>
    /// <param name="addExp">获得的经验值</param>
    private void CalcExp(ref PlayerData pd, int addExp)
    {
        int curtLv = pd.lv;
        int curtExp = pd.exp;
        int addRestExp = addExp;
        while (true)
        {
            int upNeedExp = PECommon.GetExpUpValByLv(curtLv) - curtExp;
            if (addRestExp >= upNeedExp)
            {
                curtLv += 1;
                curtExp = 0;
                addRestExp -= upNeedExp;
            }
            else
            {
                pd.lv = curtLv;
                pd.exp = curtExp + addRestExp;
                break;
            }
        }
    }
}
