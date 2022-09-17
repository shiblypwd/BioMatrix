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
        public static int HOUR;
        public static int MINUTE;
        

        public static string CONNECTION_STRING = @"Server=DESKTOP-VGQL2VE\\SQL19;Database=Pwd.Cms;Trusted_Connection=True";

       // public static string USER_INFO_CSV_FILE_PATH = @"F:\usr.csv";
        public static string USER_INFO_CSV_FILE_PATH = @"E:\PWD\BioMatrix\usr.csv";

        public static string NOTIFICATION_FLAG_FILE_PATH = @"E:\PWD\BioMatrix\NotificationFlagFile.txt";





        private static string logPath = @"log.txt";

        private static string debugPath = @"debug.txt";

       // public static string IP_ADDRESS = "172.16.1.72";
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
        //public static List<int> absentList = new List<int>();



        [STAThread]
        static void Main()
        {

            //Console.WriteLine("Enter Month: ");
            //MONTH = Convert.ToInt32(Console.ReadLine());

            //Console.WriteLine("Enter Day: ");
            //DAY = Convert.ToInt32(Console.ReadLine());

            //Console.WriteLine("Enter Hour: ");
            //HOUR = Convert.ToInt32(Console.ReadLine());

            //Console.WriteLine("Enter Minute: ");
            //MINUTE = Convert.ToInt32(Console.ReadLine());

            MONTH = 9;
            DAY = 15;
            HOUR = 9;
            MINUTE = 30;

            Console.WriteLine(MONTH+ "-"+ DAY+" :: "+HOUR+":"+MINUTE);

            Console.WriteLine("\n\n");

            //string TODAY_OUTPUT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-idWise.csv";
            //string TODAY_OUTPUT_PATH_TIMEWISE = @"F:\" + MONTH + "_" + DAY + "_22-timeWise.csv";
            //string TODAY_OUTPUT_ABSENT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-absent.csv";


            string TODAY_OUTPUT_PATH_ID = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-idWise.csv";
            string TODAY_OUTPUT_PATH_TIMEWISE = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-timeWise.csv";
            string TODAY_OUTPUT_ABSENT_PATH_ID = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-absent.csv";

            loadInfoFromFile();
            //userEntries = getTodayInData();
            userEntries = getTodayInDataDummy();

            HashSet<int> st = new HashSet<int>();

            HashSet<int> notificationSentFlat = new HashSet<int>();

            setNotificationSentItemsIntoSet(ref notificationSentFlat);

            //First Entry of Everyone
            foreach (UserEntry entry in userEntries)
            {
                if (st.Contains(entry.Id)) continue;    //Skip if not the first entry of person @entry
                st.Add(entry.Id);

                if (notificationSentFlat.Contains(entry.Id))
                {
                    continue;
                }
                string name = ""; //Need to add later
                string designation = ""; //nedd to add later

                sendNotificationBySmS(entry.Id, name, designation);
                notificationSentFlat.Add(entry.Id);

                //Write id, date in sent_file

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
                //DateTime t1 = DateTime.Parse(HOUR+":"+MINUTE+":"+"00");

                //DateTime t1 = DateTime.Parse(HOUR + ":30:00");



                //if (i < 50)
                //    Console.WriteLine(listID[i].EntryTime.TimeOfDay + "  " + listID[i].Id + "#\t" + listID[i].EntryTime.TimeOfDay.ToString() + "," + listID[i].DataStr);

                //if (listID[i].EntryTime.TimeOfDay < t1.TimeOfDay) 
                {

                    //listPresentIdInt.Add(Convert.ToInt32(listID[i].Id));
                    //File.AppendAllText(TODAY_OUTPUT_PATH_TIMEWISE, listTime[i].EntryTime.TimeOfDay + "," + listTime[i].DataStr + "," + "\n");
                    //File.AppendAllText(TODAY_OUTPUT_PATH_ID, listID[i].DataStr + "," + listID[i].EntryTime.TimeOfDay + "\n");

                    File.AppendAllText(TODAY_OUTPUT_PATH_ID, listTime[i].Id + "," + listTime[i].DataStr + listTime[i].EntryTime + "\n");
                    File.AppendAllText(TODAY_OUTPUT_PATH_TIMEWISE, listID[i].EntryTime + "," + listID[i].Id + listID[i].DataStr +"\n");


                }





                //if (i==10) break; 
            }
            //foreach (int i in listFullId)
            //{
            //    if (!listPresentIdInt.Contains(i))
            //    {
            //        File.AppendAllText(TODAY_OUTPUT_ABSENT_PATH_ID, info[i] +"\n");

            //    }
            //}



            //runUI();
            //runCode();
            //periodicLogFetching();
            //periodicLogFetchingOneConnection();

        }


        static void setNotificationSentItemsIntoSet(ref HashSet<int> notificationSentFlat)
        {
            int id = 0;
            string[] lines = System.IO.File.ReadAllLines(NOTIFICATION_FLAG_FILE_PATH);

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
        }
        static void loadInfoFromFile() 
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
                listFullId.Add(id);
                info[id] = line;
            }
        }


        static List<UserEntry> getTodayInData()
        {
            List<UserEntry> entryList = new List<UserEntry>();

            Master master = new Master();
            master._ConnectOnly();
            List<UserEntry> machineAList= master._getLog();
            master._disconnet();

            Console.WriteLine(userEntries.Count);
            IP_ADDRESS = "172.16.1.73";

            Master master73 = new Master();
            master73._ConnectOnly();
            List<UserEntry> machineBList = master73._getLog();
            master73._disconnet();
            Console.WriteLine(userEntries.Count);

            entryList.AddRange(machineAList);
            entryList.AddRange(machineBList);

            return entryList;
        }

        static List<UserEntry> getTodayInDataDummy()
        {
            List<UserEntry> list = new List<UserEntry>();
            list.Add(new UserEntry(504, "A S M Musa", "7:50:04"));
            list.Add(new UserEntry(505, "A S M kusa", "8:50:04"));
            list.Add(new UserEntry(506, "A S M Tusa", "8:52:04"));
            list.Add(new UserEntry(507, "A S M Nusa", "8:50:04"));
            list.Add(new UserEntry(508, "A S M Husa", "7:59:04"));
            list.Add(new UserEntry(604, "A S M Lusa", "8:59:04"));
            list.Add(new UserEntry(704, "A S M Busa", "7:58:04"));
            list.Add(new UserEntry(304, "A S M Vusa", "8:57:04"));
            list.Add(new UserEntry(204, "A S M Cusa", "8:30:04"));
            list.Add(new UserEntry(104, "A S M Zusa", "8:55:04"));
            //userEntries.Add(new UserEntry(654, "A S M Qusa", "7:55:04"));
            //userEntries.Add(new UserEntry(54, "A S M Eusa", "8:53:04"));
            //userEntries.Add(new UserEntry(56, "A S M Rusa", "8:34:04"));
            //userEntries.Add(new UserEntry(44, "A S M Yusa", "8:20:04"));
            //userEntries.Add(new UserEntry(24, "A S M Gusa", "8:10:04"));
            return list;
        }

        static void sendNotificationBySmS(int id, string name, string designation)
        {

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
    }
}
