using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheChosenProject
{
    public class MyLogger
    {
        public static MyLogger ProtectSystem, GameServer, Events, Exception, Payments;

        private static string logTemplateConsole = "[{Timestamp:HH:mm:ss}]" + " {Message}{NewLine}{Exception}";
        private static string logTemplateFile = "[{Timestamp:HH:mm:ss}]" + " <{Level} {SourceContext}> {Message}{NewLine}{Exception}";

        private readonly ILogger logger;

        public MyLogger(string className, bool _console = true)
        {
            if (_console)
            {
                logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    //.WriteTo.Console(outputTemplate: logTemplateConsole)
                    .WriteTo.File($"MyLogger//{className}.log", outputTemplate: logTemplateFile)
                    .CreateLogger()
                    .ForContext("SourceContext", className);
            }
            else
            {
                logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File($"MyLogger//{className}.log", outputTemplate: logTemplateFile)
                    .CreateLogger()
                    .ForContext("SourceContext", className);
            }
        }

        public static void Init()
        {
            if (!Directory.Exists(Application.StartupPath + "\\MyLogger"))
                Directory.CreateDirectory(Application.StartupPath + "\\MyLogger");
            ProtectSystem = new MyLogger("ProtectSystem");
            GameServer = new MyLogger("GameServer");
            Events = new MyLogger("Events", false);
            Exception = new MyLogger("Exception");
            Payments = new MyLogger("Payments");
        }

        public void WriteInfo(string msg)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Information(msg);
        }

        public void WriteInfo(string msg, params object[] args)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Information(msg, args);
        }

        public void WriteError(string msg)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Error(msg);
        }

        public void WriteError(string msg, params object[] args)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Error(msg, args);
        }

        public void WriteFatal(string msg)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Fatal(msg);
        }

        public void WriteFatal(string msg, params object[] args)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Fatal(msg, args);
        }

        public void WriteWarning(string msg)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Warning(msg);
        }

        public void WriteWarning(string msg, params object[] args)
        {
            //if (!ServerConfig.IsInterServer)
                logger.Warning(msg, args);
        }
    }
}