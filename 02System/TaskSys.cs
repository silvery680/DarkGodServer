/********************************************************************
	File: 	TaskSys.cs
	Author:	groundhog
	Time:	2025/2/26  19:47
	Description: 任务奖励系统
*********************************************************************/

public class TaskSys
{
    private static TaskSys instance = null;
    public static TaskSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskSys();
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
}
