/********************************************************************
	File: 	ServerRoot.cs
	Author:	groundhog
	Time:	2025/2/20  16:37
	Description: 服务器入口
*********************************************************************/

class ServerRoot
{
    private static ServerRoot instance = null;
    public static ServerRoot Instance
    {
        get{
            if (instance == null)
            {
                instance = new ServerRoot();
            }
            return instance;
        }
    }

    public void Init()
    {
        // 数据层TODO
        DBMgr.Instance.Init();

        // 服务层
        CfgSvc.Instance.Init();
        CacheSvc.Instance.Init();
        NetSvc.Instance.Init();

        // 业务系统层
        LoginSys.Instance.Init();
        GuideSys.Instance.Init();
        StrongSys.Instance.Init();
    }
    
    public void Update()
    {
        NetSvc.Instance.Update();
    }

    private int sessionID = 0;
    public int GetSessionID()
    {
        if (sessionID == int.MaxValue)
        {
            sessionID = 0;
        }
        return sessionID += 1;
    }
}
