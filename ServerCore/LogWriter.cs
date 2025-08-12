using System;
using System.IO;

 
namespace TheChosenProject
{
  public class LogWriter
  {
    public const string STR_CONSOLE_MSG = "{0} - {1}";
    public const string STR_GMLOG_FORMAT = "{0} - {1}";
    public const string STR_GMLOG_FOLDER = "zfserver\\gmlog\\";
    public const string STR_GMLOG_SUBFOLDER = "yyyyMM";
    public const string STR_SYSLOG_FORMAT = "{0} [{1}] - {2}";
    public const string STR_SYSLOG_FOLDER = "zfserver\\syslog\\";
    public const string STR_SYSLOG_GAMESERVER = "CQ_Server";
    public const string STR_SYSLOG_NPCSERVER = "NPC_Server";
    public const string STR_SYSLOG_ANALYTIC = "Analytic";
    public const string STR_SYSLOG_DATABASE = "Database";
    private readonly string _szMainDirectory = Path.GetPathRoot(Environment.CurrentDirectory) + "zfserver\\";

    public LogWriter(string szPath)
    {
      this._szMainDirectory = szPath;
      this.CheckFolders();
    }

    public void SaveLog(string szMessage) => this.SaveLog(szMessage, false);

    public void SaveLog(string szMessage, bool bConsole)
    {
      this.SaveLog(szMessage, bConsole, "CQ_Server");
    }

    public void SaveLog(string szMessage, bool bConsole, LogType ltLog)
    {
      this.SaveLog(szMessage, bConsole, "CQ_Server", ltLog);
    }

    public void SaveLog(string szMessage, bool bConsole, string szFileName, LogType ltLog = LogType.MESSAGE)
    {
      this.CheckFolders();
      string szMessage1 = szMessage;
      szMessage = this.FormatSysString(szMessage, ltLog);
      string szFilePath = this._szMainDirectory + "zfserver\\syslog\\" + szFileName;
      if (bConsole)
        this.AppendServerLog(szMessage1);
      this.WriteToFile(szMessage, szFilePath);
    }

    public void GmLog(string szFileName, string szMessage)
    {
      this.GmLog(szFileName, szMessage, false);
    }

    public void GmLog(string szFileName, string szMessage, bool bConsole)
    {
      this.CheckFolders();
      bConsole = false;
      szFileName = this.GetGmFolder() + szFileName;
      szMessage = this.FormatGmString(szMessage);
      this.WriteToFile(szMessage, szFileName);
    }

    public void AppendServerLog(string szMessage)
    {
      szMessage = this.FormatGmString(szMessage);
      ServerKernel.ServerManager.AppendLog(szMessage, ServerManager.Logger.Main);
    }

    public void AppendGameLog(string szMessage)
    {
            
    }

    public void ServerLogCheats(string szMessage)
    {
      szMessage = this.FormatGmString(szMessage);
      ServerKernel.ServerManager.AppendLog(szMessage, ServerManager.Logger.Cheater);
    }

    public void WriteToFile(string szFullMessage, string szFilePath)
    {
      bool flag = false;
      szFilePath = szFilePath + DateTime.Now.ToString("yyyy-M-dd") + ".log";
      if (!File.Exists(szFilePath))
        File.Create(szFilePath).Close();
      while (!flag)
      {
        try
        {
          StreamWriter streamWriter = File.AppendText(szFilePath);
          streamWriter.WriteLine(szFullMessage);
          streamWriter.Close();
          flag = true;
        }
        catch
        {
        }
      }
    }

    private string FormatSysString(string szMessage, LogType ltType, bool bTime = true)
    {
      return bTime ? string.Format("{0} [{1}] - {2}", (object) DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), (object) ltType, (object) szMessage) : string.Format(szMessage);
    }

    private string FormatGmString(string szMessage)
    {
      return string.Format("{0} - {1}", (object) DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), (object) szMessage);
    }

    private string GetGmFolder()
    {
      return this._szMainDirectory + "zfserver\\gmlog\\" + DateTime.Now.ToString("yyyyMM") + "\\";
    }

    private void CheckFolders()
    {
      try
      {
        if (!Directory.Exists(this._szMainDirectory + "zfserver\\gmlog\\"))
          Directory.CreateDirectory(this._szMainDirectory + "zfserver\\gmlog\\");
        string szMainDirectory1 = this._szMainDirectory;
        DateTime now = DateTime.Now;
        string str1 = now.ToString("yyyyMM");
        if (!Directory.Exists(szMainDirectory1 + "zfserver\\gmlog\\" + str1 + "\\"))
        {
          string szMainDirectory2 = this._szMainDirectory;
          now = DateTime.Now;
          string str2 = now.ToString("yyyyMM");
          Directory.CreateDirectory(szMainDirectory2 + "zfserver\\gmlog\\" + str2 + "\\");
        }
        if (Directory.Exists(this._szMainDirectory + "zfserver\\syslog\\"))
          return;
        Directory.CreateDirectory(this._szMainDirectory + "zfserver\\syslog\\");
      }
      catch (Exception ex)
      {
        this.AppendServerLog(ex.ToString());
      }
    }
  }
}
