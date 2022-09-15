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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static int MONTH;
        public static int DAY;
        

        public static string CONNECTION_STRING = @"Server=DESKTOP-VGQL2VE\\SQL19;Database=Pwd.Cms;Trusted_Connection=True";

        public static string USER_INFO_CSV_FILE_PATH = @"F:\usr.csv";



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
        public static List<int> listPresentIdInt = new List<int>();
        public static List<int> listFullId = new List<int>();
        public static List<int> absentList = new List<int>();




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

            Console.WriteLine("Enter Month: ");
            MONTH = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter Day: ");
            DAY = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\n\n");

            string TODAY_OUTPUT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-idWise.csv";
            string TODAY_OUTPUT_PATH_TIMEWISE = @"F:\" + MONTH + "_" + DAY + "_22-timeWise.csv";
            string TODAY_OUTPUT_ABSENT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-absent.csv";

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
                listFullId.Add(id);
                info[id] = line;
            }
            getTodayInData();

            HashSet<int> st = new HashSet<int>();


            //First Entry of Everyone
            foreach (UserEntry entry in userEntries)
            {
                if (st.Contains(entry.Id)) continue;    //Skip if not the first entry of person @entry
                st.Add(entry.Id);

                if (info.ContainsKey(entry.Id))
                {
                    entry.DataStr = info[entry.Id];
                }

                uniqueEntrys.Add(entry);
            }
            

            var listTime = uniqueEntrys.OrderBy(x => x.EntryTime).ToList(); 

            var listID = uniqueEntrys.OrderBy(x => x.Id).ToList();

           

            for (int i = 0; i < listTime.Count; i++)
            {
                //DateTime t1 = DateTime.Parse("2022/09/12 11:00:00");


                //if (i < 50)
                //    Console.WriteLine(listID[i].EntryTime.TimeOfDay + "  " + listID[i].Id + "#\t" + listID[i].EntryTime.TimeOfDay.ToString() + "," + listID[i].DataStr);

                //if(listID[i].EntryTime.TimeOfDay > t1.TimeOfDay){}

                listPresentIdInt.Add(Convert.ToInt32(listID[i].Id));
                File.AppendAllText(TODAY_OUTPUT_PATH_TIMEWISE, listTime[i].EntryTime.TimeOfDay + "," + listTime[i].DataStr + ","+ "\n");
                File.AppendAllText(TODAY_OUTPUT_PATH_ID, listID[i].DataStr + "," + listID[i].EntryTime.TimeOfDay +"\n");


                //if (i==10) break; 
            }
            foreach (int i in listFullId)
            {
                if (!listPresentIdInt.Contains(i))
                {
                    File.AppendAllText(TODAY_OUTPUT_ABSENT_PATH_ID, i + "\n");

                }
            }



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
