using TheChosenProject.ServerCore.Website;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

 
namespace TheChosenProject.Website
{
  public class PayPal
  {
    public static Dictionary<int, int> getItems(string username)
    {
      Dictionary<int, int> items = new Dictionary<int, int>();
      try
      {
        using (MySqlConnection connection = new MySqlConnection(TopRankings.ConnectionString))
        {
          using (MySqlCommand mySqlCommand = new MySqlCommand("select item_number from payments where username=@u and claimed=0", connection))
          {
            connection.Open();
            mySqlCommand.Parameters.AddWithValue("@u", (object) username);
            using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
            {
              while (mySqlDataReader.Read())
              {
                int key = int.Parse(mySqlDataReader.GetString("item_number"));
                if (items.ContainsKey(key))
                  items[key]++;
                else
                  items.Add(key, 1);
              }
            }
          }
          using (MySqlCommand mySqlCommand = new MySqlCommand("update payments set claimed=1 where username=@u", connection))
          {
            mySqlCommand.Parameters.AddWithValue("@u", (object) username);
            mySqlCommand.ExecuteNonQuery();
          }
        }
      }
      catch (Exception ex)
      {
        ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
      }
      return items;
    }

    public static void logDonation(string user, string name, string log, string DateTime)
    {
      try
      {
        using (MySqlConnection connection = new MySqlConnection(TopRankings.ConnectionString))
        {
          using (MySqlCommand mySqlCommand = new MySqlCommand("insert into log_payments (username,name,log) values (@user,@name,@log)", connection))
          {
            connection.Open();
            mySqlCommand.Parameters.AddWithValue("@user", (object) user);
            mySqlCommand.Parameters.AddWithValue("@name", (object) name);
            mySqlCommand.Parameters.AddWithValue("@log", (object) log);
            mySqlCommand.ExecuteNonQuery();
          }
        }
      }
      catch (Exception ex)
      {
        ServerKernel.Log.SaveLog(ex.ToString(), false, LogType.EXCEPTION);
      }
    }
  }
}
