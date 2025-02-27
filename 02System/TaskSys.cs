/********************************************************************
	File: 	TaskSys.cs
	Author:	groundhog
	Time:	2025/2/26  19:47
	Description: 任务奖励系统
*********************************************************************/

using PEProtocol;

public class TaskSys
{
    private static TaskSys instance = null;
    public static TaskSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskSys();
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
        PECommon.Log("LoginSys Init Done.");
    }

    public void ReqTakeTaskReward(MsgPack pack)
    {
        ReqTaskTaskReward data = pack.msg.reqTaskTaskReward;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTakeTaskReward,
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
        TaskRewardData trd = CalcTaskRewardData(pd, data.rid);
        
        if (trd.prgs == trc.count && !trd.taked)
        {
            pd.coin += trc.coin;
            PECommon.CalcExp(ref pd, trc.exp);
            trd.taked = true;
            CalcTaskArr(pd, trd);

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspTaskTaskReward rspTaskTaskReward = new RspTaskTaskReward
                {
                    coin = pd.coin,
                    lv = pd.lv,
                    exp = pd.exp,
                    taskArr = pd.taskArr
                };
                msg.rspTaskTaskReward = rspTaskTaskReward;
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }

        pack.session.SendMsg(msg);
    }

    public TaskRewardData CalcTaskRewardData(PlayerData pd, int rid)
    {
        TaskRewardData trd = null;
        for (int i = 0; i < pd.taskArr.Length; i++)
        {
            string[] taskInfo = pd.taskArr[i].Split('|');
            // 1|0|0
            if (int.Parse(taskInfo[0]) == rid)
            {
                trd = new TaskRewardData
                {
                    ID = int.Parse(taskInfo[0]),
                    prgs = int.Parse(taskInfo[1]),
                    taked = taskInfo[2].Equals("1")
                };
                break;
            }
        }
        return trd;
    }

    public void CalcTaskArr(PlayerData pd, TaskRewardData trd)
    {
        string result = trd.ID + "|" + trd.prgs + "|" + (trd.taked ? 1 : 0);
        int index = -1;
        for (int i = 0; i < pd.taskArr.Length; i ++)
        {
            string[] taskInfo = pd.taskArr[i].Split('|');
            if (int.Parse(taskInfo[0]) == trd.ID)
            {
                index = i;
                break;
            }
        }
        pd.taskArr[index] = result;
    }

    public void CalcTaskPrgs(PlayerData pd, int tid)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count)
        {
            trd.prgs += 1;

            CalcTaskArr(pd, trd);

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.PshTaskPrgs,
                pshTaskPrgs = new PshTaskPrgs
                {
                    taskArr = pd.taskArr
                }
            };
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                ServerSession session = cacheSvc.GetOnlineServerSessionByID(pd.id);
                if (session != null)
                {
                    session.SendMsg(msg);
                }
            }
        }
    }
}
