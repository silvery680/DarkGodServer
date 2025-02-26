/********************************************************************
	File: 	BuySys.cs
	Author:	groundhog
	Time:	2025/2/25  21:27
	Description: 购买业务系统 
*********************************************************************/

using PEProtocol;
using System;

public class BuySys
{
    private static BuySys instance = null;
    public static BuySys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuySys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("BuySys Init Done.");
    }

    internal void ReqBuy(MsgPack pack)
    {
        ReqBuy data = pack.msg.reqBuy;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspBuy,
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        if (pd.diamond < data.cost)
        {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else
        {
            pd.diamond -= data.cost;
            switch(data.type)
            {
                case BuyType.Power:
                    pd.power += 100;
                    break;
                case BuyType.Coin:
                    pd.coin += 1000;
                    break;
            }

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspBuy rspBuy = new RspBuy
                {
                    type = data.type,
                    dimond = pd.diamond,
                    coin = pd.coin,
                    power = pd.power,
                };
                msg.rspBuy = rspBuy;
            }
        }

        pack.session.SendMsg(msg);
    }
}
