using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace BioMetrixCore
{
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static string CONNECTION_STRING = @"Server=DESKTOP-VGQL2VE\\SQL19;Database=Pwd.Cms;Trusted_Connection=True";

        private static string logPath = @"log.txt";
        private static string debugPath = @"debug.txt";

        //public static string IP_ADDRESS = "172.16.1.72";
        public static string IP_ADDRESS = "172.16.1.74";
        public static int PORT = 4370;
        public static int DEFAULT_MACHINE_NUMBER = 1;

        //static TimeSpan waitingTime = new TimeSpan(0, 10, 0);
        static TimeSpan waitingTime = new TimeSpan(0, 0, 10);


        public static void debug(string content)
        {
            File.AppendAllText(debugPath, content+"\n");
        }

        public static void writeToFile(string content)
        {
            string path = logPath;
            File.AppendAllText(path, content+"\n");
        }

        public static void log(string content)
        {            
            File.AppendAllText(logPath, content+"\n");
        }

        public static void writeToFileWithoutNL(string content)
        {
            string path = logPath;
            File.AppendAllText(path, content);
        }

        [STAThread]
        static void Main()
        {            
            writeToFile("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
            //runUI();
            //runCode();
            periodicLogFetching();
            //periodicLogFetchingOneConnection();
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


                //stopwatchClear.Start();
                //master._clearLog();
                //stopwatchClear.Stop();
                //logClearTime = stopwatchClear.ElapsedMilliseconds;                

                master._disconnet();


                Console.WriteLine("logReadingTime: "+logReadingTime+"\t\tconnectingTime: "+connectingTime.ToString()+ "\t\tlogClearTime: "+logClearTime.ToString());
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
            //master._clearLog();

            stopwatch.Stop();
            logReadingTime = stopwatch.ElapsedMilliseconds;


            Console.WriteLine("logReadingTime: "+logReadingTime+"\t\tconnectingTime: "+connectingTime.ToString());
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
            Console.WriteLine("Connection Done# Time: "+connectingTime);

            while (true)
            {                
                stopwatch.Start();

                master._getLog();
                //master._clearLog();

                stopwatch.Stop();
                logReadingTime = stopwatch.ElapsedMilliseconds;


                Console.WriteLine("logReadingTime: "+logReadingTime.ToString());
                Thread.Sleep(waitingTime);
            }
        }

        static void runUI() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Master());
        }
    }
}
