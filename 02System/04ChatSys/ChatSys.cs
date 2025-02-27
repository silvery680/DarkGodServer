/********************************************************************
	File: 	ChatSys.cs
	Author:	groundhog
	Time:	2025/2/25  16:25
	Description: 聊天业务系统
*********************************************************************/

using PEProtocol;
using System;
using System.Collections.Generic;

public class ChatSys
{
    private static ChatSys instance = null;
    public static ChatSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("ChatSys Init Done.");
    }

    internal void SndChat(MsgPack pack)
    {
        SndChat data = pack.msg.sndChat;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        // 任务进度更新
        TaskSys.Instance.CalcTaskPrgs(pd, 6);

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshChat,
            pshChat = new PshChat
            {
                name = pd.name,
                chat = data.chat
            }
        };

        // 广播所有在线客户端
        // 先序列化，然后再转发序列化的数据，避免重复序列化带来的性能消耗
        List<ServerSession> lst = cacheSvc.GetOnlineServerSessions();
        byte[] bytes = PENet.PETool.PackNetMsg(msg);
        foreach (var item in lst)
        {
            item.SendMsg(bytes);
        }
    }
}
