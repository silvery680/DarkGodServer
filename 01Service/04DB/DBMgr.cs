/********************************************************************
	File: 	DBMgr.cs
	Author:	groundhog
	Time:	2025/2/21  15:03
	Description: 数据库管理类
*********************************************************************/

using MySql.Data.MySqlClient;
using PEProtocol;
using System;

public class DBMgr
{
    private static DBMgr instance = null;
    public static DBMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMgr();
            }
            return instance;
        }
    }

    private MySqlConnection conn;

    public void Init()
    {
        conn = new MySqlConnection(@"server=localhost;User Id = root;password=12345678;Database=darkgoddb;Charset = utf8mb4");
        conn.Open();
        PECommon.Log("DBMgr Init Done.");

        // QueryPlayerData("123", "123");
    }

    public PlayerData QueryPlayerData(string acct, string pass)
    {
        bool isNewAccount = true;
        PlayerData playerData = null;
        MySqlDataReader reader = null;

        try
        {
            MySqlCommand queryCmd = new MySqlCommand(@"select * from account where acct=@acct", conn);
            queryCmd.Parameters.AddWithValue("acct", acct);
            reader = queryCmd.ExecuteReader();
            if (reader.Read())
            {
                isNewAccount = false;
                string _pass = reader.GetString("pass");
                if (_pass.Equals(pass))
                {
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("id"),
                        lv = reader.GetInt32("lv"),
                        exp = reader.GetInt32("exp"),
                        power = reader.GetInt32("power"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond")

                        // TOADD
                    };
                }
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query PlayerData By Acct&Pass Error: " + e, LogType.Error);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (isNewAccount)
            {
                // 不存在账号数据，创建新的默认账号数据并返回
                playerData = InsertNewAcctData(acct, pass);
            }
        }
        return playerData;
    }

    private PlayerData InsertNewAcctData(string acct, string pass)
    {
        PlayerData newPlayerData = new PlayerData
        {
            id = -1,
            name = "",
            lv = 1,
            exp = 0,
            power = 150,
            coin = 5000,
            diamond = 500,
        };
        try
        {
            MySqlCommand insertCmd = new MySqlCommand(@"insert into account set acct=@acct,pass =@pass,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond", conn);
            insertCmd.Parameters.AddWithValue("acct", acct);
            insertCmd.Parameters.AddWithValue("pass", pass);
            insertCmd.Parameters.AddWithValue("name", newPlayerData.name);
            insertCmd.Parameters.AddWithValue("level", newPlayerData.lv);
            insertCmd.Parameters.AddWithValue("exp", newPlayerData.exp);
            insertCmd.Parameters.AddWithValue("power", newPlayerData.power);
            insertCmd.Parameters.AddWithValue("coin", newPlayerData.coin);
            insertCmd.Parameters.AddWithValue("diamond", newPlayerData.diamond);

            insertCmd.ExecuteNonQuery();
            newPlayerData.id = (int)insertCmd.LastInsertedId;
        }
        catch (Exception e)
        {
            PECommon.Log("Insert New PlayerData Error: " + e, LogType.Error);
        }

        return newPlayerData;
    }

    public  bool QueryNameData(string name)
    {
        bool isExist = false;
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand queryCmd = new MySqlCommand("select * from account where name = @name", conn);
            queryCmd.Parameters.AddWithValue("name", name);
            reader = queryCmd.ExecuteReader();
            if (reader.Read())
            {
                isExist = true;
            }
        }
        catch (Exception e)
        {
            PECommon.Log("Query Name State Error: " + e, LogType.Error);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
        return isExist;
    }

    public bool UpdatePlayerData(int id, PlayerData playerData)
    {
        try
        {
            //更新玩家数据
            MySqlCommand updateCmd = new MySqlCommand(
            "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond where id =@id", conn);
            updateCmd.Parameters.AddWithValue("id", id);
            updateCmd.Parameters.AddWithValue("name", playerData.name);
            updateCmd.Parameters.AddWithValue("level", playerData.lv);
            updateCmd.Parameters.AddWithValue("exp", playerData.exp);
            updateCmd.Parameters.AddWithValue("power", playerData.power);
            updateCmd.Parameters.AddWithValue("coin", playerData.coin);
            updateCmd.Parameters.AddWithValue("diamond", playerData.diamond);

            //TOADD Others
            updateCmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            PECommon.Log("Update PlayerData by id Error: " + e, LogType.Error);
            return false;
        }
        return true;
    }
}
    

