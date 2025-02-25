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

    private DBMgr dbMgr = null;

    public void Init()
    {
        dbMgr = DBMgr.Instance;
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
        // 从数据库查找
        return dbMgr.QueryPlayerData(acct, pass);
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

    public bool IsNameExist(string name)
    {
        return dbMgr.QueryNameData(name);
    }

    /// <summary>
    /// 从指定链接获取玩家数据
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        if (onlineSessionDic.TryGetValue(session, out PlayerData playerData))
        {
            return playerData;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 更新玩家数据
    /// </summary>
    /// <param name="id">玩家id</param>
    /// <param name="playerData">新数据</param>
    /// <returns></returns>
    public bool UpdatePlayerData(int id, PlayerData playerData)
    { 
        return dbMgr.UpdatePlayerData(id, playerData);
    }

    /// <summary>
    /// 断线清除缓存数据
    /// </summary>
    /// <param name="session">连接</param>
    public void AcctOffline(ServerSession session)
    {
        foreach (var item in onlineAcctDic)
        {
            if (item.Value == session)
            {
                onlineAcctDic.Remove(item.Key);
                break;
            }
        }

        bool succ = onlineSessionDic.Remove(session);
        PECommon.Log("Offline Result: Session ID:" + session.sessionID + " Success");
    }

    public List<ServerSession> GetOnlineServerSessions()
    {
        List<ServerSession> lst = new List<ServerSession>();
        foreach (var item in onlineSessionDic)
        {
            lst.Add(item.Key);
        }
        return lst;
    }
}
