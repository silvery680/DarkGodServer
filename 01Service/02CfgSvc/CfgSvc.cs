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
        InitStrongCfg();
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
        PECommon.Log("GuideCfg Load Done.");
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

    #region 强化升级配置
    // 位置 + 星级
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDataDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(@"D:\UnityProjects\DarkGod\Assets\Resources\ResCfgs\strong.xml");

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        foreach (XmlElement ele in nodeList)
        {
            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }

            int _ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            StrongCfg sc = new StrongCfg()
            {
                ID = _ID
            };


            foreach (XmlElement e in ele.ChildNodes)
            {
                switch (e.Name)
                {
                    case "pos":
                        {
                            sc.pos = int.Parse(e.InnerText);
                        }
                        break;
                    case "starlv":
                        {
                            sc.starLv = int.Parse(e.InnerText);
                        }
                        break;
                    case "addhp":
                        {
                            sc.addHp = int.Parse(e.InnerText);
                        }
                        break;
                    case "addhurt":
                        {
                            sc.addHurt = int.Parse(e.InnerText);
                        }
                        break;
                    case "adddef":
                        {
                            sc.addDef = int.Parse(e.InnerText);
                        }
                        break;
                    case "minlv":
                        {
                            sc.minLv = int.Parse(e.InnerText);
                        }
                        break;
                    case "coin":
                        {
                            sc.coin = int.Parse(e.InnerText);
                        }
                        break;
                    case "crystal":
                        {
                            sc.crystal = int.Parse(e.InnerText);
                        }
                        break;
                }
            }

            Dictionary<int, StrongCfg> dic = null;
            if (strongDataDic.TryGetValue(sc.pos, out dic))
            {
                dic.Add(sc.starLv, sc);
            }
            else
            {
                dic = new Dictionary<int, StrongCfg>();
                dic.Add(sc.starLv, sc);

                strongDataDic.Add(sc.pos, dic);
            }
        }
        PECommon.Log("StongCfg Load Done.");
    }
    public StrongCfg GetStrongData(int id, int starLv)
    {
        StrongCfg sc = null;
        Dictionary<int, StrongCfg> dic = null;
        if (strongDataDic.TryGetValue(id, out dic))
        {
            if (dic.ContainsKey(starLv))
            {
                sc = dic[starLv];
            }
        }
        return sc;
    }
    #endregion
}

#region 数据格式定义
public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;
    public int starLv;
    public int addHp;
    public int addHurt;
    public int addDef;
    public int minLv;
    public int coin;
    public int crystal;
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
#endregion
