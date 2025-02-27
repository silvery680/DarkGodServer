/********************************************************************
	File: 	StrongSys.cs
	Author:	groundhog
	Time:	2025/2/25  11:06
	Description: 强化系统
*********************************************************************/

using PEProtocol;
using System;

public class StrongSys
{
    private static StrongSys instance = null;
    public static StrongSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrongSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("StrongSys Init Done.");
    }

    public void ReqStrong(MsgPack pack)
    {
        ReqStrong data = pack.msg.reqStrong;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspStrong,
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        int curtStartLv = pd.strongArr[data.pos];
        StrongCfg nextSd = CfgSvc.Instance.GetStrongData(data.pos, curtStartLv + 1);

        // 数据校验
        if (pd.lv < nextSd.minLv)
        {
            msg.err = (int)ErrorCode.LackLevel;
        }
        else if (pd.coin < nextSd.coin)
        {
            msg.err = (int)ErrorCode.LackCoin;
        }
        else if (pd.crystal < nextSd.crystal)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        else
        {
            // 任务进度更新
            TaskSys.Instance.CalcTaskPrgs(pd, 3);

            // 资源扣除
            pd.coin -= nextSd.coin;
            pd.crystal -= nextSd.crystal;

            pd.strongArr[data.pos] += 1;

            // 增加属性
            pd.hp += nextSd.addHp;
            pd.ad += nextSd.addHurt;
            pd.ap += nextSd.addHurt;
            pd.addef += nextSd.addDef;
            pd.apdef += nextSd.addDef;
        }

        if (!cacheSvc.UpdatePlayerData(pd.id, pd))
        {
            msg.err = (int)ErrorCode.UpdateDBError;
        }
        else
        {
            msg.rspStrong = new RspStrong
            {
                coin = pd.coin,
                crystal = pd.crystal,
                hp = pd.hp,
                ad = pd.ad,
                ap = pd.ap,
                addef = pd.addef,
                apdef = pd.apdef,
                strongArr = pd.strongArr,
            };
        }
        pack.session.SendMsg(msg);
    }
}
