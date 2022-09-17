using BioMetrixCore.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace BioMetrixCore
{
    class LocalSMS
    {        
        public static string CONNECTION_STRING = @"Server=DESKTOP-VGQL2VE\\SQL19;Database=Pwd.Cms;Trusted_Connection=True";

        //public static string DEFAULT_PATH = @"";
        public static string DEFAULT_PATH = @"E:\PWD\BioMatrix\";
        public static string USER_INFO_CSV_FILE_PATH = DEFAULT_PATH + "usr.csv";
        public static string NOTIFICATION_FLAG_FILE_PATH = DEFAULT_PATH + "NotificationFlagFile.txt";

        private static string logPath = DEFAULT_PATH + "log.txt";
        private static string debugPath = DEFAULT_PATH + "debug.txt";

        // public static string IP_ADDRESS = "172.16.1.72";
        public static string IP_ADDRESS = "172.16.1.74";
        public static int PORT = 4370;
        public static int DEFAULT_MACHINE_NUMBER = 1;

        //static TimeSpan waitingTime = new TimeSpan(0, 10, 0);
        static TimeSpan waitingTime = new TimeSpan(0, 0, 10);
                
        public static List<UserEntry> uniqueEntrys = new List<UserEntry>();
        public static List<int> listPresentIdInt = new List<int>();
        public static List<int> listFullId = new List<int>();

        public LocalSMS()
        {
        }

        public void processLocalSMS()
        {
            HashSet<int> isNotficationSent = new HashSet<int>();

            DateTime time = DateTime.Now;
            
            //Console.WriteLine(time);
            //Console.WriteLine(time.DayOfWeek);

            Dictionary<int, UserEntry> usrInfoMap = loadUserInfoFromFile();

            //List<UserEntry> userEntries = getAllInDataFromAllMachines();
            List<UserEntry> userEntries = getTodayInDataDummy();

            List<UserEntry> notificationSentList = getNotificationSentList();

            foreach (UserEntry userEntry in notificationSentList)
            {
                isNotficationSent.Add(userEntry.Id);
            }

            //First Entry of Everyone
            foreach (UserEntry entry in userEntries)
            {
                if (entry.EntryTime.Day != time.Day || entry.EntryTime.Month != time.Month) continue;

                if (isNotficationSent.Contains(entry.Id)) continue;    //Skip if not the first entry of person @entry
                isNotficationSent.Add(entry.Id);

                UserEntry info = null;

                if (usrInfoMap.ContainsKey(entry.Id))
                {
                    info = usrInfoMap[entry.Id];
                    sendNotificationBySmS(info);
                    writeNotificationSentFile(info);
                    uniqueEntrys.Add(entry);
                }                                               
            }

            //var listTime = uniqueEntrys.OrderBy(x => x.EntryTime).ToList();

            //var listID = uniqueEntrys.OrderBy(x => x.Id).ToList();
        }

        static List<UserEntry> getAllInDataFromAllMachines()
        {
            List<UserEntry> entryList = new List<UserEntry>();

            Master master = new Master();
            master._ConnectOnly();
            List<UserEntry> machineAList = master._getLog();
            master._disconnet();

            Console.WriteLine(machineAList.Count);
            IP_ADDRESS = "172.16.1.73";

            Master master73 = new Master();
            master73._ConnectOnly();
            List<UserEntry> machineBList = master73._getLog();
            master73._disconnet();
            Console.WriteLine(machineBList.Count);

            entryList.AddRange(machineAList);
            entryList.AddRange(machineBList);

            return entryList;
        }

        void writeNotificationSentFile(UserEntry info)
        {
            string content = "";
            
            content = info.Id.ToString();
            content += "\t$"+info.EntryTime.Day.ToString();
            content += "$" + info.EntryTime.Month.ToString();
            content += "$" + info.EntryTime.Year.ToString();
            content += "\t$" + info.EntryTime.Hour.ToString();
            content += "$" + info.EntryTime.Minute.ToString();
            content += "$" + info.EntryTime.Second.ToString();
            content += "\t$" + info.Name;
            content += "\t$" + info.Designation;
            
            Console.WriteLine(content);

            //File.AppendAllText(NOTIFICATION_FLAG_FILE_PATH, content + "\n");
        }

        void sendNotificationBySmS(UserEntry info)
        {
            Console.WriteLine("Notification Sent to {0}", info.Id);
        }

        Dictionary<int, UserEntry> loadUserInfoFromFile()
        {
            Dictionary<int, UserEntry> info = new Dictionary<int, UserEntry>();           
            string[] lines = System.IO.File.ReadAllLines(USER_INFO_CSV_FILE_PATH);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                string[] tokens = line.Split(',');

                for (int j = 0; j < tokens.Length; j++)
                {
                    tokens[j] = tokens[j].Trim();
                }

                int id = Convert.ToInt32(tokens[0]);                
                string name = tokens[1];
                string designation = tokens[3];
                info[id] = new UserEntry(id, name, designation);
            }
            return info;
        }

        void generateFileOutput() {
            //string TODAY_OUTPUT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-idWise.csv";
            //string TODAY_OUTPUT_PATH_TIMEWISE = @"F:\" + MONTH + "_" + DAY + "_22-timeWise.csv";
            //string TODAY_OUTPUT_ABSENT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-absent.csv";


            //string TODAY_OUTPUT_PATH_ID = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-idWise.csv";
            //string TODAY_OUTPUT_PATH_TIMEWISE = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-timeWise.csv";
            //string TODAY_OUTPUT_ABSENT_PATH_ID = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-absent.csv";
        }
    

        DateTime readTodayTime() {
            DateTime today = new DateTime();
                //Console.WriteLine("Enter Month: ");
                //MONTH = Convert.ToInt32(Console.ReadLine());

                //Console.WriteLine("Enter Day: ");
                //DAY = Convert.ToInt32(Console.ReadLine());

                //Console.WriteLine("Enter Hour: ");
                //HOUR = Convert.ToInt32(Console.ReadLine());

                //Console.WriteLine("Enter Minute: ");
                //MINUTE = Convert.ToInt32(Console.ReadLine());        
                return today;
        }
        
        
        List<UserEntry> getNotificationSentList()
        {
            List<UserEntry> list = new List<UserEntry>();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(NOTIFICATION_FLAG_FILE_PATH);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    //Console.WriteLine(line);    
                    string[] tokens = line.Split('$');
                    for (int j = 0; j < tokens.Length; j++)
                    {
                        tokens[j] = tokens[j].Trim();
                    }

                    int id = Convert.ToInt32(tokens[0]);
                    int day = Convert.ToInt32(tokens[1]);
                    int month = Convert.ToInt32(tokens[2]);
                    int year = Convert.ToInt32(tokens[3]);
                    int hour = Convert.ToInt32(tokens[4]);
                    int minute = Convert.ToInt32(tokens[5]);
                    int second = Convert.ToInt32(tokens[6]);
                    int dayOfTheWeek = 0;

                    string name = tokens[7];
                    string designation = tokens[8];

                    list.Add(new UserEntry(id, name, designation, "", new DateTime(year, month, dayOfTheWeek, day, hour, minute, second)));
                }
            }
            catch (IOException e)
            {
                //if (lines == null) return list;
            }
                               
            return list;
        }
        
        List<UserEntry> getTodayInDataDummy()
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
    }
}
