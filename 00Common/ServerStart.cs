/********************************************************************
	File: 	ServerStart.cs
	Author:	groundhog
	Time:	2025/2/20  16:36
	Description: 服务器入口
*********************************************************************/

using System.Threading;

class ServerStart
{
    static void Main(string[] args)
    {
        ServerRoot.Instance.Init();

        while (true)
        {
            ServerRoot.Instance.Update();
            Thread.Sleep(20);
        }
    }
}
