using BioMetrixCore.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace BioMetrixCore
{
    class LocalSMS
    {        
        public static string CONNECTION_STRING = @"Server=DESKTOP-VGQL2VE\\SQL19;Database=Pwd.Cms;Trusted_Connection=True";

        public static string DEFAULT_PATH = @"F:\";
        //public static string DEFAULT_PATH = @"E:\PWD\";
        public static string USER_INFO_CSV_FILE_PATH = DEFAULT_PATH + "usr.csv";
        public static string NOTIFICATION_FLAG_FILE_PATH = DEFAULT_PATH + "NotificationFlagFile.txt";

        private static string logPath = DEFAULT_PATH + "log.txt";
        private static string debugPath = DEFAULT_PATH + "debug.txt";

        // public static string IP_ADDRESS = "172.16.1.72";
        public static string IP_ADDRESS = "172.16.1.74";
        public static int PORT = 4370;
        public static int DEFAULT_MACHINE_NUMBER = 1;

        //static TimeSpan waitingTime = new TimeSpan(0, 10, 0);


        static TimeSpan waitingTime = new TimeSpan(0, 3, 0);
        static TimeSpan waitingTimeAfterEachSMS = new TimeSpan(0, 0, 10);

        //static TimeSpan waitingTime = new TimeSpan(0, 0, 20);
        //static TimeSpan waitingTimeAfterEachSMS = new TimeSpan(0, 0, 2);

        public static List<UserEntry> uniqueEntrys = new List<UserEntry>();
        public static List<int> listPresentIdInt = new List<int>();
        public static List<int> listFullId = new List<int>();
        Dictionary<int, string> smsDestination = null;
        AlphaSMS smsManager;

        public LocalSMS()
        {
            smsManager = new AlphaSMS();

            smsDestination = new Dictionary<int, string>()
            {
                {   1, "01628287273"},      //CE
                {  17, "01628287273"},      //ACE Co.& Es.
                { 546, "01628287273"},      //EE,MIS-1    
                { 112, "01628287273"},      //EE,MIS-2    
                //{ 534, "01628287273"},      //SDE, MIS-2

                //{ 32, "01819207128"},       // SE Nandita
                //{ 19, "01819207128"},       // SE Shakhawat
                //{ 30, "01819207128"},       // EE Noor

                //{   9, "01710289237"},
                //{ 730, "01710289237"},
                //{ 808, "01710289237"},
                { 890, "01710289237"},

                //ACE
                {   19, "01819207128" },    // SE, Coordination
                {   32, "01819207128" },    // SE, EST
                
                //SE(Co.)
                {  27, "01819207958" },   // Executive
                {  28, "01819207958" },   // Executive

                //SE(Est.)
                {  30,  "01711386959"},    //EE, Establishment
                {  114, "01711386959"},   //Executive
                {  206, "01711386959"},   //Executive
            };
        }

        public void processLocalSMS()
        {
            //new AlphaSMS().sendSMS("PWD Entrence ID: 23", "01710289237");
            
            HashSet<int> isNotficationSent = new HashSet<int>();

            DateTime time = DateTime.Now;

            Console.WriteLine(time.TimeOfDay.Hours);
            Console.WriteLine(time.TimeOfDay.Minutes);
            Console.WriteLine(time.TimeOfDay.Seconds);

            //Console.WriteLine(time);
            //Console.WriteLine(time.DayOfWeek);

            Dictionary<int, UserEntry> usrInfoMap = loadUserInfoFromFile();


            while (true)
            {
                List<UserEntry> userEntries = getAllInDataFromAllMachines();
                //List<UserEntry> userEntries = getTodayInDataDummy(usrInfoMap);

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
                        info.EntryTime = entry.EntryTime;
                        sendNotificationBySmS(info);
                        writeNotificationSentFile(info, entry.EntryTime);
                        uniqueEntrys.Add(entry);
                    }
                }
                Console.WriteLine("Process Waiting Interval....");
                Thread.Sleep(waitingTime);
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

        void writeNotificationSentFile(UserEntry info, DateTime time)
        {
            string content = "";
            
            content = info.Id.ToString();
            content += "\t$"+ time.Day.ToString();
            content += "$" + time.Month.ToString();
            content += "$" + time.Year.ToString();
            content += "\t$" + time.Hour.ToString();
            content += "$" + time.Minute.ToString();
            content += "$" + time.Second.ToString();
            content += "\t$" + info.Name;
            content += "\t$" + info.Designation;
            
            //Console.WriteLine(content);

            File.AppendAllText(NOTIFICATION_FLAG_FILE_PATH, content + "\n");
        }

        void sendNotificationBySmS(UserEntry info)
        {
            if (smsDestination.ContainsKey(info.Id))
            {
                string reportingOfficerMobileNumberStr = smsDestination[info.Id];
                string timeStr = info.EntryTime.TimeOfDay.Hours.ToString() + ":"
                    + info.EntryTime.TimeOfDay.Minutes.ToString() + ":"
                    + info.EntryTime.TimeOfDay.Seconds.ToString();

                string messageBody = "Purta Bhavan Entrance Notification.\n"
                                    + "Employee ID: " + info.Id + ",\n"
                                    + "Employee Name: "+info.Name+" (" + info.Designation
                                    + ").\nEntry Time: "+ timeStr;

                Console.WriteLine("["+messageBody+"]");

                //Send SMS;
                Thread.Sleep(waitingTimeAfterEachSMS);
                smsManager.sendSMS(messageBody, reportingOfficerMobileNumberStr);
            }
            
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

                    string name = tokens[7];
                    string designation = tokens[8];                    

                    list.Add(new UserEntry(id, name, designation, "", new DateTime(year, month, day, hour, minute, second, 0)));
                }
            }
            catch (IOException e)
            {
                //if (lines == null) return list;
            }
                               
            return list;
        }
        
        List<UserEntry> getTodayInDataDummy(Dictionary<int, UserEntry> usrInfoMap)
        {
            List<UserEntry> list = new List<UserEntry>();
            Random rnd = new Random();
            for (int i = 0; i < 5; i++)
            {
                int id = rnd.Next(15, 25);
                DateTime time = DateTime.Now;
                Console.WriteLine(time + "   => "+ id);
                list.Add(new UserEntry(id, usrInfoMap[id].Name, usrInfoMap[id].Designation, time));
            }
            //list.Add(new UserEntry(504, "A S M Musa", "7:50:04"));
            //list.Add(new UserEntry(505, "A S M kusa", "8:50:04"));
            //list.Add(new UserEntry(506, "A S M Tusa", "8:52:04"));
            //list.Add(new UserEntry(507, "A S M Nusa", "8:50:04"));
            //list.Add(new UserEntry(508, "A S M Husa", "7:59:04"));
            //list.Add(new UserEntry(604, "A S M Lusa", "8:59:04"));
            //list.Add(new UserEntry(704, "A S M Busa", "7:58:04"));
            //list.Add(new UserEntry(304, "A S M Vusa", "8:57:04"));
            //list.Add(new UserEntry(204, "A S M Cusa", "8:30:04"));
            //list.Add(new UserEntry(104, "A S M Zusa", "8:55:04"));
            //userEntries.Add(new UserEntry(654, "A S M Qusa", "7:55:04"));
            //userEntries.Add(new UserEntry(54, "A S M Eusa", "8:53:04"));
            //userEntries.Add(new UserEntry(56, "A S M Rusa", "8:34:04"));
            //userEntries.Add(new UserEntry(44, "A S M Yusa", "8:20:04"));
            //userEntries.Add(new UserEntry(24, "A S M Gusa", "8:10:04"));
            return list;
        }

        void generateFileOutput()
        {
            //string TODAY_OUTPUT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-idWise.csv";
            //string TODAY_OUTPUT_PATH_TIMEWISE = @"F:\" + MONTH + "_" + DAY + "_22-timeWise.csv";
            //string TODAY_OUTPUT_ABSENT_PATH_ID = @"F:\" + MONTH + "_" + DAY + "_22-absent.csv";


            //string TODAY_OUTPUT_PATH_ID = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-idWise.csv";
            //string TODAY_OUTPUT_PATH_TIMEWISE = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-timeWise.csv";
            //string TODAY_OUTPUT_ABSENT_PATH_ID = @"E:\PWD\BioMatrix\" + MONTH + "_" + DAY + "_22-absent.csv";
        }


        DateTime readTodayTime()
        {
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
    }
}
