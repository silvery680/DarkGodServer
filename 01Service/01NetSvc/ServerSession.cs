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
    public int sessionID = 0;

    protected override void OnConnected()
    {
        sessionID = ServerRoot.Instance.GetSessionID();
        PECommon.Log("Session ID:" + sessionID + " Client Connection");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("Session ID:" + sessionID + " RcvPack CMD:" + ((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQue(this, msg);
    }

    protected override void OnDisConnected()
    {
        LoginSys.Instance.ClearOfflineData(this);
        PECommon.Log("Session ID:" + sessionID + " Client DisConnect");
    }
}
