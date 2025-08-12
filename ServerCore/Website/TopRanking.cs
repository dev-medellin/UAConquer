using TheChosenProject.Multithreading;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheChosenProject.WindowsAPI;


namespace TheChosenProject.ServerCore.Website
{
  public class TopRankings
  {
    public static string db_name = DatabaseConfig.db_name;
    public static string db_password = DatabaseConfig.db_password;
    public static string db_username = DatabaseConfig.db_username;
    public static string ConnectionString = DatabaseConfig.ConnectionStringOpt;

    public static void LoadTopRankings()
    {
      try
      {
        List<TopRankings.Item> source = new List<TopRankings.Item>();
        IniFile iniFile = new IniFile("");
        foreach (string file in Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\"))
        {
          iniFile.FileName = file;
          source.Add(new TopRankings.Item()
          {
            NobilityDonation = iniFile.ReadUInt64("Character", "DonationNobility", 0UL),
            Class = (uint) iniFile.ReadByte("Character", "Class", (byte) 0),
            Level = (uint) iniFile.ReadByte("Character", "Level", (byte) 0),
            VIP = iniFile.ReadByte("Character", "VipLevel", (byte) 0),
            Name = iniFile.ReadString("Character", "Name", "None"),
            PkPoints = iniFile.ReadUInt32("Character", "PkPoints", 0),
            ConquerPoints = iniFile.ReadUInt32("Character", "ConquerPoints", 0),
            Money = iniFile.ReadUInt32("Character", "Money", 0),
            WHMoney = iniFile.ReadUInt32("Character", "WHMoney", 0)
          });
        }
        using (MySqlConnection connection = new MySqlConnection(TopRankings.ConnectionString))
        {
          connection.Open();
          try
          {
            using (MySqlCommand mySqlCommand = new MySqlCommand("DELETE FROM tops where top <> 5", connection))
              mySqlCommand.ExecuteNonQuery();
          }
          catch (Exception ex)
          {
            ServerKernel.Log.SaveLog(ex.ToString());
          }
          foreach (TopRankings.Item obj in source.OrderByDescending<TopRankings.Item, uint>((Func<TopRankings.Item, uint>) (e => e.PkPoints)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) (byte) 1);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) (byte) 2);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.OrderByDescending<TopRankings.Item, byte>((Func<TopRankings.Item, byte>) (e => e.VIP)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) (byte) 3);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.Where<TopRankings.Item>((Func<TopRankings.Item, bool>) (e => e.Class >= 11U && e.Class <= 15)).OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) 15);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.Where<TopRankings.Item>((Func<TopRankings.Item, bool>) (e => e.Class >= 21U && e.Class <= 25)).OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) 25);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.Where<TopRankings.Item>((Func<TopRankings.Item, bool>) (e => e.Class >= 41U && e.Class <= 45)).OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) 45);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.Where<TopRankings.Item>((Func<TopRankings.Item, bool>) (e => e.Class >= 142U && e.Class <= 145)).OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) 145);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.Where<TopRankings.Item>((Func<TopRankings.Item, bool>) (e => e.Class >= 132U && e.Class <= 135)).OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) 135);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.Where<TopRankings.Item>((Func<TopRankings.Item, bool>) (e => e.Class >= 51U && e.Class <= 55)).OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) 55);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          foreach (TopRankings.Item obj in source.Where<TopRankings.Item>((Func<TopRankings.Item, bool>) (e => e.Class >= 61U && e.Class <= 65)).OrderByDescending<TopRankings.Item, ulong>((Func<TopRankings.Item, ulong>) (e => e.NobilityDonation)).Take<TopRankings.Item>(10))
          {
            try
            {
              using (MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO tops VALUES(@name,@top,@pk,@level,@vip,@rank,@gold,@cps)", connection))
              {
                mySqlCommand.Parameters.AddWithValue("@name", (object) obj.Name);
                mySqlCommand.Parameters.AddWithValue("@top", (object) 65);
                mySqlCommand.Parameters.AddWithValue("@pk", (object) obj.PkPoints);
                mySqlCommand.Parameters.AddWithValue("@level", (object) obj.Level);
                mySqlCommand.Parameters.AddWithValue("@vip", (object) obj.VIP);
                mySqlCommand.Parameters.AddWithValue("@rank", (object) obj.NobilityDonation);
                mySqlCommand.Parameters.AddWithValue("@gold", (object)(obj.Money + obj.WHMoney));
                mySqlCommand.Parameters.AddWithValue("@cps", (object)obj.ConquerPoints);
                mySqlCommand.ExecuteNonQuery();
              }
            }
            catch (Exception ex)
            {
              ServerKernel.Log.SaveLog(ex.ToString());
            }
          }
          try
          {
            string[] strArray = new string[5]
            {
              "update Servers set MaxOnline=",
              null,
              null,
              null,
              null
            };
            int num = KernelThread.MaxOnline;
            strArray[1] = num.ToString();
            strArray[2] = ",Online=";
            num = KernelThread.Online;
            strArray[3] = num.ToString();
            strArray[4] = " where Name=@u";
            using (MySqlCommand mySqlCommand = new MySqlCommand(string.Concat(strArray), connection))
            {
              mySqlCommand.Parameters.AddWithValue("@u", (object) ServerKernel.ServerName);
              mySqlCommand.ExecuteNonQuery();
            }
          }
          catch (Exception ex)
          {
            ServerKernel.Log.SaveLog(ex.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        ServerKernel.Log.SaveLog(ex.ToString());
      }
    }

    public enum TopsType : byte
    {
      PK = 1,
      Nobility = 2,
      VIP = 3,
    }

    public struct Item
    {
      public byte VIP;
      public uint Class;
      public uint PkPoints;
      public uint Level;
      public ulong NobilityDonation;
      public unsafe fixed sbyte szName[16];
      public uint ConquerPoints;
      public uint Money;
      public uint WHMoney;

      public unsafe string Name
      {
        get
        {
          fixed (sbyte* numPtr = this.szName)
            return new string(numPtr);
        }
        set
        {
          fixed (sbyte* numPtr = this.szName)
          {
            for (int index = 0; index < value.Length; ++index)
              numPtr[index] = (sbyte) value[index];
          }
        }
      }
    }
  }
}
