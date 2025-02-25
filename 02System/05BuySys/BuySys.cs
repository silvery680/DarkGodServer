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


        pack.session.SendMsg(msg);
    }
}
