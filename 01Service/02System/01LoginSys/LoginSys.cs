/********************************************************************
	File: 	LoginSys.cs
	Author:	groundhog
	Time:	2025/2/20  16:44
	Description: 登录业务系统
*********************************************************************/

using PENet;

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
    
    public void Init()
    {
        PECommon.Log("LoginSys Init Done.");
    }
}

