/********************************************************************
	File: 	LoginSys.cs
	Author:	groundhog
	Time:	2025/2/20  16:44
	Description: 登录业务系统
*********************************************************************/

using PENet;
using PEProtocol;

public class LoginSys
{
    private static LoginSys instance = null;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("LoginSys Init Done.");
    }

    public void ReqLogin(MsgPack pack)
    {
        // 解包
        ReqLogin data = pack.msg.reqLogin;
        // 当前账号是否已经上线
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,

        };
        // 已上线：返回错误信息
        if (cacheSvc.IsAcctOnline(data.acct))
        {
            msg.err = (int)ErrorCode.AcctIsOnline;
        }
        else
        {
            PlayerData pd = cacheSvc.GetPlayerData(data.acct, data.pass);
            // 存在，但是密码错误
            if (pd == null)
            {
                msg.err = (int)ErrorCode.WrongPass;
            }
            else
            {
                msg.rspLogin = new RspLogin
                {
                    playerData = pd
                };
                // 缓存账号数据
                cacheSvc.AcctOnline(data.acct, pack.session, pd);
            }
        }

        // 回应客户端
        pack.session.SendMsg(msg);
    }
}

