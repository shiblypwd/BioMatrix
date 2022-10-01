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
using RestSharp;

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
        //public static List<int> absentList = new List<int>();

        static string timeFormat = "yyyy-MM-ddTHH:mm:ss.fffT";

        [STAThread]
        static void Main()
        {
            RestSharp();


            //new LocalSMS().processLocalSMS();


            //new LocalSMS().ClearInGateLogs();
        }

        public static void RestSharp()
        {
            DateTime currentTime = DateTime.Now;
            var client = new RestClient("http://hrapi.pwdsoft.org/api/app/attendance");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);
            //var request = new RestRequest("resource/{id}");
            var request = new RestRequest();
            request.AddParameter("id", 0);
            request.AddParameter("attendanceCardId", 19);
            request.AddParameter("entryTime", currentTime.ToString(timeFormat));
            request.AddHeader("machineName", "InGate-1");
            request.AddHeader("verifyMode", "Finger");
            request.AddHeader("inOutStatus", 1);
            //request.AddFile("file", path);
            var response = client.Post(request);
            var content = response.Content; // Raw content as string
            Console.WriteLine(content);
            //var response2 = client.Post<Person>(request);
            //var name = response2.Data.Name;
        }

        public static void testPostCurl()
        {
            DateTime currentTime = DateTime.Now;
            timeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
            Console.WriteLine(currentTime.ToString(timeFormat));

            string machineName = "InGate-1";
            string verifyMood = "Finger";
            int inOut = 1;

            int startId = Convert.ToInt32(Console.ReadLine());
            int n = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < n; i++)
            {
                string command = getCurlCommand(startId + i, machineName, verifyMood, inOut);
                Console.WriteLine(command);
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(command);
                Console.WriteLine(procStartInfo.ErrorDialog);
            }
        }

        public static string getCurlCommand(int employeeId, string machineName, string verifyMood, int inOut) {
            //string api = "https://localhost:44399/api/app/attendance";
            string api = "http://hrapi.pwdsoft.org/api/app/attendance";
            
            DateTime currentTime = DateTime.Now;            

            string comamnd = "curl -X POST \""
                + api
                + "\" -H  \"accept: text/plain\" -H \"Content-Type: application/json\" -d "
                + "{\n\"id\": 0,\n\"attendanceCardId\": " + employeeId.ToString()
                + ",\n\"entryTime\": \"" + currentTime.ToString(timeFormat)
                + "\",\n\"machineName\": \"" + machineName
                + "\",\n\"verifyMode\": \"" + verifyMood
                + "\",\n\"inOutStatus\": " + inOut.ToString()
                + "\n}";

            return comamnd;
        }
        
    }
}
