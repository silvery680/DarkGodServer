/********************************************************************
	File: 	CacheSvc.cs
	Author:	groundhog
	Time:	2025/2/21  11:39
	Description: 缓存层
*********************************************************************/
using PEProtocol;
using System.Collections.Generic;

public class CacheSvc
{
    private static CacheSvc instance = null;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }
            return instance;
        }
    }

    public void Init()
    {
        PECommon.Log("CacheSvc Init Done");
    }

    private Dictionary<string, ServerSession> onlineAcctDic = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> onlineSessionDic = new Dictionary<ServerSession, PlayerData>();

    public bool IsAcctOnline(string acct)
    {
        return onlineAcctDic.ContainsKey(acct);
    }

    /// <summary>
    /// 根据账号密码查找相关的玩家数据
    /// </summary>
    /// <param name="acct">账号</param>
    /// <param name="pass">密码</param>
    /// <returns></returns>
    public PlayerData GetPlayerData(string acct, string pass)
    {
        // TODO
        // 从数据库查找
        return null;
    }

    /// <summary>
    /// 账号上线，缓存数据
    /// </summary>
    /// <param name="acct">账号</param>
    /// <param name="session">连接</param>
    /// <param name="playerData">玩家基础数据</param>
    public void AcctOnline(string acct, ServerSession session, PlayerData playerData)
    {
        onlineAcctDic.Add(acct, session);
        onlineSessionDic.Add(session, playerData);
    }
}
