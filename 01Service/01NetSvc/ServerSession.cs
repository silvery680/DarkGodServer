/********************************************************************
	File: 	ServerSession.cs
	Author:	groundhog
	Time:	2025/2/20  17:28
	Description: 网络会话连接
*********************************************************************/

using PENet;
using PEProtocol;

public class ServerSession:PESession<GameMsg>
{
    protected override void OnConnected()
    {
        PECommon.Log("Client Connection");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("RcvPack CMD:" + ((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQue(this, msg);
    }

    protected override void OnDisConnected()
    {
        PECommon.Log("Client DisConnect");
    }
}
