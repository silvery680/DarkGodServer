/********************************************************************
	File: 	NetSvc.cs
	Author:	groundhog
	Time:	2025/2/20  16:42
	Description: 网络服务
*********************************************************************/

using PENet;
using PEProtocol;

public class NetSvc
{
    private static NetSvc instance = null;
    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }
            return instance;
        }
    }

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.srvIP, SrvCfg.srvPort);

        PECommon.Log("NetSvc Init Done.");
    }
}
