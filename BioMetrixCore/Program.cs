using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using BioMetrixCore.Utilities;
using System.Linq;

namespace BioMetrixCore
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static int MONTH = 9;
        public static int DAY = 12;

        public static string CONNECTION_STRING = @"Server=DESKTOP-VGQL2VE\\SQL19;Database=Pwd.Cms;Trusted_Connection=True";

        public static string USER_INFO_CSV_FILE_PATH = @"F:\usr.csv";
        public static string TODAY_OUTPUT_PATH = @"F:\today.csv";

        private static string logPath = @"log.txt";

        private static string debugPath = @"debug.txt";

        //public static string IP_ADDRESS = "172.16.1.72";
        public static string IP_ADDRESS = "172.16.1.74";
        public static int PORT = 4370;
        public static int DEFAULT_MACHINE_NUMBER = 1;

        //static TimeSpan waitingTime = new TimeSpan(0, 10, 0);
        static TimeSpan waitingTime = new TimeSpan(0, 0, 10);

        public static Dictionary<int, string> info = new Dictionary<int, string>();

        public static List<UserEntry> userEntries = new List<UserEntry>();
        public static List<UserEntry> uniqueEntrys = new List<UserEntry>();


        public static void debug(string content)
        {
            File.AppendAllText(debugPath, content + "\n");
        }

        public static void writeToFile(string content)
        {
            string path = logPath;
            File.AppendAllText(path, content + "\n");
        }

        public static void log(string content)
        {
            File.AppendAllText(logPath, content + "\n");
        }

        public static void writeToFileWithoutNL(string content)
        {
            string path = logPath;
            File.AppendAllText(path, content);
        }

        [STAThread]
        static void Main()
        {
            int id = 0;
            string[] lines = System.IO.File.ReadAllLines(USER_INFO_CSV_FILE_PATH);
            for (int i = 0; i < lines.Length; i++)
            {
                
                string line = lines[i];
                //Console.WriteLine(line);    

                int ind = line.IndexOf(',');
                if (ind != -1)
                {
                    id = Convert.ToInt32(line.Substring(0, ind));
                }
                //Console.Write(id+" ");  
                info[id] = line;
            }
            getTodayInData();

            HashSet<int> st = new HashSet<int>();


            //First Entry of Everyone
            foreach (UserEntry entry in userEntries)
            {
                if (st.Contains(entry.Id)) continue;    //Skip if not the first entry of persion @entry
                st.Add(entry.Id);

                if (info.ContainsKey(entry.Id))
                {
                    entry.DataStr = info[entry.Id];
                }

                uniqueEntrys.Add(entry);
            }

            var list = uniqueEntrys.OrderBy(x => x.EntryTime).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (i < 50)
                    Console.WriteLine(list[i].EntryTime.TimeOfDay + "  " + list[i].Id + "#\t" + list[i].EntryTime.TimeOfDay.ToString() + "," + list[i].DataStr);

                File.AppendAllText(TODAY_OUTPUT_PATH, list[i].EntryTime.TimeOfDay.ToString() + "," + list[i].DataStr + "\n");
                //if (i==10) break;
            }
            //Console.WriteLine("Done!!!!!");

            //runUI();
            //runCode();
            //periodicLogFetching();
            //periodicLogFetchingOneConnection();

        }

        static void getTodayInData()
        {

            Master master = new Master();
            master._ConnectOnly();
            master._getLog();
            master._disconnet();

            Console.WriteLine(userEntries.Count);
            IP_ADDRESS = "172.16.1.73";

            Master master73 = new Master();
            master73._ConnectOnly();
            master73._getLog();
            master73._disconnet();
            Console.WriteLine(userEntries.Count);
        }



        static void periodicLogFetching()
        {
            while (true)
            {
                Master master = new Master();

                var stopwatchConnection = new Stopwatch();
                var stopwatchFetch = new Stopwatch();
                var stopwatchClear = new Stopwatch();
                long connectingTime = -1;
                long logReadingTime = -1;
                long logClearTime = -1;

                stopwatchConnection.Start();

                master._ConnectOnly();

                stopwatchConnection.Stop();
                connectingTime = stopwatchConnection.ElapsedMilliseconds;

                stopwatchFetch.Start();
                master._getLog();
                stopwatchFetch.Stop();
                logReadingTime = stopwatchFetch.ElapsedMilliseconds;



                master._disconnet();


                Console.WriteLine("logReadingTime: " + logReadingTime + "\t\tconnectingTime: " + connectingTime.ToString() + "\t\tlogClearTime: " + logClearTime.ToString());
                break;
                Thread.Sleep(waitingTime);

            }
        }


        static void runCode()
        {
            Master master = new Master();

            var stopwatch = new Stopwatch();
            long connectingTime = -1;
            long logReadingTime = -1;

            stopwatch.Start();

            master._Connect();

            stopwatch.Stop();
            connectingTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Start();

            master._getLog();

            stopwatch.Stop();
            logReadingTime = stopwatch.ElapsedMilliseconds;


            Console.WriteLine("logReadingTime: " + logReadingTime + "\t\tconnectingTime: " + connectingTime.ToString());
        }


        static void periodicLogFetchingOneConnection()
        {
            Master master = new Master();

            var stopwatch = new Stopwatch();
            long connectingTime = -1;
            long logReadingTime = -1;

            stopwatch.Start();

            master._Connect();

            stopwatch.Stop();
            connectingTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Connection Done# Time: " + connectingTime);

            while (true)
            {
                stopwatch.Start();

                master._getLog();

                stopwatch.Stop();
                logReadingTime = stopwatch.ElapsedMilliseconds;


                Console.WriteLine("logReadingTime: " + logReadingTime.ToString());
                Thread.Sleep(waitingTime);
            }
        }

        static void runUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Master());
        }
    }
}
