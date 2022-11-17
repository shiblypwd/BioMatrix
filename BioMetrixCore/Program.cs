using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using BioMetrixCore.Utilities;
using System.Linq;
using System.Text.RegularExpressions;

namespace BioMetrixCore
{

    static class Program
    {
        public static int MONTH;
        public static int DAY;
        public static int HOUR;
        public static int MINUTE;

        public static string CONNECTION_STRING = @"Server=DESKTOP-VGQL2VE\\SQL19;Database=Pwd.Cms;Trusted_Connection=True";

        //public static string DEFAULT_PATH = @"";
        public static string DEFAULT_PATH = @"E:\PWD\BioMatrix\";
        public static string USER_INFO_CSV_FILE_PATH = DEFAULT_PATH + "usr.csv";
        public static string NOTIFICATION_FLAG_FILE_PATH = DEFAULT_PATH + "NotificationFlagFile.txt";

        private static string logPath = DEFAULT_PATH + "log.txt";
        private static string debugPath = DEFAULT_PATH + "debug.txt";

        public static string IP_ADDRESS = "172.16.1.72";
        //public static string IP_ADDRESS = "172.16.1.74";
        public static int PORT = 4370;
        public static int DEFAULT_MACHINE_NUMBER = 1;

        //static TimeSpan waitingTime = new TimeSpan(0, 10, 0);
        static TimeSpan waitingTime = new TimeSpan(0, 0, 10);

        public static Dictionary<int, string> info = new Dictionary<int, string>();
        public static List<UserEntry> userEntries = new List<UserEntry>();
        public static List<UserEntry> uniqueEntrys = new List<UserEntry>();
        public static List<int> listPresentIdInt = new List<int>();
        public static List<int> listFullId = new List<int>();



        [STAThread]
        static void Main()
        {

            new LocalSMS().processLocalSMS();


            //new LocalSMS().ClearInGateLogs();
        }
        
    }
}
