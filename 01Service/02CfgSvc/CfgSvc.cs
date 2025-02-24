/********************************************************************
	File: 	CfgSvc.cs
	Author:	groundhog
	Time:	2025/2/24  12:03
	Description: 配置数据服务
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;

public class CfgSvc
{
    private static CfgSvc instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }
            return instance;
        }
    }

    public void Init()
    {
        InitGuideCfg();
        PECommon.Log("CfgSvc Init Done.");
    }

    #region 自动引导配置
    private Dictionary<int, GuideCfg> autoGuideCfgDataDic = new Dictionary<int, GuideCfg>();
    private void InitGuideCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"D:\UnityProjects\DarkGod\Assets\Resources\ResCfgs\guide.xml");

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        foreach (XmlElement ele in nodeList)
        {
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int _ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            GuideCfg agc = new GuideCfg()
            {
                ID = _ID
            };


            foreach (XmlElement e in ele.ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        {
                            agc.coin = int.Parse(e.InnerText);
                        }
                        break;
                    case "exp":
                        {
                            agc.exp = int.Parse(e.InnerText);
                        }
                        break;

                }
            }

            autoGuideCfgDataDic.Add(_ID, agc);
        }
    }

    public GuideCfg GetAutoGuideData(int id)
    {
        GuideCfg data = null;
        if (autoGuideCfgDataDic.TryGetValue(id, out data))
        {
            return data;
        }
        return null;
    }
    #endregion
}

public class GuideCfg : BaseData<GuideCfg>
{
    public int coin;
    public int exp;
}

public class BaseData<T>
{
    public int ID;
}
