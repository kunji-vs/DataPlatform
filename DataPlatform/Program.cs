using DataPlatform.DBLib;
using DataPlatform.Log;
using DataPlatform.Tools;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataPlatform
{
    class Program
    {
        static DBHelper db;

        private static readonly object LockObj = new object();

        static MainStart MainStart;

        static void Main(string[] args)
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 1)
            {
                Console.WriteLine("本程序最多只能运行一个实例");
                Console.WriteLine("按任意键退出。。。");
                Console.ReadKey();
                return;
            }
            string configPath = "config.ini";
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    configPath = args[0];
                }
            }
            IniConfigHelper configHelper = new IniConfigHelper(configPath);
            configHelper.Load();
            var host = configHelper.GetConfig("mysql", "host");
            var port = configHelper.GetConfig("mysql", "port");
            var username = configHelper.GetConfig("mysql", "username");
            var password = configHelper.GetConfig("mysql", "password");
            var database = configHelper.GetConfig("mysql", "database");
            var connstr = $"server={host};user id={username};port={port};password={password};database={database};persistsecurityinfo=True;Charset=utf8;";
            db = new DBHelper(connstr);
            if (!db.GetConnectState())
            {
                Console.WriteLine("链接数据库失败");
                Console.ReadKey();
                return;
            }
            LogHelper.WriteToConsole(true);
            LogHelper.WriteInfo("程序启动");
            SetConsoleCtrlHandler(cancelHandler, true);
            //MainStart = new MainStart(db);
            //MainStart.Start();
            while (true)
            {
                lock (LockObj)
                {
                    Monitor.Wait(LockObj);
                }
            }
        }

        static void Closing()
        {
            MainStart?.Stop();
        }

        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    //Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭  
                    Closing();
                    break;
                case 2:
                    //Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭  
                    Closing();
                    break;
            }
            Console.ReadLine();
            return false;
        }
    }
}
