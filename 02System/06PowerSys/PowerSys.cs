/********************************************************************
	File: 	PowerSys.cs
	Author:	groundhog
	Time:	2025/2/26  11:52
	Description: 体力恢复系统
*********************************************************************/

using PEProtocol;
using System.Collections.Generic;

public class PowerSys
{
    private static PowerSys instance = null;
    public static PowerSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PowerSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;

        // TO Change
        TimeSvc.Instance.AddTimeTask(CalcPowerAdd, PECommon.PowerAddSapce, PETimeUnit.Second, 0);
        PECommon.Log("PowerSys Init Done.");
    }

    private void CalcPowerAdd(int tid)
    {
        // 计算体力增长
        PECommon.Log("All Online PlayerCalc PowerIncrease...");
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshPower,
        };
        msg.pshPower = new PshPower();

        // 所有在线玩家获得实时的体力增长和推送数据
        Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();
        foreach(var item in onlineDic)
        {
            PlayerData pd = item.Value;
            ServerSession session = item.Key;

            int powerMax = PECommon.GetPowerLimit(pd.lv);
            if (pd.power >= powerMax)
            {
                continue;
            }
            else
            {
                pd.power += PECommon.PowerAddCount;
                if (pd.power > powerMax)
                {
                    pd.power = powerMax;
                }
            }

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.pshPower.power = pd.power;
                session.SendMsg(msg);
            }
        }


    }
}
